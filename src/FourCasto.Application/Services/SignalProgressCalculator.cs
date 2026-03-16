namespace FourCasto.Application.Services;

using FourCasto.Application.Interfaces;
using FourCasto.Contracts.Enums;

public class SignalProgressCalculator : ISignalProgressCalculator
{
    public SignalProgressResult Calculate(
        decimal entryPrice,
        decimal targetPrice,
        decimal stopLossPrice,
        decimal currentPrice,
        decimal maxBettingProgressPercent)
    {
        var rangeToTarget = targetPrice - entryPrice;
        var rangeToStop = stopLossPrice - entryPrice;

        decimal progressToTarget = 0;
        decimal progressToStop = 0;

        if (rangeToTarget != 0)
            progressToTarget = (currentPrice - entryPrice) / rangeToTarget;

        if (rangeToStop != 0)
            progressToStop = (currentPrice - entryPrice) / rangeToStop;

        var progressPercent = progressToTarget * 100m;
        var direction = progressToTarget >= 0
            ? ProgressDirection.TOWARD_TARGET
            : ProgressDirection.TOWARD_STOP;

        if (Math.Abs(progressToTarget) < 0.001m && Math.Abs(progressToStop) < 0.001m)
            direction = ProgressDirection.NEUTRAL;

        var relevantProgress = direction == ProgressDirection.TOWARD_TARGET
            ? Math.Abs(progressPercent)
            : 0;

        var isAvailable = relevantProgress <= maxBettingProgressPercent;

        return new SignalProgressResult(
            Math.Round(progressPercent, 2),
            direction,
            isAvailable);
    }
}
