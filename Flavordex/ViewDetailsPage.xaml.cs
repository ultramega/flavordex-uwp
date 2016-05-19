using Flavordex.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Flavordex
{
    /// <summary>
    /// Page to show the details for a journal entry.
    /// </summary>
    public sealed partial class ViewDetailsPage : Page
    {
        /// <summary>
        /// The Entry for which to display the details.
        /// </summary>
        private EntryViewModel Entry { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ViewDetailsPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads the requested Entry and realizes the relevant layout when the Page is navigated
        /// to.
        /// </summary>
        /// <param name="e">
        /// The event arguments containing the ID of the requested Entry as the Parameter.
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Entry = (EntryViewModel)e.Parameter;
        }
    }
}
