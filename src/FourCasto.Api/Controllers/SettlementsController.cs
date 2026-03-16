namespace FourCasto.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FourCasto.Api.Extensions;
using FourCasto.Application.Interfaces;
using FourCasto.Contracts.Enums;
using FourCasto.Domain.Markets;
using FourCasto.Infrastructure.Persistence;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SettlementsController : ControllerBase
{
    private readonly FourCastoDbContext _db;
    private readonly ISettlementService _settlementService;

    public SettlementsController(FourCastoDbContext db, ISettlementService settlementService)
    {
        _db = db;
        _settlementService = settlementService;
    }

    public record CreateSettlementDto(
        Guid MarketId,
        SignalOutcomeType OutcomeType,
        decimal? ResolvedPrice,
        string SettledBy);

    [HttpPost]
    public async Task<IActionResult> CreateSettlement([FromBody] CreateSettlementDto dto)
    {
        // Find market and signal
        var market = await _db.Markets
            .Include(m => m.Signal)
            .FirstOrDefaultAsync(m => m.Id == dto.MarketId);

        if (market == null) return NotFound("Market not found");

        // Create signal outcome
        var outcome = new SignalOutcome
        {
            SignalId = market.SignalId!.Value,
            OutcomeType = dto.OutcomeType,
            ResolvedPrice = dto.ResolvedPrice
        };
        _db.SignalOutcomes.Add(outcome);

        // Update signal status
        if (market.Signal != null)
        {
            market.Signal.Status = SignalStatus.SETTLED;
            market.Signal.ResolvedAt = DateTime.UtcNow;
        }

        market.Status = MarketStatus.SETTLED;
        await _db.SaveChangesAsync();

        // Create settlement
        var settlementId = await _settlementService.CreateSettlementAsync(
            dto.MarketId, outcome.Id, dto.SettledBy);

        // Settle all open bets for this market
        var openBets = await _db.Bets
            .Where(b => b.MarketId == dto.MarketId && b.Status == BetStatus.OPEN)
            .ToListAsync();

        var results = new List<object>();
        foreach (var bet in openBets)
        {
            var result = await _settlementService.SettleBetAsync(bet.Id, settlementId);
            results.Add(new { bet.Id, result.PayoutType, result.PayoutAmount, result.PnlAmount });
        }

        // Confirm settlement
        var settlement = await _db.Settlements.FindAsync(settlementId);
        if (settlement != null)
        {
            settlement.Status = SettlementStatus.CONFIRMED;
            settlement.ConfirmedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        return Ok(new { SettlementId = settlementId, BetsSettled = results.Count, Results = results });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSettlement(Guid id)
    {
        var settlement = await _db.Settlements.FindAsync(id);
        if (settlement == null) return NotFound();

        var payouts = await _db.BetPayouts
            .Where(p => p.SettlementId == id)
            .ToListAsync();

        return Ok(new { Settlement = settlement, Payouts = payouts });
    }
}
