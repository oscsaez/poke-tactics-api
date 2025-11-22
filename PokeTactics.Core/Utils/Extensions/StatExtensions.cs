using PokeTactics.Core.Entities;

namespace PokeTactics.Core.Utils.Extensions;

public static class StatExtensions
{
    public static bool Compare(this ICollection<Stat> stats1, ICollection<Stat> stats2)
    {
        if (stats1.Count != stats2.Count)
        {
            return false;
        }

        List<Stat> orderedStats1 = [.. stats1.OrderBy(x => x.Name)];
        List<Stat> orderedStats2 = [.. stats2.OrderBy(x => x.Name)];

        for (int i = 0; i < stats1.Count; i++)
        {
            if (!orderedStats1[i].Compare(orderedStats2[i]))
            {
                return false;
            }
        }

        return true;
    }

    public static bool Compare(this Stat stat1, Stat stat2)
    {
        return stat1.Name == stat2.Name &&
            stat2.Base == stat2.Base;
    }
}
