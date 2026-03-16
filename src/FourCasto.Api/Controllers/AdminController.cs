namespace FourCasto.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FourCasto.Contracts.Enums;
using FourCasto.Domain.Admin;
using FourCasto.Domain.Pricing;
using FourCasto.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly FourCastoDbContext _db;

    public AdminController(FourCastoDbContext db)
    {
        _db = db;
    }

    public record AdminOverrideDto(
        Guid FourCastoWlId,
        Guid AdminUserId,
        AdminActionType ActionType,
        string RefType,
        Guid RefId,
        string Reason,
        string? Details);

    [HttpPost("override")]
    public async Task<IActionResult> CreateOverride([FromBody] AdminOverrideDto dto)
    {
        var overrideEntry = new AdminOverride
        {
            FourCastoWlId = dto.FourCastoWlId,
            AdminUserId = dto.AdminUserId,
            ActionType = dto.ActionType,
            RefType = dto.RefType,
            RefId = dto.RefId,
            Reason = dto.Reason,
            Details = dto.Details
        };
        _db.AdminOverrides.Add(overrideEntry);
        await _db.SaveChangesAsync();

        return Ok(new { overrideEntry.Id });
    }

    public record PriceTickDto(Guid SubjectId, decimal Price, Guid? QuoteSourceId);

    [HttpPost("price-tick")]
    public async Task<IActionResult> PostPriceTick([FromBody] PriceTickDto dto)
    {
        // Save tick
        _db.PriceTicks.Add(new PriceTick
        {
            SubjectId = dto.SubjectId,
            QuoteSourceId = dto.QuoteSourceId,
            Price = dto.Price
        });

        // Save snapshot
        _db.PriceSnapshots.Add(new PriceSnapshot
        {
            SubjectId = dto.SubjectId,
            QuoteSourceId = dto.QuoteSourceId,
            Price = dto.Price
        });

        await _db.SaveChangesAsync();

        return Ok(new { SubjectId = dto.SubjectId, Price = dto.Price, Timestamp = DateTime.UtcNow });
    }

    public record ConfigUpdateDto(
        Guid FourCastoWlId,
        string Description,
        string CreatedBy);

    [HttpPost("config")]
    public async Task<IActionResult> CreateConfigVersion([FromBody] ConfigUpdateDto dto)
    {
        var latestVersion = await _db.ConfigVersions
            .Where(v => v.FourCastoWlId == dto.FourCastoWlId)
            .MaxAsync(v => (int?)v.VersionNumber) ?? 0;

        var version = new Domain.CountryRules.ConfigVersion
        {
            FourCastoWlId = dto.FourCastoWlId,
            VersionNumber = latestVersion + 1,
            Description = dto.Description,
            CreatedBy = dto.CreatedBy
        };
        _db.ConfigVersions.Add(version);
        await _db.SaveChangesAsync();

        return Ok(new { version.Id, version.VersionNumber });
    }
}
