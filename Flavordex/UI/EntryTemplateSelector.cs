using Flavordex.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Flavordex.UI
{
    /// <summary>
    /// Template selector for selecting a specialized layout for entries in preset categories.
    /// </summary>
    public class EntryTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the template for non-specific categories.
        /// </summary>
        public DataTemplate GenericTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for the Beer category.
        /// </summary>
        public DataTemplate BeerTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for the Coffee category.
        /// </summary>
        public DataTemplate CoffeeTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for the Whiskey category.
        /// </summary>
        public DataTemplate WhiskeyTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for the Wine category.
        /// </summary>
        public DataTemplate WineTemplate { get; set; }

        /// <summary>
        /// Selects the appropriate template based on the ViewModel type.
        /// </summary>
        /// <param name="item">The ViewModel.</param>
        /// <param name="container">The ContentControl.</param>
        /// <returns>The DataTemplate to use.</returns>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is BeerEntryViewModel)
            {
                return BeerTemplate;
            }
            else if (item is CoffeeEntryViewModel)
            {
                return CoffeeTemplate;
            }
            else if (item is WhiskeyEntryViewModel)
            {
                return WhiskeyTemplate;
            }
            else if (item is WineEntryViewModel)
            {
                return WineTemplate;
            }
            else if (item is EntryViewModel)
            {
                return GenericTemplate;
            }

            return base.SelectTemplateCore(item, container);
        }
    }
}
