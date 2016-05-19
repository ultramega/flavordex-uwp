using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Flavordex.UI.Controls
{
    /// <summary>
    /// Control used for interacting with a RadarControl.
    /// </summary>
    public sealed partial class RadarGraphEditor : UserControl
    {
        /// <summary>
        /// Occurs when the cancel button is clicked.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event RoutedEventHandler Cancel = delegate { };

        /// <summary>
        /// Occurs when the save button is clicked.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event RoutedEventHandler Save = delegate { };

        /// <summary>
        /// Gets or sets the RadarControl to interact with.
        /// </summary>
        public RadarGraph Target
        {
            get { return (RadarGraph)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(RadarGraph), typeof(RadarGraphEditor), null);

        /// <summary>
        /// Gets or sets whether to show the Save and Cancel buttons.
        /// </summary>
        public bool ShowCommands
        {
            get { return (bool)GetValue(ShowCommandsProperty); }
            set { SetValue(ShowCommandsProperty, value); }
        }
        public static readonly DependencyProperty ShowCommandsProperty =
            DependencyProperty.Register("ShowCommands", typeof(bool), typeof(RadarGraphEditor), new PropertyMetadata(true));

        /// <summary>
        /// A copy of the original values of the RadarItems in the RadarGraph before editing.
        /// </summary>
        private Collection<RadarItem> _savedValues;

        /// <summary>
        /// Constructor.
        /// </summary>
        public RadarGraphEditor()
        {
            InitializeComponent();
            Content.RegisterPropertyChangedCallback(VisibilityProperty, OnVisibilityChanged);
        }

        /// <summary>
        /// Saves a copy of the current values of the items in the RadarGraph when the
        /// RadarGraphEditor is made visible.
        /// </summary>
        /// <param name="sender">The Content of this RadarGraphEditor.</param>
        /// <param name="dp">The VisibilityProperty.</param>
        private void OnVisibilityChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (Content.Visibility == Visibility.Visible && Target != null && Target.HasItems)
            {
                _savedValues = Target.Items;
            }
        }

        /// <summary>
        /// Turns the RadarControl to the previous item when the previous button is clicked.
        /// </summary>
        /// <param name="sender">The Button.</param>
        /// <param name="e">The event arguments.</param>
        private void OnPreviousClick(object sender, RoutedEventArgs e)
        {
            if (Target != null)
            {
                Target.SelectPreviousItem();
            }
        }

        /// <summary>
        /// Turns the RadarControl to the next item when the next button is clicked.
        /// </summary>
        /// <param name="sender">The Button.</param>
        /// <param name="e">The event arguments.</param>
        private void OnNextClick(object sender, RoutedEventArgs e)
        {
            if (Target != null)
            {
                Target.SelectNextItem();
            }
        }

        /// <summary>
        /// Resets the RadarGraph to its original state and fires the Cancel event when the Cancel
        /// Button is clicked.
        /// </summary>
        /// <param name="sender">The Button.</param>
        /// <param name="e">The event arguments.</param>
        private void OnCancel(object sender, RoutedEventArgs e)
        {
            if (Target != null)
            {
                Target.IsInteractive = false;
                Target.Items = _savedValues;
            }

            _savedValues = null;

            Cancel(this, e);
        }

        /// <summary>
        /// Fires the Save event when the Save Button is clicked.
        /// </summary>
        /// <param name="sender">The Button.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSave(object sender, RoutedEventArgs e)
        {
            _savedValues = null;

            if (Target != null)
            {
                Target.IsInteractive = false;
            }

            Save(this, e);
        }
    }
}
