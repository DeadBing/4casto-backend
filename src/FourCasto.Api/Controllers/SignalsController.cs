namespace FourCasto.Api.Controllers;

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
public class SignalsController : ControllerBase
{
    private readonly FourCastoDbContext _db;
    private readonly ISignalProgressCalculator _progressCalculator;

    public SignalsController(FourCastoDbContext db, ISignalProgressCalculator progressCalculator)
    {
        _db = db;
        _progressCalculator = progressCalculator;
    }

    [HttpGet]
    public async Task<IActionResult> GetSignals(
        [FromQuery] Guid fourCastoWlId,
        [FromQuery] SignalStatus? status = null)
    {
        var query = _db.Signals
            .Include(s => s.Subject)
            .Where(s => s.FourCastoWlId == fourCastoWlId);

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        var signals = await query
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new
            {
                s.Id, s.SignalType, s.SignalDirection,
                s.EntryPrice, s.TargetPrice, s.StopLossPrice,
                s.Status, s.MaxBettingProgressPercent,
                s.BasePayoutPercentAgree, s.BasePayoutPercentDisagree,
                s.CreatedAt, s.ExpiresAt,
                SubjectSymbol = s.Subject.Symbol
            })
            .ToListAsync();

        return Ok(signals);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSignal(Guid id)
    {
        var signal = await _db.Signals
            .Include(s => s.Subject)
            .Include(s => s.Outcome)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (signal == null) return NotFound();
        return Ok(signal);
    }

    [HttpGet("{id:guid}/progress")]
    public async Task<IActionResult> GetSignalProgress(Guid id)
    {
        var signal = await _db.Signals.FindAsync(id);
        if (signal == null) return NotFound();

        var latestPrice = await _db.PriceSnapshots
            .Where(p => p.SubjectId == signal.SubjectId)
            .OrderByDescending(p => p.SnapshotAt)
            .FirstOrDefaultAsync();

        var currentPrice = latestPrice?.Price ?? signal.EntryPrice;

        var progress = _progressCalculator.Calculate(
            signal.EntryPrice, signal.TargetPrice, signal.StopLossPrice,
            currentPrice, signal.MaxBettingProgressPercent);

        return Ok(new
        {
            SignalId = id,
            CurrentPrice = currentPrice,
            progress.ProgressPercent,
            progress.Direction,
            progress.IsAvailableForBetting,
            PriceSnapshotAt = latestPrice?.SnapshotAt
        });
    }
}
