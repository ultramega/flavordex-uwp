using Windows.ApplicationModel.Resources;

namespace Flavordex.Utilities
{
    /// <summary>
    /// Utility for pluralizing strings.
    /// </summary>
    public class Plurals
    {
        /// <summary>
        /// A reference to the plural string resources.
        /// </summary>
        private static readonly ResourceLoader _resources =
            ResourceLoader.GetForCurrentView("Plurals");

        /// <summary>
        /// Get the appropriate form of the word based on the count.
        /// </summary>
        /// <param name="pluralWord">The plural form of the word.</param>
        /// <param name="count">The number of items to identify.</param>
        /// <returns>The appropriate form of the word.</returns>
        public static string GetWord(string pluralWord, long count)
        {
            pluralWord = pluralWord.Substring(0, 1).ToUpper() + pluralWord.Substring(1).ToLower();
            var key = count == 1 ? "One" : "Other";
            return _resources.GetString(pluralWord + "/" + key);
        }
    }
}
