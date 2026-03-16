namespace FourCasto.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FourCasto.Api.Extensions;
using FourCasto.Contracts.Enums;
using FourCasto.Infrastructure.Persistence;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MarketsController : ControllerBase
{
    private readonly FourCastoDbContext _db;

    public MarketsController(FourCastoDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetMarkets(
        [FromQuery] Guid fourCastoWlId,
        [FromQuery] MarketType? type = null,
        [FromQuery] MarketStatus? status = null)
    {
        var query = _db.Markets
            .Include(m => m.Signal)
            .Include(m => m.Subject)
            .Where(m => m.FourCastoWlId == fourCastoWlId);

        if (type.HasValue)
            query = query.Where(m => m.MarketType == type.Value);

        if (status.HasValue)
            query = query.Where(m => m.Status == status.Value);

        var markets = await query
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new
            {
                m.Id, m.MarketType, m.Title, m.Description,
                m.Status, m.OpensAt, m.ClosesAt, m.CreatedAt,
                SignalId = m.SignalId,
                SubjectSymbol = m.Subject.Symbol,
                SubjectName = m.Subject.Name
            })
            .ToListAsync();

        return Ok(markets);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetMarket(Guid id)
    {
        var market = await _db.Markets
            .Include(m => m.Signal)
            .Include(m => m.Subject)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (market == null) return NotFound();
        return Ok(market);
    }
}
