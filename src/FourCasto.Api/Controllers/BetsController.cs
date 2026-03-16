namespace FourCasto.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FourCasto.Application.Interfaces;
using FourCasto.Contracts.Enums;
using FourCasto.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class BetsController : ControllerBase
{
    private readonly FourCastoDbContext _db;
    private readonly IBetPlacementService _placementService;
    private readonly IBetCancellationService _cancellationService;

    public BetsController(
        FourCastoDbContext db,
        IBetPlacementService placementService,
        IBetCancellationService cancellationService)
    {
        _db = db;
        _placementService = placementService;
        _cancellationService = cancellationService;
    }

    public record PlaceBetDto(
        Guid FourCastoWlId,
        Guid UserId,
        Guid TradingAccountId,
        Guid MarketId,
        BetDirection Direction,
        decimal StakeAmount,
        string? IdempotencyKey);

    [HttpPost]
    public async Task<IActionResult> PlaceBet([FromBody] PlaceBetDto dto)
    {
        var result = await _placementService.PlaceBetAsync(new PlaceBetRequest(
            dto.FourCastoWlId, dto.UserId, dto.TradingAccountId,
            dto.MarketId, dto.Direction, dto.StakeAmount, dto.IdempotencyKey));

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetBets(
        [FromQuery] Guid fourCastoWlId,
        [FromQuery] Guid userId,
        [FromQuery] BetStatus? status = null)
    {
        var query = _db.Bets
            .Where(b => b.FourCastoWlId == fourCastoWlId && b.UserId == userId);

        if (status.HasValue)
            query = query.Where(b => b.Status == status.Value);

        var bets = await query
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new
            {
                b.Id, b.MarketId, b.Direction, b.StakeAmount,
                b.Status, b.CreatedAt, b.SettledAt, b.CancelledAt
            })
            .ToListAsync();

        return Ok(bets);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBet(Guid id)
    {
        var bet = await _db.Bets
            .Include(b => b.PayoutSnapshot)
            .Include(b => b.EntryPriceContext)
            .Where(b => b.Id == id)
            .FirstOrDefaultAsync();

        if (bet == null) return NotFound();
        return Ok(bet);
    }

    public record CancelBetDto(string IdempotencyKey);

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelBet(Guid id, [FromBody] CancelBetDto dto)
    {
        // Get userId from bet (in production, from JWT)
        var bet = await _db.Bets.FindAsync(id);
        if (bet == null) return NotFound();

        var result = await _cancellationService.CancelBetAsync(
            new CancelBetRequest(id, bet.UserId, dto.IdempotencyKey));

        if (!result.Success)
            return BadRequest(new { result.DenialReason });

        return Ok(result);
    }
}
