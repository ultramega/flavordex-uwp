using Flavordex.Models;

namespace Flavordex.ViewModels
{
    /// <summary>
    /// Represents an Extra in a list.
    /// </summary>
    public class EntryExtraItemViewModel : ModelViewModel<EntryExtra>
    {
        /// <summary>
        /// Gets or sets the name of the Extra.
        /// </summary>
        public string Name
        {
            get
            {
                return Model.Name;
            }
        }

        /// <summary>
        /// Gets or sets the value of the Extra.
        /// </summary>
        public string Value
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
        /// <param name="extra">The Extra to represent.</param>
        public EntryExtraItemViewModel(EntryExtra extra) : base(extra) { }
    }
}
