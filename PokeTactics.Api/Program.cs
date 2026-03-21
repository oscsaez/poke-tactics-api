using Microsoft.EntityFrameworkCore;
using PokeTactics.Api.Utils;
using PokeTactics.Infrastructure.Data;
using PokeTactics.Infrastructure;
using System.Text.Json;
using PokeTactics.Core.Definitions;
using PokeTactics.Api;
using PokeTactics.Api.Endpoints;
using PokeTactics.Services;
using Microsoft.OpenApi.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
string? connectionString = configuration.GetConnectionString(ApiConstants.DefaultConnection);

// Add services to the container (db connection configuration).
var serverVersion = new MySqlServerVersion(new Version(8, 0));

builder.Services.AddDbContext<PokeTacticsContext>(options =>
    options.UseMySql(
        connectionString,
        serverVersion,
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
builder.Services.AddSwaggerGen(options =>
{
    var openApiInfo = new OpenApiInfo
    {
        Title = "PokeTactics API",
        Version = ApiConstants.ApiVersion
    };

    options.SwaggerDoc(ApiConstants.ApiVersion, openApiInfo);
    options.CustomSchemaIds(type => type.Name.Contains('`')
        ? type.Name.Substring(0, type.Name.IndexOf('`'))
        : type.Name);
});

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
    // This try-catch is for avoiding compilation failure of swagger generation process
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<PokeTacticsContext>();
        await dbContext.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"WARNING: Cannot connect to DB: {ex.Message}");
    }
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
