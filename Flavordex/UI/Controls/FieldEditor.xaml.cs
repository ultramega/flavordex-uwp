using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Flavordex.UI.Controls
{
    /// <summary>
    /// Special input Control for editing extra fields of a Category. Includes a TextBox and a
    /// Button to delete or undelete.
    /// </summary>
    public sealed partial class FieldEditor : UserControl
    {
        /// <summary>
        /// Occurs when the Return key is pressed in the TextBox.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
            "CA1009:DeclareEventHandlersCorrectly")]
        public event RoutedEventHandler Return = delegate { };

        /// <summary>
        /// Gets or sets the value of the field.
        /// </summary>
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(FieldEditor), null);

        /// <summary>
        /// Gets or sets whether the field is in the deleted state.
        /// </summary>
        public bool Deleted
        {
            get { return (bool)GetValue(DeletedProperty); }
            set { SetValue(DeletedProperty, value); }
        }
        public static readonly DependencyProperty DeletedProperty =
            DependencyProperty.Register("Deleted", typeof(bool), typeof(FieldEditor),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the placeholder text of the field.
        /// </summary>
        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText", typeof(string), typeof(FieldEditor),
                null);

        /// <summary>
        /// Gets or sets the maximum length of the value of the field.
        /// </summary>
        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int), typeof(FieldEditor),
                new PropertyMetadata(0));

        /// <summary>
        /// Constructor.
        /// </summary>
        public FieldEditor()
        {
            InitializeComponent();
            RegisterPropertyChangedCallback(DeletedProperty, OnDeletePropertyChanged);
        }

        /// <summary>
        /// Changes the state of the field when the Deleted property changes.
        /// </summary>
        /// <param name="sender">This Control.</param>
        /// <param name="dp">The DeletedProperty.</param>
        private void OnDeletePropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (Deleted)
            {
                DeleteButton.Visibility = Visibility.Collapsed;
                UndoButton.Visibility = Visibility.Visible;
                Field.IsEnabled = false;
            }
            else
            {
                DeleteButton.Visibility = Visibility.Visible;
                UndoButton.Visibility = Visibility.Collapsed;
                Field.IsEnabled = true;
            }
        }

        /// <summary>
        /// Changes the field to the deleted state when the delete Button is pressed.
        /// </summary>
        /// <param name="sender">The delete Button.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDelete(object sender, RoutedEventArgs e)
        {
            Deleted = true;
        }

        /// <summary>
        /// Changes the field to the normal state when the undo Button is pressed.
        /// </summary>
        /// <param name="sender">The undo Button.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUndo(object sender, RoutedEventArgs e)
        {
            Deleted = false;
        }

        /// <summary>
        /// Commits the current value and raises the Return event when the Enter key is pressed.
        /// </summary>
        /// <param name="sender">The TextBox.</param>
        /// <param name="e">The event arguments.</param>
        private void OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                Value = Field.Text;
                Return(sender, e);
            }
        }
    }
}
