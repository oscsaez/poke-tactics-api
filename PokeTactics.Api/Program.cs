using Microsoft.EntityFrameworkCore;
using PokeTactics.Api.Utils;
using PokeTactics.Infrastructure.Data;
using PokeTactics.Infrastructure;
using System.Text.Json;
using PokeTactics.Core.Definitions;
using PokeTactics.Api;

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

builder.Services.AddOpenApi();
builder.Services.AddApiServices();
builder.Services.AddInfrastructureServices();

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
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.UseMiddleware<ExceptionHandler>();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
