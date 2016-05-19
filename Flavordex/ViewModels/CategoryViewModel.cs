using Flavordex.Models;
using Flavordex.Utilities;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Flavordex.ViewModels
{
    /// <summary>
    /// Represents a Category.
    /// </summary>
    public class CategoryViewModel : ModelViewModel<Category>
    {
        /// <summary>
        /// Gets or sets the name of the Category.
        /// </summary>
        public string Name
        {
            get
            {
                return Presets.GetRealCategoryName(Model.Name);
            }
            set
            {
                if (Model.IsPreset)
                {
                    return;
                }
                Model.Name = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets whether this is a preset Category.
        /// </summary>
        public bool IsPreset
        {
            get
            {
                return Model.IsPreset;
            }
        }

        /// <summary>
        /// Gets the custom extra fields for the Category.
        /// </summary>
        public ExtraItemViewModel[] CustomFields
        {
            get
            {
                if (_extras == null)
                {
                    return null;
                }
                return _extras.Where(e => !e.Model.IsPreset).ToArray();
            }
        }

        /// <summary>
        /// The list of ExtraItemViewModels for the Category.
        /// </summary>
        private Collection<ExtraItemViewModel> _extras = new Collection<ExtraItemViewModel>();

        /// <summary>
        /// Gets or sets the list of ExtraItemViewModels for the Category.
        /// </summary>
        public Collection<ExtraItemViewModel> Extras
        {
            get
            {
                return _extras;
            }
            set
            {
                _extras = value;
                if (value is ObservableCollection<ExtraItemViewModel>)
                {
                    (value as ObservableCollection<ExtraItemViewModel>).CollectionChanged += ExtrasCollectionChanged;
                }
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The list of FlavorItemViewModels for the Category.
        /// </summary>
        private Collection<FlavorItemViewModel> _flavors = new Collection<FlavorItemViewModel>();

        /// <summary>
        /// Gets or sets the list of FlavorItemViewModels for the Category.
        /// </summary>
        public Collection<FlavorItemViewModel> Flavors
        {
            get
            {
                return _flavors;
            }
            set
            {
                _flavors = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="category">The Category to represent.</param>
        public CategoryViewModel(Category category) : base(category) { }

        /// <summary>
        /// Raises a PropertyChanged event for the CustomFields when the Extras collection changes.
        /// </summary>
        /// <param name="sender">The Extras ObservableCollection.</param>
        /// <param name="e">The event arguments.</param>
        private void ExtrasCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("CustomFields");
        }
    }
}
