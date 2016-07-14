using Flavordex.Models;
using Flavordex.Models.Data;
using System.Linq;
using Windows.ApplicationModel.Resources;

namespace Flavordex.ViewModels
{
    /// <summary>
    /// Represents a Whiskey Entry.
    /// </summary>
    public class WhiskeyEntryViewModel : EntryViewModel
    {
        /// <summary>
        /// The list of whiskey types.
        /// </summary>
        private static string[] _whiskeyTypes =
            ResourceLoader.GetForCurrentView("Whiskey").GetString("Types").Split(';');

        /// <summary>
        /// Gets the list of whiskey type suggestions.
        /// </summary>
        public string[] WhiskeyTypeSuggestions
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Type))
                {
                    return null;
                }
                return _whiskeyTypes.Where(e => e.ToLower().Contains(Type.ToLower())).ToArray();
            }
        }

        /// <summary>
        /// Gets or sets the type of whiskey.
        /// </summary>
        public string Type
        {
            get
            {
                return GetExtra(Tables.Extras.Whiskey.STYLE);
            }
            set
            {
                SetExtra(Tables.Extras.Whiskey.STYLE, value);
                RaisePropertyChanged();
                RaisePropertyChanged("WhiskeyTypeSuggestions");
            }
        }

        /// <summary>
        /// Gets or sets the age of the whiskey.
        /// </summary>
        public string Age
        {
            get
            {
                return GetExtra(Tables.Extras.Whiskey.STATS_AGE);
            }
            set
            {
                SetExtra(Tables.Extras.Whiskey.STATS_AGE, value);
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
                return GetExtra(Tables.Extras.Whiskey.STATS_ABV);
            }
            set
            {
                SetExtra(Tables.Extras.Whiskey.STATS_ABV, value);
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entry">The Entry to represent.</param>
        public WhiskeyEntryViewModel(Entry entry) : base(entry) { }
    }
}
