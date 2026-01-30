using Microsoft.EntityFrameworkCore;
using PokeTactics.Api.Utils;
using PokeTactics.Infrastructure.Data;
using PokeTactics.Infrastructure;
using System.Text.Json;
using PokeTactics.Core.Definitions;
using PokeTactics.Api;
using PokeTactics.Api.Endpoints;
using PokeTactics.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
string? connectionString = configuration.GetConnectionString(ApiConstants.DefaultConnection);

// Add services to the container.
builder.Services.AddDbContext<PokeTacticsContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        x => x.MigrationsAssembly(ApiConstants.MigrationsAssembly)));

// Enable compatibility with camelCase json fields
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Configure PokemonSyncSettings
builder.Services.Configure<PokemonSyncSettings>(
    builder.Configuration.GetSection("PokemonSync"));

// Add swagger documentation (API and UI)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApiServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddServices();

if (!builder.Environment.IsEnvironment(EnvironmentConstants.TestingEnvironmentName))
{
    builder.Services.AddHostedServices();
}

WebApplication app = builder.Build();

// Apply migrations when starting service
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PokeTacticsContext>();
    await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandler>();

// Health checks
app.MapGet("/health", () => Results.Ok("Healthy"));

// Route group for Pokemon
RouteGroupBuilder pokemonGroup = app.MapGroup("/pokemon");
pokemonGroup.MapPokemonEndpoints();

await app.RunAsync();

public partial class Program
{
    protected Program()
    {
    }
}
