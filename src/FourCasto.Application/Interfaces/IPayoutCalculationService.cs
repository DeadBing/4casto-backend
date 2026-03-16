namespace FourCasto.Application.Interfaces;

using FourCasto.Contracts.Enums;

public record PayoutCalculationResult(
    decimal BasePayoutPercent,
    decimal ProgressAdjustmentPercent,
    decimal FinalPayoutPercent,
    decimal PotentialProfit,
    decimal TotalReturn
);

public interface IPayoutCalculationService
{
    Task<PayoutCalculationResult> CalculateAsync(
        Guid fourCastoWlId,
        Guid marketId,
        BetDirection direction,
        decimal stakeAmount,
        decimal currentPrice,
        string countryCode,
        Guid? countryStatusId);
}
