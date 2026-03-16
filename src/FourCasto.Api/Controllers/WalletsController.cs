namespace FourCasto.Api.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FourCasto.Api.Extensions;
using FourCasto.Application.Interfaces;
using FourCasto.Contracts.Enums;
using FourCasto.Infrastructure.Persistence;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WalletsController : ControllerBase
{
    private readonly FourCastoDbContext _db;
    private readonly ITransferService _transferService;

    public WalletsController(FourCastoDbContext db, ITransferService transferService)
    {
        _db = db;
        _transferService = transferService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetWallet(Guid id)
    {
        var wallet = await _db.Wallets
            .Include(w => w.Balance)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (wallet == null) return NotFound();

        return Ok(new
        {
            wallet.Id, wallet.CurrencyCode, wallet.CreatedAt,
            Balance = wallet.Balance == null ? null : new
            {
                wallet.Balance.TotalBalance,
                wallet.Balance.AvailableBalance,
                wallet.Balance.LockedBalance,
                wallet.Balance.WithdrawableBalance
            }
        });
    }

    public record TransferDto(
        Guid TradingAccountId,
        decimal Amount,
        TransferDirection Direction,
        string IdempotencyKey);

    [HttpPost("{id:guid}/transfer")]
    public async Task<IActionResult> Transfer(Guid id, [FromBody] TransferDto dto)
    {
        var userId = User.GetUserId();
        var wlId = User.GetFourCastoWlId();

        var result = await _transferService.ExecuteTransferAsync(new TransferRequest(
            wlId, userId, id, dto.TradingAccountId,
            dto.Amount, dto.Direction, dto.IdempotencyKey));

        return Ok(result);
    }
}
