namespace PokeTactics.Core.Utils.Extensions;

public static class CollectionExtensions
{
    public static void AddRange<T>(this ICollection<T> targetCollection, ICollection<T> collectionToAdd) where T : class
    {
        foreach (T item in collectionToAdd)
        {
            targetCollection.Add(item);
        }
    }
}
