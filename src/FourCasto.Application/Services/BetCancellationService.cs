namespace FourCasto.Application.Services;

using Microsoft.EntityFrameworkCore;
using FourCasto.Application.Interfaces;
using FourCasto.Contracts.Enums;
using FourCasto.Domain.Settlement;
using FourCasto.Infrastructure.Persistence;

public class BetCancellationService : IBetCancellationService
{
    private readonly FourCastoDbContext _db;
    private readonly IBalanceService _balanceService;
    private readonly IBetZoneEvaluator _zoneEvaluator;
    private readonly IPolicyResolutionService _policyService;
    private readonly ICountryStatusEvaluator _statusEvaluator;
    private readonly IIdempotencyService _idempotencyService;

    public BetCancellationService(
        FourCastoDbContext db,
        IBalanceService balanceService,
        IBetZoneEvaluator zoneEvaluator,
        IPolicyResolutionService policyService,
        ICountryStatusEvaluator statusEvaluator,
        IIdempotencyService idempotencyService)
    {
        _db = db;
        _balanceService = balanceService;
        _zoneEvaluator = zoneEvaluator;
        _policyService = policyService;
        _statusEvaluator = statusEvaluator;
        _idempotencyService = idempotencyService;
    }

    public async Task<CancelBetResult> CancelBetAsync(CancelBetRequest request)
    {
        // Idempotency
        var check = await _idempotencyService.CheckAsync("CancelBet", request.IdempotencyKey);
        if (check.AlreadyProcessed)
            return System.Text.Json.JsonSerializer.Deserialize<CancelBetResult>(check.ResultPayload!)!;

        await _idempotencyService.MarkProcessingAsync("CancelBet", request.IdempotencyKey);

        try
        {
            var bet = await _db.Bets
                .Include(b => b.Market).ThenInclude(m => m!.Signal)
                .Include(b => b.Market).ThenInclude(m => m!.Subject)
                .FirstOrDefaultAsync(b => b.Id == request.BetId && b.UserId == request.UserId)
                ?? throw new InvalidOperationException("Bet not found");

            if (bet.Status != BetStatus.OPEN)
                throw new InvalidOperationException($"Bet cannot be cancelled. Status: {bet.Status}");

            // Check zone
            var signal = bet.Market?.Signal;
            if (signal == null)
                throw new InvalidOperationException("Signal not found for this bet's market");

            var latestPrice = await _db.PriceSnapshots
                .Where(p => p.SubjectId == bet.Market!.SubjectId)
                .OrderByDescending(p => p.SnapshotAt)
                .FirstOrDefaultAsync();

            var currentPrice = latestPrice?.Price ?? signal.EntryPrice;

            var zoneResult = _zoneEvaluator.Evaluate(
                bet.Direction, signal.SignalDirection, signal.EntryPrice, currentPrice);

            // Save eligibility snapshot
            _db.BetCancellationEligibilitySnapshots.Add(new BetCancellationEligibilitySnapshot
            {
                BetId = bet.Id,
                CurrentMarketPrice = currentPrice,
                EntryPrice = signal.EntryPrice,
                BetDirection = bet.Direction,
                SignalDirection = signal.SignalDirection,
                IsInPositiveZone = zoneResult.IsInPositiveZone,
                IsCancellationAllowed = zoneResult.IsInPositiveZone,
                DenialReason = zoneResult.IsInPositiveZone ? null : zoneResult.Reason
            });

            if (!zoneResult.IsInPositiveZone)
            {
                await _db.SaveChangesAsync();
                var deniedResult = new CancelBetResult(false, 0, 0, zoneResult.Reason);
                await _idempotencyService.MarkCompletedAsync("CancelBet", request.IdempotencyKey,
                    System.Text.Json.JsonSerializer.Serialize(deniedResult));
                return deniedResult;
            }

            // Resolve cancellation policy
            var statusResult = await _statusEvaluator.EvaluateAsync(bet.FourCastoWlId, bet.UserId);
            var cancellationPolicy = await _policyService.ResolveCancellationPolicyAsync(
                bet.FourCastoWlId, statusResult.CountryCode, statusResult.CountryStatusId,
                bet.Market?.Subject?.SubjectGroupId, bet.Market?.SubjectId);

            if (!cancellationPolicy.IsAllowed)
            {
                await _db.SaveChangesAsync();
                var notAllowedResult = new CancelBetResult(false, 0, 0, "Cancellation not allowed by policy");
                await _idempotencyService.MarkCompletedAsync("CancelBet", request.IdempotencyKey,
                    System.Text.Json.JsonSerializer.Serialize(notAllowedResult));
                return notAllowedResult;
            }

            // Calculate penalty
            var penaltyPercent = cancellationPolicy.PenaltyPercent;
            var penaltyAmount = bet.StakeAmount * penaltyPercent / 100m;
            var amountReturned = bet.StakeAmount - penaltyAmount;

            // Create cancellation request
            var cancelRequest = new BetCancellationRequest
            {
                BetId = bet.Id,
                UserId = request.UserId,
                IdempotencyKey = request.IdempotencyKey,
                Status = CancellationRequestStatus.EXECUTED
            };
            _db.BetCancellationRequests.Add(cancelRequest);
            await _db.SaveChangesAsync();

            // Execute cancellation
            var execution = new BetCancellationExecution
            {
                CancellationRequestId = cancelRequest.Id,
                BetId = bet.Id,
                PenaltyPercent = penaltyPercent,
                PenaltyAmount = penaltyAmount,
                AmountReturned = amountReturned,
                HoldReleaseAmount = bet.StakeAmount,
                Status = CancellationExecutionStatus.COMPLETED
            };
            _db.BetCancellationExecutions.Add(execution);

            // Release hold and return funds
            await _balanceService.ReleaseFundsAsync(
                bet.TradingAccountId, bet.Id, bet.StakeAmount,
                HoldReleaseReason.CANCELLED, bet.FourCastoWlId);

            // Credit returned amount
            if (amountReturned > 0)
            {
                await _balanceService.CreditWinningsAsync(
                    bet.TradingAccountId, amountReturned, bet.Id, bet.FourCastoWlId);
            }

            // Debit penalty
            if (penaltyAmount > 0)
            {
                await _balanceService.DebitPenaltyAsync(
                    bet.TradingAccountId, penaltyAmount, bet.Id, bet.FourCastoWlId);
            }

            bet.Status = BetStatus.CANCELLED;
            bet.CancelledAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var result = new CancelBetResult(true, penaltyAmount, amountReturned, null);
            await _idempotencyService.MarkCompletedAsync("CancelBet", request.IdempotencyKey,
                System.Text.Json.JsonSerializer.Serialize(result));

            return result;
        }
        catch
        {
            await _idempotencyService.MarkFailedAsync("CancelBet", request.IdempotencyKey);
            throw;
        }
    }
}
