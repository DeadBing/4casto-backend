namespace FourCasto.Application.Interfaces;

using FourCasto.Contracts.Enums;

public record ZoneEvaluationResult(
    bool IsInPositiveZone,
    string Reason
);

public interface IBetZoneEvaluator
{
    ZoneEvaluationResult Evaluate(
        BetDirection betDirection,
        SignalDirection signalDirection,
        decimal entryPrice,
        decimal currentPrice);
}
