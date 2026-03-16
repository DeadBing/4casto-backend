namespace FourCasto.Application.Interfaces;

using FourCasto.Contracts.Enums;
using FourCasto.Domain.Markets;

public record PlaceBetRequest(
    Guid FourCastoWlId,
    Guid UserId,
    Guid TradingAccountId,
    Guid MarketId,
    BetDirection Direction,
    decimal StakeAmount,
    string? IdempotencyKey
);

public record PlaceBetResult(
    Guid BetId,
    decimal FinalPayoutPercent,
    decimal PotentialProfit,
    decimal TotalReturn
);

public interface IBetPlacementService
{
    Task<PlaceBetResult> PlaceBetAsync(PlaceBetRequest request);
}
