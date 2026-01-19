using PokeTactics.Contracts.Pokemon.Responses;
using PokeTactics.Core.Definitions.Dtos;
using PokeTactics.Services.Facade;

namespace PokeTactics.Api.Endpoints;

public static class PokemonEndpoints
{
    public static RouteGroupBuilder MapPokemonEndpoints(this RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet("/", async ([AsParameters] KeysetPaginationRequest request, IPokemonService pokemonService) =>
        {
            KeysetPaginationResponse<PokemonDto> result = await pokemonService.Find(request);
            return Results.Ok(result);
        })
        .WithName("GetPokemon")
        .WithSummary("Returns a paginated list of pokemon")
        .WithDescription(@"Returns a list of pokemon using keyset pagination. 
            The pokemon are sorted by PokedexOrder and then by Id, but the ones with
            null or negative PokedexOrder are returned at the end.");

        return groupBuilder;
    }
}
