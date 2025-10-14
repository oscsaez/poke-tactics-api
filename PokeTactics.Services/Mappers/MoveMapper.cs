using PokeTactics.Contracts.Move.PokeApi;
using PokeTactics.Core.Entities;

namespace PokeTactics.Services.Mappers;

public static class MoveMapper
{
    // DTO -> Entity
    public static Move ToMove(this MoveInfoPokeApiResponse moveInfoPokeApiResponse, string moveName)
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

    // Entity -> Entity
    public static void MapExisting(this Move trackedMove, Move nonTrackedMove)
    {
        trackedMove.Name = nonTrackedMove.Name;
        trackedMove.Description = nonTrackedMove.Description;
        trackedMove.Accuracy = nonTrackedMove.Accuracy;
        trackedMove.Type = nonTrackedMove.Type;
        trackedMove.Power = nonTrackedMove.Power;
        trackedMove.PowerPoints = nonTrackedMove.PowerPoints;
    }
}
