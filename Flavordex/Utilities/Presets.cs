using System.Collections.Generic;
using Windows.ApplicationModel.Resources;

namespace Flavordex.Utilities
{
    /// <summary>
    /// Utilities for handling preset categories.
    /// </summary>
    public class Presets
    {
        /// <summary>
        /// A reference to the string ResourceLoader.
        /// </summary>
        private static readonly ResourceLoader _resources = ResourceLoader.GetForCurrentView();

        /// <summary>
        /// A Dictionary mapping internal category names to their display names.
        /// </summary>
        private static Dictionary<string, string> _categories = new Dictionary<string, string>()
        {
            { Constants.CAT_BEER, _resources.GetString("Category/Beer") },
            { Constants.CAT_COFFEE, _resources.GetString("Category/Coffee") },
            { Constants.CAT_WHISKEY, _resources.GetString("Category/Whiskey") },
            { Constants.CAT_WINE, _resources.GetString("Category/Wine") }
        };

        /// <summary>
        /// Gets the display name of a category.
        /// </summary>
        /// <param name="internalName">The internal category name.</param>
        /// <returns>The real display name of the category.</returns>
        public static string GetRealCategoryName(string internalName)
        {
            if (internalName != null && _categories.ContainsKey(internalName))
            {
                return _categories[internalName];
            }
            return internalName;
        }
    }
}
