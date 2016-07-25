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
using Windows.ApplicationModel.Resources;

namespace Flavordex.ViewModels
{
    /// <summary>
    /// Represents a Coffee Entry.
    /// </summary>
    public class CoffeeEntryViewModel : EntryViewModel
    {
        /// <summary>
        /// The index of the espresso brew method.
        /// </summary>
        private int _espressoIndex = 4;

        /// <summary>
        /// Gets or sets the list of brewing method options.
        /// </summary>
        public string[] BrewMethods { get; private set; } =
            ResourceLoader.GetForCurrentView("Coffee").GetString("BrewMethods").Split(';');

        /// <summary>
        /// Gets or sets the name of the roaster.
        /// </summary>
        public string Roaster
        {
            get
            {
                return GetExtra(Tables.Extras.Coffee.ROASTER);
            }
            set
            {
                SetExtra(Tables.Extras.Coffee.ROASTER, value);
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the roasting date.
        /// </summary>
        public string RoastDate
        {
            get
            {
                return GetExtra(Tables.Extras.Coffee.ROAST_DATE);
            }
            set
            {
                SetExtra(Tables.Extras.Coffee.ROAST_DATE, value);
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the coarseness of the grind.
        /// </summary>
        public string Grind
        {
            get
            {
                return GetExtra(Tables.Extras.Coffee.GRIND);
            }
            set
            {
                SetExtra(Tables.Extras.Coffee.GRIND, value);
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the brewing method.
        /// </summary>
        public int BrewMethod
        {
            get
            {
                var method = 0;
                int.TryParse(GetExtra(Tables.Extras.Coffee.BREW_METHOD), out method);
                return method;
            }
            set
            {
                value = Math.Max(0, Math.Min(BrewMethods.Length - 1, value));
                SetExtra(Tables.Extras.Coffee.BREW_METHOD, value);
                RaisePropertyChanged();
                RaisePropertyChanged("IsEspresso");
                RaisePropertyChanged("BrewMethodName");
                RaisePropertyChanged("CBR");
            }
        }

        /// <summary>
        /// Gets whether this is an espresso brew.
        /// </summary>
        public bool IsEspresso
        {
            get
            {
                return BrewMethod == _espressoIndex;
            }
        }

        /// <summary>
        /// Gets the name of the brewing method.
        /// </summary>
        public string BrewMethodName
        {
            get
            {
                return BrewMethods[BrewMethod];
            }
        }

        /// <summary>
        /// Gets or sets the dose of coffee.
        /// </summary>
        public string Dose
        {
            get
            {
                return GetExtra(Tables.Extras.Coffee.STATS_DOSE);
            }
            set
            {
                SetExtra(Tables.Extras.Coffee.STATS_DOSE, value);
                RaisePropertyChanged();
                RaisePropertyChanged("CBR");
            }
        }

        /// <summary>
        /// Gets or sets the mass of water or espresso.
        /// </summary>
        public string Mass
        {
            get
            {
                return GetExtra(Tables.Extras.Coffee.STATS_MASS);
            }
            set
            {
                SetExtra(Tables.Extras.Coffee.STATS_MASS, value);
                RaisePropertyChanged();
                RaisePropertyChanged("CBR");
            }
        }

        /// <summary>
        /// Gets the coffee brew ratio.
        /// </summary>
        public string CBR
        {
            get
            {
                var dose = 0D;
                double.TryParse(Dose, out dose);

                var mass = 0D;
                double.TryParse(Mass, out mass);

                if (dose > 0 && mass > 0)
                {
                    if (IsEspresso)
                    {
                        return string.Format("{0:0.#}%", dose / mass * 100);
                    }
                    else
                    {
                        return string.Format("{0:0.#}", mass / dose);
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets the brewing temperature.
        /// </summary>
        public string Temperature
        {
            get
            {
                return GetExtra(Tables.Extras.Coffee.STATS_TEMP);
            }
            set
            {
                SetExtra(Tables.Extras.Coffee.STATS_TEMP, value);
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the total extraction time in seconds.
        /// </summary>
        public int? ExtractionTime
        {
            get
            {
                var time = 0;
                if (int.TryParse(GetExtra(Tables.Extras.Coffee.STATS_EXTIME), out time))
                {
                    return time;
                }
                return null;
            }
            set
            {
                value = value != null && value > 0 ? value : null;
                SetExtra(Tables.Extras.Coffee.STATS_EXTIME, value);
                RaisePropertyChanged();
                RaisePropertyChanged("ExtractionTimeMinutes");
                RaisePropertyChanged("ExtractionTimeSeconds");
                RaisePropertyChanged("ExtractionTimeString");
            }
        }

        /// <summary>
        /// Gets or sets the minutes component of the extraction time.
        /// </summary>
        public string ExtractionTimeMinutes
        {
            get
            {
                if (ExtractionTime.HasValue)
                {
                    var minutes = ExtractionTime.Value / 60;
                    if (minutes > 0)
                    {
                        return minutes.ToString();
                    }
                }
                return null;
            }
            set
            {
                var minutes = 0;
                int.TryParse(value, out minutes);
                var currentTime = ExtractionTime.HasValue ? ExtractionTime.Value : 0;
                ExtractionTime = minutes * 60 + currentTime % 60;
            }
        }

        /// <summary>
        /// Gets or sets the seconds component of the extraction time.
        /// </summary>
        public string ExtractionTimeSeconds
        {
            get
            {
                if (ExtractionTime.HasValue)
                {
                    var seconds = ExtractionTime.Value % 60;
                    if (seconds > 0)
                    {
                        return seconds.ToString("D2");
                    }
                }
                return null;
            }
            set
            {
                var seconds = 0;
                int.TryParse(value, out seconds);
                var currentTime = ExtractionTime.HasValue ? ExtractionTime.Value : 0;
                ExtractionTime = currentTime - currentTime % 60 + seconds;
            }
        }

        /// <summary>
        /// Gets the extraction time as a string.
        /// </summary>
        public string ExtractionTimeString
        {
            get
            {
                if (ExtractionTime.HasValue)
                {
                    return ExtractionTimeMinutes + ":" + ExtractionTimeSeconds;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the strength of the brew in Total Dissolved Solids.
        /// </summary>
        public string TDS
        {
            get
            {
                return GetExtra(Tables.Extras.Coffee.STATS_TDS);
            }
            set
            {
                SetExtra(Tables.Extras.Coffee.STATS_TDS, value);
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the extraction yield.
        /// </summary>
        public string Yield
        {
            get
            {
                return GetExtra(Tables.Extras.Coffee.STATS_YIELD);
            }
            set
            {
                SetExtra(Tables.Extras.Coffee.STATS_YIELD, value);
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entry">The Entry to represent.</param>
        public CoffeeEntryViewModel(Entry entry) : base(entry) { }

        /// <summary>
        /// Adds the Any option to the list of brew methods.
        /// </summary>
        public override void EnableSearchMode()
        {
            base.EnableSearchMode();
            var brewMethods = new string[BrewMethods.Length + 1];
            brewMethods[0] = ResourceLoader.GetForCurrentView("Search").GetString("Any");
            BrewMethods.CopyTo(brewMethods, 1);
            BrewMethods = brewMethods;
            _espressoIndex = 5;
        }
    }
}
