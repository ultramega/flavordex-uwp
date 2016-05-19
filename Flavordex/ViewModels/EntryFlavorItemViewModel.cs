using Flavordex.Models;
using Flavordex.UI;

namespace Flavordex.ViewModels
{
    /// <summary>
    /// Represents a Flavor in a RadarControl.
    /// </summary>
    public class EntryFlavorItemViewModel : ModelViewModel<EntryFlavor>, RadarItem
    {
        /// <summary>
        /// Gets the name of the Flavor.
        /// </summary>
        public string Name
        {
            get
            {
                return Model.Name;
            }
            set { }
        }

        /// <summary>
        /// Gets or sets the value of the Flavor.
        /// </summary>
        public long Value
        {
            get
            {
                return Model.Value;
            }
            set
            {
                Model.Value = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="flavor">The Flavor to represent.</param>
        public EntryFlavorItemViewModel(EntryFlavor flavor) : base(flavor) { }
    }
}
