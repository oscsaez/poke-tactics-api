using PokeTactics.Contracts.Move.PokeApi;
using PokeTactics.Core.Entities;

namespace PokeTactics.Services.Mappers;

public static class MovesInPokemonMapper
{
    public static MovesInPokemon MovePokeApiResponseToMovesInPokemon(string moveName, MoveInfoPokeApiResponse moveInfoPokeApiResponse)
    {
        return new MovesInPokemon
        {
            Move = moveInfoPokeApiResponse.ToMove(moveName)
        };
    }
}
