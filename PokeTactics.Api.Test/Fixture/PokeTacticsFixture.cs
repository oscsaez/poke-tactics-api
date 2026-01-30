using Microsoft.AspNetCore.Mvc.Testing;
using WireMock.Server;
using Microsoft.Extensions.Configuration;
using PokeTactics.Contracts.Ability.PokeApi;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using PokeTactics.Contracts.Move.PokeApi;
using PokeTactics.Contracts.Pokemon.PokeApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using PokeTactics.Api.Test.Utils;
using MySqlConnector;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PokeTactics.Api.Utils;
using Microsoft.EntityFrameworkCore;
using PokeTactics.Infrastructure.Data;
using PokeTactics.Core.Interfaces;
using System.Net.Http.Json;
using PokeTactics.Core.Utils.Extensions;
using Microsoft.AspNetCore.HttpsPolicy;


namespace PokeTactics.Api.Test.Fixture;

public class PokeTacticsFixture : IAsyncLifetime
{
    private const string RootConnectionString = "server=localhost;user=root;password=password;port=3333;";
    private const string ExternalApiName = "PokeApi";
    private const string PokeApiAbilityPath = "/ability";
    private const string PokeApiMovePath = "/move";
    private const string PokeApiPokemonPath = "/pokemon";
    private const string LimitParam = "limit";

    private static readonly string _databaseName = $"poketactics_test_{TestGenerator.RandomGuidAsString()}";

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public HttpClient ApiClient { get; private set; } = null!;

    public WireMockServer PokeApiMockServer { get; private set; } = null!;

    public WebApplicationFactory<Program> Factory { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        // Database configuration
        await using (var connection = new MySqlConnection(RootConnectionString))
        {
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = $"CREATE DATABASE `{_databaseName}`;";
            await command.ExecuteNonQueryAsync();
        }

        string connectionString = $"{RootConnectionString}database={_databaseName};";

        // External API mocks
        PokeApiMockServer = WireMockServer.Start();

        // Real API configuration
        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment(EnvironmentConstants.TestingEnvironmentName);

                builder.ConfigureAppConfiguration((_, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:DefaultConnection"] = connectionString
                    });
                });

                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<PokeTacticsContext>>();
                    services.RemoveAll<PokeTacticsContext>();

                    services.AddDbContext<PokeTacticsContext>(options =>
                    {
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                    });

                    services.PostConfigure<HttpsRedirectionOptions>(options =>
                    {
                        options.HttpsPort = null;
                    });

                    services.AddHttpClient(ExternalApiName, client =>
                    {
                        client.BaseAddress = new Uri(PokeApiMockServer.Url!);
                    });
                });
            });

        ApiClient = Factory.CreateClient();
        await CreateDatabase();
    }

    public async Task DisposeAsync()
    {
        ApiClient.Dispose();
        await Factory.DisposeAsync();
        PokeApiMockServer.Stop();
        PokeApiMockServer.Dispose();
        
        await using var connection = new MySqlConnection(RootConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = $"DROP DATABASE IF EXISTS `{_databaseName}`;";
        await command.ExecuteNonQueryAsync();
    }

    #region Mock external services

    public void ConfigurePokeApiMockServerForGet<TResponse>(string path, TResponse response) where TResponse : class
    {
        PokeApiMockServer
            .Given(Request.Create().WithPath(path).UsingGet())
            .RespondWith(Response.Create().WithSuccess().WithBody(JsonSerializer.Serialize(response, _jsonOptions)));
    }

    public void ConfigurePokeApiMockServerToGetEmptyAbilitiesSummary()
    {
        AbilitySummaryListPokeApiResponse emptyResponse = new()
        {
            Count = 0,
            Results = []
        };

        ConfigurePokeApiMockServerToGetAbilitiesSummary(emptyResponse);
    }

    public void ConfigurePokeApiMockServerToGetAbilitiesSummary(AbilitySummaryListPokeApiResponse response)
    {
        PokeApiMockServer
            .Given(Request.Create().WithPath(PokeApiAbilityPath).WithParam(LimitParam, int.MaxValue.ToString()).UsingGet())
            .RespondWith(Response.Create().WithSuccess().WithBodyAsJson(response));
    }

    public void ConfigurePokeApiMockServerToGetEmptyMovesSummary()
    {
        MoveSummaryListPokeApiResponse emptyResponse = new()
        {
            Count = 0,
            Results = []
        };

        ConfigurePokeApiMockServerToGetMovesSummary(emptyResponse);
    }

    public void ConfigurePokeApiMockServerToGetMovesSummary(MoveSummaryListPokeApiResponse response)
    {
        PokeApiMockServer
            .Given(Request.Create().WithPath(PokeApiMovePath).WithParam(LimitParam, int.MaxValue.ToString()).UsingGet())
            .RespondWith(Response.Create().WithSuccess().WithBodyAsJson(response));
    }

    public void ConfigurePokeApiMockServerToGetEmptyPokemonSummaryList()
    {
        PokemonSummaryListPokeApiResponse emptyResponse = new()
        {
            Count = 0,
            Results = []
        };

        ConfigurePokeApiMockServerToGetPokemonSummaryList(emptyResponse);
    }

    public void ConfigurePokeApiMockServerToGetPokemonSummaryList(PokemonSummaryListPokeApiResponse response)
    {
        PokeApiMockServer
            .Given(Request.Create().WithPath(PokeApiPokemonPath).WithParam(LimitParam, int.MaxValue.ToString()).UsingGet())
            .RespondWith(Response.Create().WithSuccess().WithBodyAsJson(response));
    }

    #endregion

    #region REST simplified methods

    public async Task<ApiResponse<TResponse>> GetAsync<TResponse>(string requestedUri) where TResponse : class
    {
        Uri uri = requestedUri.ToUri(UriKind.Relative);
        HttpResponseMessage httpResponse = await ApiClient.GetAsync(uri);
        TResponse? body = await httpResponse.Content.ReadFromJsonAsync<TResponse>();

        return new ApiResponse<TResponse>(body, httpResponse.StatusCode);
    }

    #endregion

    public IServiceScope CreateScope()
    {
        return Factory.Services.CreateScope();
    }

    public TService GetService<TService>()
    {
        return Factory.Services.GetService<TService>()
            ?? throw new ArgumentNullException($"Invalid service {typeof(TService)}, it is not registered");
    }

    public async Task DeleteAll()
    {
        using IServiceScope scope = CreateScope();
        IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        await unitOfWork.AbilityDao.DeleteAllAsync();
        await unitOfWork.MoveDao.DeleteAllAsync();
        await unitOfWork.PokemonDao.DeleteAllAsync();
    }

    private async Task CreateDatabase()
    {
        using IServiceScope scope = CreateScope();
        PokeTacticsContext db = scope.ServiceProvider.GetRequiredService<PokeTacticsContext>();
        await db.Database.MigrateAsync();
    }
}