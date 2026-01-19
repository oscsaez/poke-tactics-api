using System.Net;
using PokeTactics.Api.Test.Fixture;
using PokeTactics.Api.Test.Utils;
using PokeTactics.Contracts.Pokemon.Responses;
using PokeTactics.Core.Definitions.Dtos;
using PokeTactics.Core.Utils.Extensions;

namespace PokeTactics.Api.Test.Endpoints;

[Collection(PokeTacticsCollection.Name)]
public class PokemonEndpointsTest : IAsyncLifetime
{
    private const string BaseUri = "/pokemon";

    private readonly string DefaultUriForGetPokemon = $"{BaseUri}?pageSize=1";

    private readonly PokeTacticsFixture _fixture;

    public PokemonEndpointsTest(PokeTacticsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetPokemon_ThereAreNoPokemon_ReturnsKeysetPaginationResponseWithoutItemsAndNextLastIdNull()
    {
        // Act
        ApiResponse<KeysetPaginationResponse<ICollection<PokemonDto>>> response = 
            await _fixture.GetAsync<KeysetPaginationResponse<ICollection<PokemonDto>>>(DefaultUriForGetPokemon);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Body);
        Assert.Empty(response.Body.Items);
        Assert.Null(response.Body.NextLastId);
    }

    [Fact]
    public async Task GetPokemonSimple_ThereAreNoPokemon_ReturnsKeysetPaginationResponseWithoutItemsAndNextLastIdNull()
    {
        // Arrange
        string uri = $"{BaseUri}/simple";

        // Act
        ApiResponse<KeysetPaginationResponse<ICollection<PokemonDto>>> response = 
            await _fixture.GetAsync<KeysetPaginationResponse<ICollection<PokemonDto>>>(DefaultUriForGetPokemon);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Body);
        Assert.Empty(response.Body.Items);
        Assert.Null(response.Body.NextLastId);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _fixture.DeleteAll();
    }
}
