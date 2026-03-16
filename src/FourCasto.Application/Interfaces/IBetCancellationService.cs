namespace FourCasto.Application.Interfaces;

public record CancelBetRequest(
    Guid BetId,
    Guid UserId,
    string IdempotencyKey
);

public record CancelBetResult(
    bool Success,
    decimal PenaltyAmount,
    decimal AmountReturned,
    string? DenialReason
);

public interface IBetCancellationService
{
    Task<CancelBetResult> CancelBetAsync(CancelBetRequest request);
}
