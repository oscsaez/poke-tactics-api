using PokeTactics.Contracts.Constants;
using PokeTactics.Contracts.Move.PokeApi;
using PokeTactics.Contracts.Utils.Extensions;
using PokeTactics.Core.Entities;

namespace PokeTactics.Services.Mappers;

public static class MoveMapper
{
    private const string TempDefaultMoveType = "TempDefaultType";

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

    // TODO: Find a better way to map this not using a default value. This can cause errors
    public static Move ToMove(this MovePokeApiResponse movePokeApiResponse)
    {
        return new Move
        {
            Name = movePokeApiResponse.MoveUriPokeApiResponse.Name,
            Type = TempDefaultMoveType
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
