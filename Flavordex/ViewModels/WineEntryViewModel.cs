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
using Flavordex.Models;
using Flavordex.Models.Data;
using System.Linq;
using Windows.ApplicationModel.Resources;

namespace Flavordex.ViewModels
{
    /// <summary>
    /// Represents a Wine Entry.
    /// </summary>
    public class WineEntryViewModel : EntryViewModel
    {
        /// <summary>
        /// The list of wine varietals.
        /// </summary>
        private static string[] _wineVarietals =
            ResourceLoader.GetForCurrentView("Wine").GetString("Varietals").Split(';');

        /// <summary>
        /// Gets the list of wine varietal suggestions.
        /// </summary>
        public string[] WineVarietalSuggestions
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Varietal))
                {
                    return null;
                }
                return _wineVarietals.Where(e => e.ToLower().Contains(Varietal.ToLower()))
                    .ToArray();
            }
        }

        /// <summary>
        /// Gets or sets the varietal of the wine.
        /// </summary>
        public string Varietal
        {
            get
            {
                return GetExtra(Tables.Extras.Wine.VARIETAL);
            }
            set
            {
                SetExtra(Tables.Extras.Wine.VARIETAL, value);
                RaisePropertyChanged();
                RaisePropertyChanged("WineVarietalSuggestions");
            }
        }

        /// <summary>
        /// Gets or sets the vintage year.
        /// </summary>
        public string Vintage
        {
            get
            {
                return GetExtra(Tables.Extras.Wine.STATS_VINTAGE);
            }
            set
            {
                SetExtra(Tables.Extras.Wine.STATS_VINTAGE, value);
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the alcohol by volume.
        /// </summary>
        public string ABV
        {
            get
            {
                return GetExtra(Tables.Extras.Wine.STATS_ABV);
            }
            set
            {
                SetExtra(Tables.Extras.Wine.STATS_ABV, value);
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entry">The Entry to represent.</param>
        public WineEntryViewModel(Entry entry) : base(entry) { }
    }
}
