using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.MySql;
using WireMock.Server;
using Microsoft.Extensions.Configuration;
using PokeTactics.Contracts.Ability.PokeApi;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using PokeTactics.Contracts.Move.PokeApi;
using PokeTactics.Contracts.Pokemon.PokeApi;

namespace PokeTactics.Api.Test.Fixture;

public class PokeTacticsFixture : IAsyncLifetime
{
    private readonly string pokeApiAbilityPath = $"/api/v2/ability?={int.MaxValue}";

    public HttpClient ApiClient { get; private set; } = null!;

    public WireMockServer PokeApiMockServer { get; private set; } = null!;

    public MySqlContainer MySqlContainer { get; private set; } = null!;

    public WebApplicationFactory<IApiMarker> Factory { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        // Database configuration        
        MySqlContainer = new MySqlBuilder()
            .WithDatabase("poketactics")
            .WithUsername("test")
            .WithPassword("test")
            .WithImage("mysql:8.0")
            .Build();

        await MySqlContainer.StartAsync();

        // External API mocks
        PokeApiMockServer = WireMockServer.Start();

        ConfigurePokeApiMockServerToGetEmptyAbilitiesSummary();
        // TODO: add empty calls for moves and pokemon

        // Real API configuration
        Factory = new WebApplicationFactory<IApiMarker>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:Default"] = MySqlContainer.GetConnectionString(),
                        ["PokeApi:BaseUrl"] = PokeApiMockServer.Url
                    });
                });
            });

        ApiClient = Factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        ApiClient.Dispose();
        Factory.Dispose();
        PokeApiMockServer.Stop();
        PokeApiMockServer.Dispose();
        await MySqlContainer.DisposeAsync();
    }

    public void ConfigurePokeApiMockServerToGetEmptyAbilitiesSummary()
    {
        AbilitySummaryListPokeApiResponse emptyResponse = new()
        {
            Count = 0,
            Results = []
        };

        PokeApiMockServer
            .Given(Request.Create().WithPath(pokeApiAbilityPath).UsingGet())
            .RespondWith(Response.Create().WithSuccess().WithBodyAsJson(emptyResponse));
    }

    public void ConfigurePokeApiMockServerForGet<TResponse>(string path, TResponse response) where TResponse : class
    {
        PokeApiMockServer
            .Given(Request.Create().WithPath(path).UsingGet())
            .RespondWith(Response.Create().WithSuccess().WithBodyAsJson(response));
    }

    public void ConfigurePokeApiMockServerToGetAbilitiesSummary(AbilitySummaryListPokeApiResponse response)
    {
        PokeApiMockServer
            .Given(Request.Create().WithPath(pokeApiAbilityPath).UsingGet())
            .RespondWith(Response.Create().WithSuccess().WithBodyAsJson(response));
    }

    public void ConfigurePokeApiMockServerToGetMovesSummary(MoveSummaryListPokeApiResponse response)
    {
        PokeApiMockServer
            .Given(Request.Create().WithPath($"/api/v2/move?={int.MaxValue}").UsingGet())
            .RespondWith(Response.Create().WithSuccess().WithBodyAsJson(response));
    }

    public void ConfigurePokeApiMockServerToGetPokemonSummaryList(PokemonSummaryListPokeApiResponse response)
    {
        PokeApiMockServer
            .Given(Request.Create().WithPath($"/api/v2/pokemon?={int.MaxValue}").UsingGet())
            .RespondWith(Response.Create().WithSuccess().WithBodyAsJson(response));
    }
}
