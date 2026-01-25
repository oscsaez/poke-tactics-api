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

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetPokemon_ThereAreNoPokemon_ReturnsKeysetPaginationResponseWithoutItemsAndNextLastIdNull(bool isSimpleApi)
    {
        // Arrange
        string uri = isSimpleApi ? DefaultUriForGetPokemonSimple : DefaultUriForGetPokemon;

        // Act
        ApiResponse<KeysetPaginationResponse<PokemonDto>> response = await _fixture.GetAsync<KeysetPaginationResponse<PokemonDto>>(uri);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Body);
        Assert.Empty(response.Body.Items);
        Assert.Null(response.Body.NextLastId);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetPokemon_ReturnsKeysetPaginationResponseWithItemsAndNextLastIdFilled(bool isSimpleApi)
    {
        // Arrange
        await _fixture.SetupSyncSinglePokemon();

        string uri = isSimpleApi ? DefaultUriForGetPokemonSimple : DefaultUriForGetPokemon;

        // Act
        ApiResponse<KeysetPaginationResponse<PokemonDto>> response = await _fixture.GetAsync<KeysetPaginationResponse<PokemonDto>>(uri);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Body);
        PokemonDto pokemon = Assert.Single(response.Body.Items);

        await VerifyPokemonDto(isSimpleApi, pokemon);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetPokemon_RequestWithNextLastPokedexOrderAndNextLastId_ReturnsPokemonInTheRightOrder(bool isSimpleApi)
    {
        // Arrange
        const int NumberOfPokemon = 3;

        await _fixture.SetupSyncPokemon(NumberOfPokemon);

        string defaultUri = isSimpleApi ? DefaultUriForGetPokemonSimple : DefaultUriForGetPokemon;
        string uri = isSimpleApi ? DefaultUriForGetPokemonSimple : DefaultUriForGetPokemon;
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
                await VerifyPokemonDto(isSimpleApi, currentPokemon);
            }
            else
            {
                await VerifyPokemonDto(isSimpleApi, currentPokemon, previousPokemon);
            }

            previousPokemon = currentPokemon;

            uri =
                $"{defaultUri}" +
                $"&LastPokedexOrder={response.Body.NextLastPokedexOrder}" +
                $"&LastId={response.Body.NextLastId}";
        }
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

    private async Task VerifyPokemonDto(bool isSimpleApi, PokemonDto pokemon)
    {
        if(isSimpleApi)
        {
            await _fixture.VerifySimplePokemonDto(pokemon);
        }
        else
        {
            await _fixture.VerifyPokemonDto(pokemon);
        }
    }

    private async Task VerifyPokemonDto(bool isSimpleApi, PokemonDto currentPokemon, PokemonDto previousPokemon)
    {
        if(isSimpleApi)
        {
            await _fixture.VerifySimplePokemonDto(currentPokemon, previousPokemon);
        }
        else
        {
            await _fixture.VerifyPokemonDto(currentPokemon, previousPokemon);
        }
    }
}
