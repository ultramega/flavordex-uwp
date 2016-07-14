using Flavordex.Models;
using Flavordex.Utilities;
using Windows.ApplicationModel.Resources;

namespace Flavordex.ViewModels
{
    /// <summary>
    /// Represents a Category in a list.
    /// </summary>
    public class CategoryItemViewModel : ModelViewModel<Category>
    {
        /// <summary>
        /// The format string for the entry count string.
        /// </summary>
        private static readonly string _countFormat =
            ResourceLoader.GetForCurrentView().GetString("EntryCount");

        /// <summary>
        /// Gets the name of the Category.
        /// </summary>
        public string Name
        {
            get
            {
                return Presets.GetRealCategoryName(Model.Name);
            }
        }

        /// <summary>
        /// Gets a formatted string representing the number of journal entries in the Category.
        /// </summary>
        public string EntryCount
        {
            get
            {
                var entries = Plurals.GetWord("Entries", Model.EntryCount);
                return string.Format(_countFormat, Model.EntryCount, entries);
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
        /// Constructor.
        /// </summary>
        /// <param name="category">The Category to represent.</param>
        public CategoryItemViewModel(Category category) : base(category) { }
    }
}
