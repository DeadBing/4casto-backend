namespace FourCasto.Application.Interfaces;

using FourCasto.Contracts.Enums;

public record TransferRequest(
    Guid FourCastoWlId,
    Guid UserId,
    Guid WalletId,
    Guid TradingAccountId,
    decimal Amount,
    TransferDirection Direction,
    string IdempotencyKey
);

public record TransferResult(
    Guid TransferId,
    TransferStatus Status
);

public interface ITransferService
{
    Task<TransferResult> ExecuteTransferAsync(TransferRequest request);
}
