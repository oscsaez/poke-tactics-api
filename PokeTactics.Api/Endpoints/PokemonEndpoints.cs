using PokeTactics.Core.Definitions.Dtos;
using PokeTactics.Core.Entities;
using PokeTactics.Services.Facade;

namespace PokeTactics.Api.Endpoints;

public static class PokemonEndpoints
{
    public static RouteGroupBuilder MapPokemonEndpoints(this RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet("/", async ([AsParameters] KeysetPaginationRequest request, IPokemonService pokemonService) =>
        {
            KeysetPaginationResponse<Pokemon> result = await pokemonService.Find(request);
            return Results.Ok(result);
        }).WithName("GetPokemon")
          .WithOpenApi();

        return groupBuilder;
    }
}
