namespace FourCasto.Application.Interfaces;

public record IdempotencyCheckResult(
    bool AlreadyProcessed,
    string? ResultPayload
);

public interface IIdempotencyService
{
    Task<IdempotencyCheckResult> CheckAsync(string operationType, string idempotencyKey);
    Task MarkProcessingAsync(string operationType, string idempotencyKey);
    Task MarkCompletedAsync(string operationType, string idempotencyKey, string? resultPayload);
    Task MarkFailedAsync(string operationType, string idempotencyKey);
}
