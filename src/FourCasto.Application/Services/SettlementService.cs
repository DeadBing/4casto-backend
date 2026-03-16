namespace FourCasto.Application.Services;

using Microsoft.EntityFrameworkCore;
using FourCasto.Application.Interfaces;
using FourCasto.Contracts.Enums;
using FourCasto.Domain.Settlement;
using FourCasto.Infrastructure.Persistence;

public class SettlementService : ISettlementService
{
    private readonly FourCastoDbContext _db;
    private readonly IBalanceService _balanceService;

    public SettlementService(FourCastoDbContext db, IBalanceService balanceService)
    {
        _db = db;
        _balanceService = balanceService;
    }

    public async Task<Guid> CreateSettlementAsync(Guid marketId, Guid signalOutcomeId, string settledBy)
    {
        var settlement = new Domain.Settlement.Settlement
        {
            MarketId = marketId,
            SignalOutcomeId = signalOutcomeId,
            Status = SettlementStatus.PENDING,
            SettledBy = settledBy
        };
        _db.Settlements.Add(settlement);
        await _db.SaveChangesAsync();
        return settlement.Id;
    }

    public async Task<SettleBetResult> SettleBetAsync(Guid betId, Guid settlementId)
    {
        var bet = await _db.Bets
            .Include(b => b.PayoutSnapshot)
            .Include(b => b.Market).ThenInclude(m => m!.Signal).ThenInclude(s => s!.Outcome)
            .FirstOrDefaultAsync(b => b.Id == betId)
            ?? throw new InvalidOperationException($"Bet {betId} not found");

        if (bet.Status != BetStatus.OPEN)
            throw new InvalidOperationException($"Bet is not open. Status: {bet.Status}");

        var snapshot = bet.PayoutSnapshot
            ?? throw new InvalidOperationException("BetPayoutSnapshot not found");

        var outcome = bet.Market?.Signal?.Outcome
            ?? throw new InvalidOperationException("Signal outcome not found");

        // Create execution record
        var execution = new BetSettlementExecution
        {
            BetId = betId,
            SettlementId = settlementId,
            Status = SettlementExecutionStatus.IN_PROGRESS,
            AttemptNumber = 1
        };
        _db.BetSettlementExecutions.Add(execution);

        // Determine win/loss
        bool isWin = DetermineWin(bet.Direction, outcome.OutcomeType);
        var payoutType = isWin ? PayoutType.WIN : PayoutType.LOSS;

        decimal payoutAmount;
        decimal pnlAmount;

        if (isWin)
        {
            // Use historical snapshot for payout calculation
            payoutAmount = snapshot.TotalReturn;
            pnlAmount = snapshot.PotentialProfit;
        }
        else
        {
            payoutAmount = 0;
            pnlAmount = -bet.StakeAmount;
        }

        // Create bet payout record
        var betPayout = new BetPayout
        {
            BetId = betId,
            SettlementId = settlementId,
            PayoutAmount = payoutAmount,
            PnlAmount = pnlAmount,
            PayoutType = payoutType
        };
        _db.BetPayouts.Add(betPayout);

        // Release hold
        var releaseReason = isWin ? HoldReleaseReason.SETTLED_WIN : HoldReleaseReason.SETTLED_LOSS;
        await _balanceService.ReleaseFundsAsync(
            bet.TradingAccountId, betId, bet.StakeAmount, releaseReason, bet.FourCastoWlId);

        if (isWin)
        {
            // Credit winnings (stake + profit)
            await _balanceService.CreditWinningsAsync(
                bet.TradingAccountId, payoutAmount, betId, bet.FourCastoWlId);
        }
        // For loss: stake is already gone (was locked, now released from locked but not added to available)

        bet.Status = BetStatus.SETTLED;
        bet.SettledAt = DateTime.UtcNow;

        execution.Status = SettlementExecutionStatus.COMPLETED;
        execution.CompletedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return new SettleBetResult(settlementId, payoutType, payoutAmount, pnlAmount);
    }

    private static bool DetermineWin(BetDirection direction, SignalOutcomeType outcomeType)
    {
        return direction switch
        {
            BetDirection.AGREE => outcomeType == SignalOutcomeType.TARGET_REACHED
                || outcomeType == SignalOutcomeType.EXPIRED_PROFIT,
            BetDirection.DISAGREE => outcomeType == SignalOutcomeType.STOP_LOSS_REACHED
                || outcomeType == SignalOutcomeType.EXPIRED_LOSS,
            _ => false
        };
    }
}
