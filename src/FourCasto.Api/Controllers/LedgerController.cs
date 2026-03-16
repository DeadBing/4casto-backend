namespace FourCasto.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FourCasto.Contracts.Enums;
using FourCasto.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class LedgerController : ControllerBase
{
    private readonly FourCastoDbContext _db;

    public LedgerController(FourCastoDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetEntries(
        [FromQuery] Guid fourCastoWlId,
        [FromQuery] Guid? accountId = null,
        [FromQuery] LedgerAccountType? accountType = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = _db.LedgerEntries
            .Where(e => e.FourCastoWlId == fourCastoWlId);

        if (accountId.HasValue)
            query = query.Where(e => e.AccountId == accountId.Value);

        if (accountType.HasValue)
            query = query.Where(e => e.AccountType == accountType.Value);

        var total = await query.CountAsync();

        var entries = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new
            {
                e.Id, e.AccountType, e.AccountId, e.BalanceType,
                e.EntryType, e.Amount, e.RefType, e.RefId,
                e.BalanceTotalAfter, e.BalanceAvailableAfter, e.BalanceLockedAfter,
                e.CreatedAt
            })
            .ToListAsync();

        return Ok(new { Total = total, Page = page, PageSize = pageSize, Entries = entries });
    }
}
