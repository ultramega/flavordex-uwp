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
using System;
using System.Linq;
using Windows.ApplicationModel.Resources;

namespace Flavordex.ViewModels
{
    /// <summary>
    /// Represents a Beer Entry.
    /// </summary>
    public class BeerEntryViewModel : EntryViewModel
    {
        /// <summary>
        /// The list of beer styles.
        /// </summary>
        private static string[] _beerStyles =
            ResourceLoader.GetForCurrentView("Beer").GetString("Styles").Split(';');

        /// <summary>
        /// Gets the list of beer style suggestions.
        /// </summary>
        public string[] BeerStyleSuggestions
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Style))
                {
                    return null;
                }
                return _beerStyles.Where(e => e.ToLower().Contains(Style.ToLower())).ToArray();
            }
        }

        /// <summary>
        /// Gets or sets the list of beer serving type options.
        /// </summary>
        public string[] ServingTypes { get; private set; } =
            ResourceLoader.GetForCurrentView("Beer").GetString("ServingTypes").Split(';');

        /// <summary>
        /// Gets or sets the style of beer.
        /// </summary>
        public string Style
        {
            get
            {
                return GetExtra(Tables.Extras.Beer.STYLE);
            }
            set
            {
                SetExtra(Tables.Extras.Beer.STYLE, value);
                RaisePropertyChanged();
                RaisePropertyChanged("BeerStyleSuggestions");
            }
        }

        /// <summary>
        /// Gets or sets the serving type.
        /// </summary>
        public int ServingType
        {
            get
            {
                var type = 0;
                int.TryParse(GetExtra(Tables.Extras.Beer.SERVING), out type);
                return type;
            }
            set
            {
                value = Math.Max(0, Math.Min(ServingTypes.Length - 1, value));
                SetExtra(Tables.Extras.Beer.SERVING, value);
                RaisePropertyChanged();
                RaisePropertyChanged("ServingTypeName");
            }
        }

        /// <summary>
        /// Gets the name of the serving type.
        /// </summary>
        public string ServingTypeName
        {
            get
            {
                return ServingTypes[ServingType];
            }
        }

        /// <summary>
        /// Gets or sets the bitterness in International Bittering Units.
        /// </summary>
        public string IBU
        {
            get
            {
                return GetExtra(Tables.Extras.Beer.STATS_IBU);
            }
            set
            {
                SetExtra(Tables.Extras.Beer.STATS_IBU, value);
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
                return GetExtra(Tables.Extras.Beer.STATS_ABV);
            }
            set
            {
                SetExtra(Tables.Extras.Beer.STATS_ABV, value);
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the original gravity.
        /// </summary>
        public string OG
        {
            get
            {
                return GetExtra(Tables.Extras.Beer.STATS_OG);
            }
            set
            {
                SetExtra(Tables.Extras.Beer.STATS_OG, value);
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the final gravity.
        /// </summary>
        public string FG
        {
            get
            {
                return GetExtra(Tables.Extras.Beer.STATS_FG);
            }
            set
            {
                SetExtra(Tables.Extras.Beer.STATS_FG, value);
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entry">The Entry to represent.</param>
        public BeerEntryViewModel(Entry entry) : base(entry) { }

        /// <summary>
        /// Adds the Any option to the list of serving types.
        /// </summary>
        public override void EnableSearchMode()
        {
            base.EnableSearchMode();
            var servingTypes = new string[ServingTypes.Length + 1];
            servingTypes[0] = ResourceLoader.GetForCurrentView("Search").GetString("Any");
            ServingTypes.CopyTo(servingTypes, 1);
            ServingTypes = servingTypes;
        }
    }
}
