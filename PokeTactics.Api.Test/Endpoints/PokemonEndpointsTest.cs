using System.Net;
using PokeTactics.Api.Test.Fixture;
using PokeTactics.Api.Test.Utils;
using PokeTactics.Api.Test.Utils.Helpers;
using PokeTactics.Contracts.Common.Requests;
using PokeTactics.Contracts.Common.Responses;
using PokeTactics.Contracts.Pokemon.Responses;

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

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _fixture.DeleteAll();
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
        await _fixture.SetupSyncSinglePokemon();

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
    public async Task GetPokemon_RequestWithNextLastPokedexOrderAndNextLastId_ReturnsPokemonInTheRightOrder()
    {
        // Arrange
        const int NumberOfPokemon = 3;

        await _fixture.SetupSyncPokemon(NumberOfPokemon);

        ApiResponse<KeysetPaginationResponse<PokemonDto>> response1 = 
            await _fixture.GetAsync<KeysetPaginationResponse<PokemonDto>>(DefaultUriForGetPokemon);

        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        Assert.NotNull(response1.Body);
        PokemonDto pokemon1 = Assert.Single(response1.Body.Items);

        await _fixture.VerifyPokemonDto(pokemon1);

        string uri2 = $"{DefaultUriForGetPokemon}&LastPokedexOrder={response1.Body.NextLastPokedexOrder}&LastId={response1.Body.NextLastId}";

        // Act & Assert
        ApiResponse<KeysetPaginationResponse<PokemonDto>> response2 = await _fixture.GetAsync<KeysetPaginationResponse<PokemonDto>>(uri2);

        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        Assert.NotNull(response2.Body);
        PokemonDto pokemon2 = Assert.Single(response2.Body.Items);

        await _fixture.VerifyPokemonDto(pokemon2, pokemon1);

        string uri3 = $"{DefaultUriForGetPokemon}&LastPokedexOrder={response2.Body.NextLastPokedexOrder}&LastId={response2.Body.NextLastId}";

        ApiResponse<KeysetPaginationResponse<PokemonDto>> response3 = await _fixture.GetAsync<KeysetPaginationResponse<PokemonDto>>(uri3);

        Assert.Equal(HttpStatusCode.OK, response3.StatusCode);
        Assert.NotNull(response3.Body);
        PokemonDto pokemon3 = Assert.Single(response3.Body.Items);

        await _fixture.VerifyPokemonDto(pokemon3, pokemon2);
    }

    [Theory]
    [InlineData(false, null, 1)]
    [InlineData(false, 1, null)]
    [InlineData(true, null, 1)]
    [InlineData(true, 1, null)]
    public async Task GetPokemon_RequestWithoutOneCursor_ReturnsBadRequest(bool isSimpleApi, int? nextLastPokedexOrder, int? nextLastId)
    {
        // Arrange
        string expectedErrorMessage = $"Both {nameof(KeysetPaginationRequest.LastPokedexOrder)} and {nameof(KeysetPaginationRequest.LastId)} " +
                "or no one must be provided";

        string defaultUri = isSimpleApi ? DefaultUriForGetPokemonSimple : DefaultUriForGetPokemon;
        string uri = $"{defaultUri}";

        if (nextLastPokedexOrder.HasValue)
        {
            uri += $"&LastPokedexOrder={nextLastPokedexOrder}";
        }

        if (nextLastId.HasValue)
        {
            uri += $"&LastId={nextLastId}";
        }

        // Act
        ApiResponse<ErrorResponse> response = await _fixture.GetAsync<ErrorResponse>(uri);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(response.Body);
        Assert.Equal(expectedErrorMessage, response.Body.ErrorMessage);
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
        await _fixture.SetupSyncSinglePokemon();

        // Act
        ApiResponse<KeysetPaginationResponse<PokemonDto>> response = await _fixture.GetAsync<KeysetPaginationResponse<PokemonDto>>(DefaultUriForGetPokemonSimple);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Body);
        PokemonDto pokemon = Assert.Single(response.Body.Items);
        
        await _fixture.VerifySimplePokemonDto(pokemon);
    }

    [Fact]
    public async Task GetPokemonSimple_RequestWithCursor_ReturnsPokemonInTheRightOrder()
    {
        // Arrange
        const int NumberOfPokemon = 3;

        await _fixture.SetupSyncPokemon(NumberOfPokemon);

        string? uri = DefaultUriForGetPokemonSimple;
        PokemonDto? previousPokemon = null;

        // Act & Assert
        for (int i = 0; i < NumberOfPokemon; i++)
        {
            ApiResponse<KeysetPaginationResponse<PokemonDto>> response =
                await _fixture.GetAsync<KeysetPaginationResponse<PokemonDto>>(uri);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Body);

            PokemonDto currentPokemon = Assert.Single(response.Body.Items);

            if (previousPokemon is null)
            {
                await _fixture.VerifySimplePokemonDto(currentPokemon);
            }
            else
            {
                await _fixture.VerifySimplePokemonDto(currentPokemon, previousPokemon);
            }

            previousPokemon = currentPokemon;

            uri =
                $"{DefaultUriForGetPokemonSimple}" +
                $"&LastPokedexOrder={response.Body.NextLastPokedexOrder}" +
                $"&LastId={response.Body.NextLastId}";
        }
    }
}
