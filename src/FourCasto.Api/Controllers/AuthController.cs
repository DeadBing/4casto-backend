namespace FourCasto.Api.Controllers;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FourCasto.Contracts.Enums;
using FourCasto.Domain.Accounts;
using FourCasto.Domain.Identity;
using FourCasto.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly FourCastoDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(FourCastoDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public record RegisterDto(Guid FourCastoWlId, string Email, string Password, string? CountryCode);

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var exists = await _db.Users.AnyAsync(u =>
            u.FourCastoWlId == dto.FourCastoWlId && u.Email == dto.Email);

        if (exists) return Conflict("Email already registered");

        var user = new User
        {
            FourCastoWlId = dto.FourCastoWlId,
            Email = dto.Email,
            PasswordHash = BCryptHash(dto.Password),
            AuthProvider = AuthProvider.EMAIL,
            IsGuest = false
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Create profile
        _db.UserProfiles.Add(new UserProfile
        {
            UserId = user.Id,
            CountryCode = dto.CountryCode ?? "US"
        });

        // Create wallet
        var wallet = new Wallet { FourCastoWlId = dto.FourCastoWlId, UserId = user.Id };
        _db.Wallets.Add(wallet);
        await _db.SaveChangesAsync();

        _db.WalletBalances.Add(new WalletBalance { WalletId = wallet.Id });

        // Create WL user mapping
        _db.FourCastoWlUsers.Add(new FourCastoWlUser
        {
            FourCastoWlId = dto.FourCastoWlId,
            UserId = user.Id,
            Role = UserRole.USER
        });

        await _db.SaveChangesAsync();

        var token = GenerateJwt(user);
        return Ok(new { UserId = user.Id, Token = token });
    }

    public record LoginDto(Guid FourCastoWlId, string Email, string Password);

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.FourCastoWlId == dto.FourCastoWlId && u.Email == dto.Email);

        if (user == null || user.PasswordHash == null || !VerifyPassword(dto.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials");

        user.LastLoginAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var token = GenerateJwt(user);
        return Ok(new { UserId = user.Id, Token = token });
    }

    public record GuestDto(Guid FourCastoWlId);

    [AllowAnonymous]
    [HttpPost("guest")]
    public async Task<IActionResult> CreateGuest([FromBody] GuestDto dto)
    {
        var guestEmail = $"guest_{Guid.NewGuid():N}@4casto.local";
        var user = new User
        {
            FourCastoWlId = dto.FourCastoWlId,
            Email = guestEmail,
            IsGuest = true,
            AuthProvider = AuthProvider.EMAIL
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Create demo trading account
        var account = new TradingAccount
        {
            FourCastoWlId = dto.FourCastoWlId,
            UserId = user.Id,
            AccountType = AccountType.DEMO,
            AccountNumber = $"D{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}"
        };
        _db.TradingAccounts.Add(account);
        await _db.SaveChangesAsync();

        _db.TradingAccountBalances.Add(new TradingAccountBalance
        {
            TradingAccountId = account.Id,
            TotalBalance = 10000m,
            AvailableBalance = 10000m
        });
        await _db.SaveChangesAsync();

        return Ok(new { UserId = user.Id, TradingAccountId = account.Id, IsGuest = true });
    }

    public record ClaimDemoDto(Guid FourCastoWlId, Guid GuestUserId, string Email, string Password, string? CountryCode);

    [AllowAnonymous]
    [HttpPost("claim-demo")]
    public async Task<IActionResult> ClaimDemo([FromBody] ClaimDemoDto dto)
    {
        var guest = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == dto.GuestUserId && u.IsGuest);

        if (guest == null) return NotFound("Guest user not found");

        var emailTaken = await _db.Users.AnyAsync(u =>
            u.FourCastoWlId == dto.FourCastoWlId && u.Email == dto.Email && !u.IsGuest);

        if (emailTaken) return Conflict("Email already registered");

        // Convert guest to real user
        guest.Email = dto.Email;
        guest.PasswordHash = BCryptHash(dto.Password);
        guest.IsGuest = false;

        // Create profile
        _db.UserProfiles.Add(new UserProfile
        {
            UserId = guest.Id,
            CountryCode = dto.CountryCode ?? "US"
        });

        // Create wallet
        var wallet = new Wallet { FourCastoWlId = dto.FourCastoWlId, UserId = guest.Id };
        _db.Wallets.Add(wallet);
        await _db.SaveChangesAsync();

        _db.WalletBalances.Add(new WalletBalance { WalletId = wallet.Id });
        await _db.SaveChangesAsync();

        var token = GenerateJwt(guest);
        return Ok(new { UserId = guest.Id, Token = token, Claimed = true });
    }

    private string GenerateJwt(User user)
    {
        var key = _config["Jwt:Key"] ?? "4CastoDefaultDevKey_MustChange_In_Production_1234567890!";
        var issuer = _config["Jwt:Issuer"] ?? "FourCasto";

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("fourcastowl_id", user.FourCastoWlId.ToString()),
            new Claim("is_guest", user.IsGuest.ToString())
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Simple hash for MVP (in production use proper BCrypt library)
    private static string BCryptHash(string password) =>
        Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(password)));

    private static bool VerifyPassword(string password, string hash) =>
        BCryptHash(password) == hash;
}
