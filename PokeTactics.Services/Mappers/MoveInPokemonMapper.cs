using PokeTactics.Contracts.Move.PokeApi;
using PokeTactics.Core.Entities;

namespace PokeTactics.Services.Mappers;

public static class MoveInPokemonMapper
{
    public static MoveInPokemon ToMoveInPokemon(this MovePokeApiResponse movePokeApiResponse)
    {
        return new MoveInPokemon
        {
            Move = movePokeApiResponse.ToMove()
        };
    }

    public static ICollection<MoveInPokemon> ToMovesInPokemon(this ICollection<MovePokeApiResponse> movePokeApiResponses)
    {
        return [.. movePokeApiResponses.Select(ToMoveInPokemon)];
    }
}
