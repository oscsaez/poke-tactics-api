using PokeTactics.Core.Entities;

namespace PokeTactics.Core.Utils.Extensions;

public static class MoveExtensions
{
    public static bool Compare(this Move move1, Move move2)
    {
        return move1.CompareNameAndDescription(move2) &&
            move1.ComparePower(move2) &&
            move1.Type == move2.Type;
    }

    private static bool CompareNameAndDescription(this Move move1, Move move2)
    {
        return move1.Name == move2.Name &&
            move1.Description == move2.Description;
    }

    private static bool ComparePower(this Move move1, Move move2)
    {
        return move1.Power == move2.Power &&
            move1.PowerPoints == move2.PowerPoints;
    }
}
