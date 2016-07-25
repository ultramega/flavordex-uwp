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
