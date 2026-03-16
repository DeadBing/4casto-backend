namespace FourCasto.Application.Services;

using FourCasto.Application.Interfaces;
using FourCasto.Contracts.Enums;

public class BetZoneEvaluator : IBetZoneEvaluator
{
    public ZoneEvaluationResult Evaluate(
        BetDirection betDirection,
        SignalDirection signalDirection,
        decimal entryPrice,
        decimal currentPrice)
    {
        bool isUpSignal = signalDirection == SignalDirection.UP;
        bool priceMovedUp = currentPrice > entryPrice;
        bool priceMovedDown = currentPrice < entryPrice;

        bool isPositive;
        string reason;

        if (betDirection == BetDirection.AGREE)
        {
            // Agree wins when signal hits target
            // For UP signal: positive if price went up
            // For DOWN signal: positive if price went down
            isPositive = isUpSignal ? priceMovedUp : priceMovedDown;
            reason = isPositive
                ? "Price moved in favorable direction for AGREE bet"
                : "Price moved against AGREE bet - cancellation not allowed";
        }
        else
        {
            // Disagree wins when signal hits stop
            // For UP signal: positive if price went down
            // For DOWN signal: positive if price went up
            isPositive = isUpSignal ? priceMovedDown : priceMovedUp;
            reason = isPositive
                ? "Price moved in favorable direction for DISAGREE bet"
                : "Price moved against DISAGREE bet - cancellation not allowed";
        }

        // If price hasn't moved, it's neutral (not positive)
        if (currentPrice == entryPrice)
        {
            isPositive = false;
            reason = "Price unchanged from entry - no positive zone established";
        }

        return new ZoneEvaluationResult(isPositive, reason);
    }
}
