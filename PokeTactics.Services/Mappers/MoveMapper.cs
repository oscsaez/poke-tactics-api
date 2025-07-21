using PokeTactics.Contracts.Move.PokeApi;
using PokeTactics.Core.Entities;

namespace PokeTactics.Services.Mappers;

public static class MoveMapper
{
    public static Move MovePokeApiResponseToMove(string moveName, MoveInfoPokeApiResponse moveInfoPokeApiResponse)
    {
        return new Move
        {
            Accuracy = moveInfoPokeApiResponse.Accuracy,
            Description = moveInfoPokeApiResponse.EffectEntries.TakeEnglishToString(),
            Name = moveName,
            Power = moveInfoPokeApiResponse.Power,
            PowerPoints = moveInfoPokeApiResponse.Pp,
            Type = moveInfoPokeApiResponse.Type.Name,
        };
    }
}
