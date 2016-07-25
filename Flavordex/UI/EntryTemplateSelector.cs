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
using Flavordex.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Flavordex.UI
{
    /// <summary>
    /// Template selector for selecting a specialized layout for entries in preset categories.
    /// </summary>
    public class EntryTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the template for non-specific categories.
        /// </summary>
        public DataTemplate GenericTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for the Beer category.
        /// </summary>
        public DataTemplate BeerTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for the Coffee category.
        /// </summary>
        public DataTemplate CoffeeTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for the Whiskey category.
        /// </summary>
        public DataTemplate WhiskeyTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for the Wine category.
        /// </summary>
        public DataTemplate WineTemplate { get; set; }

        /// <summary>
        /// Selects the appropriate template based on the ViewModel type.
        /// </summary>
        /// <param name="item">The ViewModel.</param>
        /// <param name="container">The ContentControl.</param>
        /// <returns>The DataTemplate to use.</returns>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is BeerEntryViewModel)
            {
                return BeerTemplate;
            }
            else if (item is CoffeeEntryViewModel)
            {
                return CoffeeTemplate;
            }
            else if (item is WhiskeyEntryViewModel)
            {
                return WhiskeyTemplate;
            }
            else if (item is WineEntryViewModel)
            {
                return WineTemplate;
            }
            else if (item is EntryViewModel)
            {
                return GenericTemplate;
            }

            return base.SelectTemplateCore(item, container);
        }
    }
}
