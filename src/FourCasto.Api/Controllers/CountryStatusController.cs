namespace FourCasto.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FourCasto.Application.Interfaces;
using FourCasto.Infrastructure.Persistence;

[ApiController]
[Route("api/country-status")]
public class CountryStatusController : ControllerBase
{
    private readonly FourCastoDbContext _db;
    private readonly ICountryStatusEvaluator _evaluator;

    public CountryStatusController(FourCastoDbContext db, ICountryStatusEvaluator evaluator)
    {
        _db = db;
        _evaluator = evaluator;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyStatus(
        [FromQuery] Guid fourCastoWlId,
        [FromQuery] Guid userId)
    {
        var result = await _evaluator.EvaluateAsync(fourCastoWlId, userId);
        return Ok(result);
    }

    [HttpGet("rules")]
    public async Task<IActionResult> GetRules([FromQuery] Guid fourCastoWlId)
    {
        var rules = await _db.CountryStatusQualificationRules
            .Include(r => r.CountryStatus)
            .Where(r => r.FourCastoWlId == fourCastoWlId)
            .Select(r => new
            {
                r.Id, r.CountryCode, r.MetricType, r.MinValue, r.MaxValue,
                StatusName = r.CountryStatus.Name
            })
            .ToListAsync();

        return Ok(rules);
    }
}
