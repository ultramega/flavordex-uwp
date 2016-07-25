/*
  The MIT License (MIT)
  Copyright © 2016 Steve Guidetti

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the “Software”), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in
  all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
  THE SOFTWARE.
*/
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
