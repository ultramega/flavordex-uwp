using Flavordex.Models;

namespace Flavordex.ViewModels
{
    /// <summary>
    /// Represents an Extra in a list.
    /// </summary>
    public class ExtraItemViewModel : ModelViewModel<Extra>
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
            set
            {
                Model.Name = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets whether the Extra has been deleted.
        /// </summary>
        public bool IsDeleted
        {
            get
            {
                return Model.IsDeleted;
            }
            set
            {
                Model.IsDeleted = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="extra">The Extra to represent.</param>
        public ExtraItemViewModel(Extra extra) : base(extra) { }
    }
}
