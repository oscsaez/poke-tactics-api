namespace PokeTactics.Core.Utils.Extensions
{
    public static class StringExtensions
    {
        public static Uri ToUri(this string uriString)
        {
            return new Uri(uriString);
        }
    }
}