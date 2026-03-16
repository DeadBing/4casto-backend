namespace FourCasto.Application.Services;

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using FourCasto.Application.Interfaces;
using FourCasto.Contracts.Enums;
using FourCasto.Domain.Markets;
using FourCasto.Domain.CountryRules;
using FourCasto.Infrastructure.Persistence;

public class BetPlacementService : IBetPlacementService
{
    private readonly FourCastoDbContext _db;
    private readonly IBalanceService _balanceService;
    private readonly IPayoutCalculationService _payoutService;
    private readonly ISignalProgressCalculator _progressCalculator;
    private readonly ICountryStatusEvaluator _statusEvaluator;
    private readonly IIdempotencyService _idempotencyService;

    public BetPlacementService(
        FourCastoDbContext db,
        IBalanceService balanceService,
        IPayoutCalculationService payoutService,
        ISignalProgressCalculator progressCalculator,
        ICountryStatusEvaluator statusEvaluator,
        IIdempotencyService idempotencyService)
    {
        _db = db;
        _balanceService = balanceService;
        _payoutService = payoutService;
        _progressCalculator = progressCalculator;
        _statusEvaluator = statusEvaluator;
        _idempotencyService = idempotencyService;
    }

    public async Task<PlaceBetResult> PlaceBetAsync(PlaceBetRequest request)
    {
        // Idempotency check
        if (!string.IsNullOrEmpty(request.IdempotencyKey))
        {
            var check = await _idempotencyService.CheckAsync("PlaceBet", request.IdempotencyKey);
            if (check.AlreadyProcessed && check.ResultPayload != null)
                return JsonSerializer.Deserialize<PlaceBetResult>(check.ResultPayload)!;

            await _idempotencyService.MarkProcessingAsync("PlaceBet", request.IdempotencyKey);
        }

        try
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
            // Validate market
            var market = await _db.Markets
                .Include(m => m.Signal)
                .Include(m => m.Subject)
                .FirstOrDefaultAsync(m => m.Id == request.MarketId && m.FourCastoWlId == request.FourCastoWlId)
                ?? throw new InvalidOperationException("Market not found");

            if (market.Status != MarketStatus.OPEN)
                throw new InvalidOperationException($"Market is not open. Status: {market.Status}");

            // Check signal availability
            if (market.Signal != null)
            {
                var latestPrice = await _db.PriceSnapshots
                    .Where(p => p.SubjectId == market.SubjectId)
                    .OrderByDescending(p => p.SnapshotAt)
                    .FirstOrDefaultAsync();

                var currentPrice = latestPrice?.Price ?? market.Signal.EntryPrice;

                var progress = _progressCalculator.Calculate(
                    market.Signal.EntryPrice, market.Signal.TargetPrice,
                    market.Signal.StopLossPrice, currentPrice,
                    market.Signal.MaxBettingProgressPercent);

                if (!progress.IsAvailableForBetting)
                    throw new InvalidOperationException($"Signal has progressed beyond betting limit ({progress.ProgressPercent}%)");
            }

            // Check balance
            var available = await _balanceService.GetAvailableBalanceAsync(request.TradingAccountId);
            if (available < request.StakeAmount)
                throw new InvalidOperationException($"Insufficient funds. Available: {available}");

            // Get country status
            var statusResult = await _statusEvaluator.EvaluateAsync(request.FourCastoWlId, request.UserId);

            // Get current price for payout calculation
            var currentPriceForPayout = market.Signal?.EntryPrice ?? 0;
            var priceSnapshot = await _db.PriceSnapshots
                .Where(p => p.SubjectId == market.SubjectId)
                .OrderByDescending(p => p.SnapshotAt)
                .FirstOrDefaultAsync();
            if (priceSnapshot != null)
                currentPriceForPayout = priceSnapshot.Price;

            // Calculate payout
            var payoutResult = await _payoutService.CalculateAsync(
                request.FourCastoWlId, request.MarketId, request.Direction,
                request.StakeAmount, currentPriceForPayout,
                statusResult.CountryCode, statusResult.CountryStatusId);

            // Create bet
            var bet = new Bet
            {
                FourCastoWlId = request.FourCastoWlId,
                UserId = request.UserId,
                TradingAccountId = request.TradingAccountId,
                MarketId = request.MarketId,
                Direction = request.Direction,
                StakeAmount = request.StakeAmount,
                Status = BetStatus.OPEN
            };
            _db.Bets.Add(bet);
            await _db.SaveChangesAsync();

            // Create payout snapshot
            var signal = market.Signal;
            SignalProgressResult? progressResult = null;
            if (signal != null)
            {
                progressResult = _progressCalculator.Calculate(
                    signal.EntryPrice, signal.TargetPrice, signal.StopLossPrice,
                    currentPriceForPayout, signal.MaxBettingProgressPercent);
            }

            var snapshot = new BetPayoutSnapshot
            {
                BetId = bet.Id,
                FourCastoWlId = request.FourCastoWlId,
                UserId = request.UserId,
                CountryCode = statusResult.CountryCode,
                CountryStatusId = statusResult.CountryStatusId,
                SubjectGroupId = market.Subject?.SubjectGroupId,
                SubjectId = market.SubjectId,
                BetDirection = request.Direction,
                StakeAmount = request.StakeAmount,
                BasePayoutPercent = payoutResult.BasePayoutPercent,
                MarketPriceAtEntry = currentPriceForPayout,
                ProgressPercentAtEntry = progressResult?.ProgressPercent,
                ProgressDirectionAtEntry = progressResult?.Direction,
                ProgressAdjustmentPercent = payoutResult.ProgressAdjustmentPercent,
                FinalPayoutPercentApplied = payoutResult.FinalPayoutPercent,
                PotentialProfit = payoutResult.PotentialProfit,
                TotalReturn = payoutResult.TotalReturn
            };
            _db.BetPayoutSnapshots.Add(snapshot);

            // Create entry price context
            if (signal != null)
            {
                _db.BetEntryPriceContexts.Add(new BetEntryPriceContext
                {
                    BetId = bet.Id,
                    SignalId = signal.Id,
                    EntryPrice = signal.EntryPrice,
                    CurrentMarketPrice = currentPriceForPayout,
                    TargetPrice = signal.TargetPrice,
                    StopLossPrice = signal.StopLossPrice,
                    ProgressPercent = progressResult?.ProgressPercent ?? 0,
                    ProgressDirection = progressResult?.Direction ?? ProgressDirection.NEUTRAL
                });
            }

            await _db.SaveChangesAsync();

            // Lock funds
            await _balanceService.LockFundsAsync(
                request.TradingAccountId, bet.Id, request.StakeAmount,
                request.FourCastoWlId, request.UserId);

            // Log policy resolution
            _db.PolicyResolutionLogs.Add(new PolicyResolutionLog
            {
                BetId = bet.Id,
                RuleType = "PAYOUT",
                ResolvedPolicyId = null,
                FallbackChain = $"Payout={payoutResult.FinalPayoutPercent}%"
            });
            await _db.SaveChangesAsync();

            var result = new PlaceBetResult(
                bet.Id,
                payoutResult.FinalPayoutPercent,
                payoutResult.PotentialProfit,
                payoutResult.TotalReturn);

            if (!string.IsNullOrEmpty(request.IdempotencyKey))
                await _idempotencyService.MarkCompletedAsync("PlaceBet", request.IdempotencyKey, JsonSerializer.Serialize(result));

            await transaction.CommitAsync();
            return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch
        {
            if (!string.IsNullOrEmpty(request.IdempotencyKey))
                await _idempotencyService.MarkFailedAsync("PlaceBet", request.IdempotencyKey);
            throw;
        }
    }
}
