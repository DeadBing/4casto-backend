namespace FourCasto.Application.Interfaces;

using FourCasto.Contracts.Enums;

public record SettleBetResult(
    Guid SettlementId,
    PayoutType PayoutType,
    decimal PayoutAmount,
    decimal PnlAmount
);

public interface ISettlementService
{
    Task<Guid> CreateSettlementAsync(Guid marketId, Guid signalOutcomeId, string settledBy);
    Task<SettleBetResult> SettleBetAsync(Guid betId, Guid settlementId);
}
