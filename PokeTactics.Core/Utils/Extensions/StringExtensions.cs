namespace PokeTactics.Core.Utils.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns absolute path passed as string as Uri
        /// <remarks>
        /// Only absolute paths should be passed. It uses <see cref="UriKind.Absolute"> when creating the Uri
        /// </remarks>
        /// </summary>
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
            if (collection1.Count != collection2.Count)
            {
                return false;
            }

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