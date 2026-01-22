using PokeTactics.Contracts.Move.PokeApi;
using PokeTactics.Contracts.Move.Responses;
using PokeTactics.Core.Entities;

namespace PokeTactics.Services.Mappers;

public static class MoveInPokemonMapper
{
    // DTO -> Entity
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

    // Entity -> DTO
    public static MoveDto ToMoveDto(this MoveInPokemon moveInPokemon)
    {
        return new MoveDto
        {
            Accuracy = moveInPokemon.Move.Accuracy,
            Description = moveInPokemon.Move.Description,
            Name = moveInPokemon.Move.Name,
            Power = moveInPokemon.Move.Power,
            PowerPoints = moveInPokemon.Move.PowerPoints,
            Type = moveInPokemon.Move.Type
        };
    }

    public static ICollection<MoveDto> ToMoveDtos(this ICollection<MoveInPokemon> movesInPokemon) => [.. movesInPokemon.Select(ToMoveDto)];
}
