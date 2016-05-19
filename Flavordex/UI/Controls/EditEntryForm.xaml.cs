using Flavordex.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Flavordex.UI.Controls
{
    /// <summary>
    /// Control to display the input form for editing a journal entry.
    /// </summary>
    public sealed partial class EditEntryForm : UserControl
    {
        /// <summary>
        /// Gets or sets the Entry to edit.
        /// </summary>
        public EntryViewModel Entry
        {
            get { return (EntryViewModel)GetValue(EntryProperty); }
            set { SetValue(EntryProperty, value); }
        }
        public static readonly DependencyProperty EntryProperty =
            DependencyProperty.Register("Entry", typeof(EntryViewModel), typeof(EditEntryForm), null);

        /// <summary>
        /// Constructor.
        /// </summary>
        public EditEntryForm()
        {
            InitializeComponent();
        }
    }
}
