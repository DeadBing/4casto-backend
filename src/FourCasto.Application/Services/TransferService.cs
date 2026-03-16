namespace FourCasto.Application.Services;

using FourCasto.Application.Interfaces;
using FourCasto.Contracts.Enums;
using FourCasto.Domain.Accounts;
using FourCasto.Infrastructure.Persistence;

public class TransferService : ITransferService
{
    private readonly FourCastoDbContext _db;
    private readonly IBalanceService _balanceService;
    private readonly IIdempotencyService _idempotencyService;

    public TransferService(
        FourCastoDbContext db,
        IBalanceService balanceService,
        IIdempotencyService idempotencyService)
    {
        _db = db;
        _balanceService = balanceService;
        _idempotencyService = idempotencyService;
    }

    public async Task<TransferResult> ExecuteTransferAsync(TransferRequest request)
    {
        var check = await _idempotencyService.CheckAsync("Transfer", request.IdempotencyKey);
        if (check.AlreadyProcessed)
            return System.Text.Json.JsonSerializer.Deserialize<TransferResult>(check.ResultPayload!)!;

        await _idempotencyService.MarkProcessingAsync("Transfer", request.IdempotencyKey);

        try
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
            var transfer = new WalletTransfer
            {
                FourCastoWlId = request.FourCastoWlId,
                UserId = request.UserId,
                WalletId = request.WalletId,
                TradingAccountId = request.TradingAccountId,
                Amount = request.Amount,
                Direction = request.Direction,
                Status = TransferStatus.PENDING,
                IdempotencyKey = request.IdempotencyKey
            };
            _db.WalletTransfers.Add(transfer);
            await _db.SaveChangesAsync();

            if (request.Direction == TransferDirection.WALLET_TO_TRADING)
            {
                await _balanceService.TransferToTradingAccountAsync(
                    request.WalletId, request.TradingAccountId, request.Amount,
                    request.FourCastoWlId, request.UserId);
            }
            else
            {
                await _balanceService.TransferToWalletAsync(
                    request.TradingAccountId, request.WalletId, request.Amount,
                    request.FourCastoWlId, request.UserId);
            }

            transfer.Status = TransferStatus.COMPLETED;
            transfer.ProcessedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var result = new TransferResult(transfer.Id, TransferStatus.COMPLETED);
            await _idempotencyService.MarkCompletedAsync("Transfer", request.IdempotencyKey,
                System.Text.Json.JsonSerializer.Serialize(result));

            await transaction.CommitAsync();
            return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch
        {
            await _idempotencyService.MarkFailedAsync("Transfer", request.IdempotencyKey);
            throw;
        }
    }
}
