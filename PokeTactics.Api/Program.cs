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
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// Health checks
app.MapGet("/health", () => Results.Ok("Healthy"));

// Route group for Pokemon
RouteGroupBuilder pokemonGroup = app.MapGroup("/pokemon");
pokemonGroup.MapPokemonEndpoints();

app.UseMiddleware<ExceptionHandler>();

app.Run();

public partial class Program {}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
