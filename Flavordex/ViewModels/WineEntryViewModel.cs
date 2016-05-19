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
        private static string[] _wineVarietals = ResourceLoader.GetForCurrentView("Wine").GetString("Varietals").Split(';');

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
                return _wineVarietals.Where(e => e.ToLower().Contains(Varietal.ToLower())).ToArray();
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
