using Flavordex.Models;
using Flavordex.UI;

namespace Flavordex.ViewModels
{
    /// <summary>
    /// Represents a Flavor in a RadarControl.
    /// </summary>
    public class FlavorItemViewModel : ModelViewModel<Flavor>, RadarItem
    {
        /// <summary>
        /// Gets or sets the name of the Flavor.
        /// </summary>
        public string Name
        {
            get
            {
                return Model.Name;
            }
            set
            {
                Model.Name = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the value of the Flavor.
        /// </summary>
        public long Value
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// Whether the Flavor has been deleted.
        /// </summary>
        private bool _isDeleted;

        /// <summary>
        /// Gets or sets whether the Flavor has been deleted.
        /// </summary>
        public bool IsDeleted
        {
            get
            {
                return _isDeleted;
            }
            set
            {
                _isDeleted = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="flavor">The Flavor to represent.</param>
        public FlavorItemViewModel(Flavor flavor) : base(flavor) { }
    }
}
