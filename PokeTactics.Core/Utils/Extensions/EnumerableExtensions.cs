using System.Collections;

namespace PokeTactics.Core.Utils.Extensions;

public static class EnumerableExtensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) where T : class
    {
        if (enumerable is null || !enumerable.Any())
        {
            return true;
        }

        return false;
    }
}
