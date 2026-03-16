namespace FourCasto.Application.Services;

using Microsoft.EntityFrameworkCore;
using FourCasto.Application.Interfaces;
using FourCasto.Contracts.Enums;
using FourCasto.Domain.Admin;
using FourCasto.Infrastructure.Persistence;

public class IdempotencyService : IIdempotencyService
{
    private readonly FourCastoDbContext _db;

    public IdempotencyService(FourCastoDbContext db)
    {
        _db = db;
    }

    public async Task<IdempotencyCheckResult> CheckAsync(string operationType, string idempotencyKey)
    {
        var record = await _db.IdempotencyRecords
            .FirstOrDefaultAsync(r => r.OperationType == operationType && r.IdempotencyKey == idempotencyKey);

        if (record == null)
            return new IdempotencyCheckResult(false, null);

        if (record.Status == IdempotencyStatus.COMPLETED)
            return new IdempotencyCheckResult(true, record.ResultPayload);

        if (record.Status == IdempotencyStatus.PROCESSING)
            throw new InvalidOperationException($"Operation {operationType}/{idempotencyKey} is already being processed");

        return new IdempotencyCheckResult(false, null);
    }

    public async Task MarkProcessingAsync(string operationType, string idempotencyKey)
    {
        _db.IdempotencyRecords.Add(new IdempotencyRecord
        {
            OperationType = operationType,
            IdempotencyKey = idempotencyKey,
            Status = IdempotencyStatus.PROCESSING,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        });
        await _db.SaveChangesAsync();
    }

    public async Task MarkCompletedAsync(string operationType, string idempotencyKey, string? resultPayload)
    {
        var record = await _db.IdempotencyRecords
            .FirstOrDefaultAsync(r => r.OperationType == operationType && r.IdempotencyKey == idempotencyKey);

        if (record != null)
        {
            record.Status = IdempotencyStatus.COMPLETED;
            record.ResultPayload = resultPayload;
            await _db.SaveChangesAsync();
        }
    }

    public async Task MarkFailedAsync(string operationType, string idempotencyKey)
    {
        var record = await _db.IdempotencyRecords
            .FirstOrDefaultAsync(r => r.OperationType == operationType && r.IdempotencyKey == idempotencyKey);

        if (record != null)
        {
            record.Status = IdempotencyStatus.FAILED;
            await _db.SaveChangesAsync();
        }
    }
}
