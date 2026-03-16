namespace FourCasto.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FourCasto.Contracts.Enums;
using FourCasto.Domain.Accounts;
using FourCasto.Infrastructure.Persistence;

[ApiController]
[Route("api/trading-accounts")]
public class TradingAccountsController : ControllerBase
{
    private readonly FourCastoDbContext _db;

    public TradingAccountsController(FourCastoDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAccounts(
        [FromQuery] Guid fourCastoWlId,
        [FromQuery] Guid userId)
    {
        var accounts = await _db.TradingAccounts
            .Include(a => a.Balance)
            .Where(a => a.FourCastoWlId == fourCastoWlId && a.UserId == userId)
            .Select(a => new
            {
                a.Id, a.AccountType, a.AccountNumber, a.CurrencyCode, a.IsActive, a.CreatedAt,
                Balance = a.Balance == null ? null : new
                {
                    a.Balance.TotalBalance, a.Balance.AvailableBalance,
                    a.Balance.LockedBalance, a.Balance.BonusCredit,
                    a.Balance.Equity, a.Balance.WithdrawableBalance
                }
            })
            .ToListAsync();

        return Ok(accounts);
    }

    public record CreateAccountDto(
        Guid FourCastoWlId,
        Guid UserId,
        AccountType AccountType,
        decimal InitialBalance = 0);

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto dto)
    {
        var account = new TradingAccount
        {
            FourCastoWlId = dto.FourCastoWlId,
            UserId = dto.UserId,
            AccountType = dto.AccountType,
            AccountNumber = $"{dto.AccountType.ToString()[0]}{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}"
        };
        _db.TradingAccounts.Add(account);
        await _db.SaveChangesAsync();

        var balance = new TradingAccountBalance
        {
            TradingAccountId = account.Id,
            TotalBalance = dto.InitialBalance,
            AvailableBalance = dto.InitialBalance
        };
        _db.TradingAccountBalances.Add(balance);
        await _db.SaveChangesAsync();

        return Created($"/api/trading-accounts/{account.Id}", new { account.Id, account.AccountNumber });
    }

    [HttpGet("{id:guid}/balance")]
    public async Task<IActionResult> GetBalance(Guid id)
    {
        var balance = await _db.TradingAccountBalances
            .FirstOrDefaultAsync(b => b.TradingAccountId == id);

        if (balance == null) return NotFound();
        return Ok(balance);
    }
}
