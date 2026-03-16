namespace FourCasto.Application.Interfaces;

using FourCasto.Contracts.Enums;

public record SignalProgressResult(
    decimal ProgressPercent,
    ProgressDirection Direction,
    bool IsAvailableForBetting
);

public interface ISignalProgressCalculator
{
    SignalProgressResult Calculate(
        decimal entryPrice,
        decimal targetPrice,
        decimal stopLossPrice,
        decimal currentPrice,
        decimal maxBettingProgressPercent);
}
