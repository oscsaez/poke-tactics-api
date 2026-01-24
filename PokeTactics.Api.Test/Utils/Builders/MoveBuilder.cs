using PokeTactics.Contracts.Move.PokeApi;
using PokeTactics.Contracts.Type.PokeApi;

namespace PokeTactics.Api.Test.Utils.Builders;

public static class MoveBuilder
{
    public static MoveSummaryListPokeApiResponse BuildMoveSummaryListPokeApiResponse()
    {
        const int One = 1;

        return BuildMoveSummaryListPokeApiResponse(One);
    }

    public static MoveSummaryListPokeApiResponse BuildMoveSummaryListPokeApiResponse(int numberOfMoves)
    {
        List<MoveSummaryPokeApiResponse> moveSummaries = new(numberOfMoves);

        for (int i = 0; i < numberOfMoves; i++)
        {
            moveSummaries.Add(new MoveSummaryPokeApiResponse
            {
                Name = TestGenerator.RandomName(),
                Url = TestGenerator.RandomPath()
            });
        }

        return new MoveSummaryListPokeApiResponse
        {
            Count = numberOfMoves,
            Results = moveSummaries
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
