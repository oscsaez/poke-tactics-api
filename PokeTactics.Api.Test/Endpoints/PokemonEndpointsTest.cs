using System.Net;
using PokeTactics.Api.Test.Fixture;
using PokeTactics.Api.Test.Utils;
using PokeTactics.Api.Test.Utils.Helpers;
using PokeTactics.Contracts.Pokemon.Responses;
using PokeTactics.Core.Definitions.Dtos;

namespace PokeTactics.Api.Test.Endpoints;

[Collection(PokeTacticsCollection.Name)]
public class PokemonEndpointsTest : IAsyncLifetime
{
    private const string BaseUri = "/pokemon";
    private const string QueryParameterPageSize = "pageSize=1";

    private readonly string DefaultUriForGetPokemon = $"{BaseUri}?{QueryParameterPageSize}";
    private readonly string DefaultUriForGetPokemonSimple = $"{BaseUri}/simple?{QueryParameterPageSize}";

    private readonly PokeTacticsFixture _fixture;

    public PokemonEndpointsTest(PokeTacticsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetPokemon_ThereAreNoPokemon_ReturnsKeysetPaginationResponseWithoutItemsAndNextLastIdNull()
    {
        // Act
        ApiResponse<KeysetPaginationResponse<PokemonDto>> response = 
            await _fixture.GetAsync<KeysetPaginationResponse<PokemonDto>>(DefaultUriForGetPokemon);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Body);
        Assert.Empty(response.Body.Items);
        Assert.Null(response.Body.NextLastId);
    }

    [Fact]
    public async Task GetPokemon_ReturnsKeysetPaginationResponseWithItemsAndNextLastIdFilled()
    {
        // Arrange
        await _fixture.SetupSyncPokemon();

        // Act
        ApiResponse<KeysetPaginationResponse<PokemonDto>> response = 
            await _fixture.GetAsync<KeysetPaginationResponse<PokemonDto>>(DefaultUriForGetPokemon);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Body);
        PokemonDto pokemon = Assert.Single(response.Body.Items);

        await _fixture.VerifyPokemonDto(pokemon);
    }

    [Fact]
    public async Task GetPokemonSimple_ThereAreNoPokemon_ReturnsKeysetPaginationResponseWithoutItemsAndNextLastIdNull()
    {
        // Act
        ApiResponse<KeysetPaginationResponse<PokemonDto>> response = await _fixture.GetAsync<KeysetPaginationResponse<PokemonDto>>(DefaultUriForGetPokemonSimple);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Body);
        Assert.Empty(response.Body.Items);
        Assert.Null(response.Body.NextLastId);
    }

    [Fact]
    public async Task GetPokemonSimple_ReturnsKeysetPaginationResponseWithItemsAndNextLastIdFilled()
    {
        // Arrange
        await _fixture.SetupSyncPokemon();

        // Act
        ApiResponse<KeysetPaginationResponse<PokemonDto>> response = await _fixture.GetAsync<KeysetPaginationResponse<PokemonDto>>(DefaultUriForGetPokemonSimple);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Body);
        PokemonDto pokemon = Assert.Single(response.Body.Items);
        
        await _fixture.VerifySimplePokemonDto(pokemon);
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
