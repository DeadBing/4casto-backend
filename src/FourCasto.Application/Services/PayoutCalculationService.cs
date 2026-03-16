namespace FourCasto.Application.Services;

using Microsoft.EntityFrameworkCore;
using FourCasto.Application.Interfaces;
using FourCasto.Contracts.Enums;
using FourCasto.Infrastructure.Persistence;

public class PayoutCalculationService : IPayoutCalculationService
{
    private readonly FourCastoDbContext _db;
    private readonly ISignalProgressCalculator _progressCalculator;
    private readonly IPolicyResolutionService _policyService;
    private readonly SignalProgressRuleEngine _ruleEngine;

    public PayoutCalculationService(
        FourCastoDbContext db,
        ISignalProgressCalculator progressCalculator,
        IPolicyResolutionService policyService,
        SignalProgressRuleEngine ruleEngine)
    {
        _db = db;
        _progressCalculator = progressCalculator;
        _policyService = policyService;
        _ruleEngine = ruleEngine;
    }

    public async Task<PayoutCalculationResult> CalculateAsync(
        Guid fourCastoWlId,
        Guid marketId,
        BetDirection direction,
        decimal stakeAmount,
        decimal currentPrice,
        string countryCode,
        Guid? countryStatusId)
    {
        var market = await _db.Markets
            .Include(m => m.Signal)
            .Include(m => m.Subject)
            .FirstOrDefaultAsync(m => m.Id == marketId)
            ?? throw new InvalidOperationException($"Market {marketId} not found");

        var subjectGroupId = market.Subject?.SubjectGroupId;
        var subjectId = market.SubjectId;

        // Resolve base payout from policy
        var policy = await _policyService.ResolvePayoutPolicyAsync(
            fourCastoWlId, direction, countryCode, countryStatusId, subjectGroupId, subjectId);

        var basePayoutPercent = policy.PayoutPercent;

        // For signal markets, apply progress-based adjustment
        decimal progressAdjustment = 0;
        if (market.MarketType == MarketType.SIGNAL && market.Signal != null)
        {
            var signal = market.Signal;
            var progress = _progressCalculator.Calculate(
                signal.EntryPrice, signal.TargetPrice, signal.StopLossPrice,
                currentPrice, signal.MaxBettingProgressPercent);

            // Try DB rules first, fall back to formula
            var ruleAdjustment = await _ruleEngine.GetPayoutAdjustmentAsync(
                fourCastoWlId, subjectGroupId, progress.ProgressPercent, progress.Direction);

            if (ruleAdjustment != 0)
            {
                progressAdjustment = ruleAdjustment;
            }
            else
            {
                // Fallback formula
                if (progress.Direction == ProgressDirection.TOWARD_TARGET)
                    progressAdjustment = -(basePayoutPercent * Math.Abs(progress.ProgressPercent) / 100m);
                else if (progress.Direction == ProgressDirection.TOWARD_STOP)
                    progressAdjustment = basePayoutPercent * Math.Abs(progress.ProgressPercent) / 100m * 0.5m;
            }
        }

        var finalPayout = basePayoutPercent + progressAdjustment;
        // Clamp to [1%, 200%]
        finalPayout = Math.Clamp(finalPayout, 1m, 200m);
        finalPayout = Math.Round(finalPayout, 2);

        var potentialProfit = stakeAmount * finalPayout / 100m;
        var totalReturn = stakeAmount + potentialProfit;

        return new PayoutCalculationResult(
            basePayoutPercent,
            Math.Round(progressAdjustment, 2),
            finalPayout,
            Math.Round(potentialProfit, 2),
            Math.Round(totalReturn, 2));
    }
}
