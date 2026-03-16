using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FourCasto.Infrastructure.Persistence;
using FourCasto.Application.Interfaces;
using FourCasto.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// MySQL + EF Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Port=3306;Database=fourcasto;User=root;Password=root;";

var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));

builder.Services.AddDbContext<FourCastoDbContext>(options =>
    options.UseMySql(connectionString, serverVersion, mysqlOptions =>
    {
        mysqlOptions.MigrationsAssembly("FourCasto.Infrastructure");
        mysqlOptions.EnableRetryOnFailure(3);
    }));

// Authentication (JWT)
var jwtKey = builder.Configuration["Jwt:Key"] ?? "4CastoDefaultDevKey_MustChange_In_Production_1234567890!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "FourCasto";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// Application Services
builder.Services.AddScoped<IBalanceService, BalanceService>();
builder.Services.AddScoped<ISignalProgressCalculator, SignalProgressCalculator>();
builder.Services.AddScoped<IPayoutCalculationService, PayoutCalculationService>();
builder.Services.AddScoped<IBetZoneEvaluator, BetZoneEvaluator>();
builder.Services.AddScoped<IPolicyResolutionService, PolicyResolutionService>();
builder.Services.AddScoped<IIdempotencyService, IdempotencyService>();
builder.Services.AddScoped<IBetPlacementService, BetPlacementService>();
builder.Services.AddScoped<IBetCancellationService, BetCancellationService>();
builder.Services.AddScoped<ISettlementService, SettlementService>();
builder.Services.AddScoped<ITransferService, TransferService>();
builder.Services.AddScoped<ICountryStatusEvaluator, CountryStatusEvaluator>();
builder.Services.AddScoped<SignalProgressRuleEngine>();

// Controllers + JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "4Casto API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new()
    {
        Description = "JWT Authorization header. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
