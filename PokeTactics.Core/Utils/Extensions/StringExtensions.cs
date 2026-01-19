namespace PokeTactics.Core.Utils.Extensions
{
    public static class StringExtensions
    {
        public static Uri ToUri(this string uriString)
        {
            return new Uri(uriString);
        }

        public static Uri ToUri(this string uriString, UriKind kind)
        {
            return new Uri(uriString, kind);
        }

        public static bool Compare(this ICollection<string> collection1, ICollection<string> collection2)
        {
            if (!collection1.Except(collection2).IsNullOrEmpty())
            {
                return false;
            }

            if (!collection2.Except(collection1).IsNullOrEmpty())
            {
                return false;
            }

            return true;
        }
    }
}