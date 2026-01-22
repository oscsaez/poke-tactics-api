using PokeTactics.Contracts.Move.PokeApi;
using PokeTactics.Contracts.Type.PokeApi;

namespace PokeTactics.Api.Test.Utils.Builders;

public static class MoveBuilder
{
    public static MoveSummaryListPokeApiResponse BuildMoveSummaryListPokeApiResponse()
    {
        return new MoveSummaryListPokeApiResponse
        {
            Count = 1,
            Results = [
                new MoveSummaryPokeApiResponse
                {
                    Name = TestGenerator.RandomName(),
                    Url = TestGenerator.RandomPath()
                }
            ]
        };
    }

    public static MoveInfoPokeApiResponse BuildMoveInfoPokeApiResponse()
    {
        return new MoveInfoPokeApiResponse
        {
            Accuracy = TestGenerator.RandomInt(),
            Power = TestGenerator.RandomInt(),
            Pp = TestGenerator.RandomInt(),
            Type = new TypeInfoPokeApiResponse
            {
                Name = TestGenerator.RandomName()
            },
            EffectEntries = [EffectEntryBuilder.BuildRandomEffectEntry()]
        };
    }
}
