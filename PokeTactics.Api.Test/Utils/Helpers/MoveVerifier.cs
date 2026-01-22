using PokeTactics.Api.Test.Fixture;
using PokeTactics.Contracts.Move.PokeApi;
using PokeTactics.Contracts.Move.Responses;
using PokeTactics.Core.Entities;
using PokeTactics.Core.Interfaces;

namespace PokeTactics.Api.Test.Utils.Helpers;

public static class MoveVerifier
{
    public static async Task VerifyMoves(this PokeTacticsFixture fixture, IDictionary<string, MoveInfoPokeApiResponse> moveInfosByNameMap)
    {
        IUnitOfWork unitOfWork = fixture.GetService<IUnitOfWork>();
        IEnumerable<Move> allMoves = await unitOfWork.MoveDao.LoadAllAsync();

        foreach (var moveInfoByName in moveInfosByNameMap)
        {
            Move move = Assert.Single(allMoves, x => x.Name == moveInfoByName.Key);
            Assert.Equal(moveInfoByName.Value.Accuracy, move.Accuracy);
            Assert.Equal(moveInfoByName.Value.Power, move.Power);
            Assert.Equal(moveInfoByName.Value.Pp, move.PowerPoints);
            Assert.Equal(moveInfoByName.Value.Type.Name, move.Type);
            Assert.Equal(moveInfoByName.Value.EffectEntries.Single().Effect, move.Description);
        }
    }

    public static void VerifyMoveDtos(ICollection<Move> expectedMoves, ICollection<MoveDto> actualMoves)
    {
        Assert.Equal(expectedMoves.Count, actualMoves.Count);
        int numberOfMoves = actualMoves.Count;

        for (int i = 0; i < numberOfMoves; i++)
        {
            VerifyMoveDto(expectedMoves.ElementAt(i), actualMoves.ElementAt(i));
        }
    }

    private static void VerifyMoveDto(Move expectedMove, MoveDto actualMove)
    {
        Assert.Equal(expectedMove.Accuracy, actualMove.Accuracy);
        Assert.Equal(expectedMove.Description, actualMove.Description);
        Assert.Equal(expectedMove.Name, actualMove.Name);
        Assert.Equal(expectedMove.Power, actualMove.Power);
        Assert.Equal(expectedMove.PowerPoints, actualMove.PowerPoints);
        Assert.Equal(expectedMove.Type, actualMove.Type);
    }
}
