using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Flavordex.UI.Controls
{
    /// <summary>
    /// Contains the interface for changing application settings.
    /// </summary>
    public sealed partial class SettingsPane : UserControl
    {
        /// <summary>
        /// Gets or sets the status of the Detect Location ToggleSwitch.
        /// </summary>
        public bool DetectLocation
        {
            get { return (bool)GetValue(DetectLocationProperty); }
            set { SetValue(DetectLocationProperty, value); }
        }
        public static readonly DependencyProperty DetectLocationProperty =
            DependencyProperty.Register("DetectLocation", typeof(bool), typeof(SettingsPane), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the Visibility of the Detect Location ToggleSwitch.
        /// </summary>
        public Visibility DetectLocationVisibility
        {
            get { return (Visibility)GetValue(DetectLocationVisibilityProperty); }
            set { SetValue(DetectLocationVisibilityProperty, value); }
        }
        public static readonly DependencyProperty DetectLocationVisibilityProperty =
            DependencyProperty.Register("DetectLocationVisibility", typeof(Visibility), typeof(SettingsPane), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingsPane()
        {
            InitializeComponent();
            DetectLocation = Settings.DetectLocation;

            Settings.SettingChanged += OnSettingChanged;
            RegisterPropertyChangedCallback(DetectLocationProperty, OnDetectLocationChanged);
        }

        /// <summary>
        /// Updates the interface when a setting is changed.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSettingChanged(object sender, Settings.SettingChangedEventArgs e)
        {
            switch (e.Key)
            {
                case Settings.Key.DetectLocation:
                    DetectLocation = Settings.DetectLocation;
                    break;
            }
        }

        /// <summary>
        /// Opens the Edit Categories Dialog when the Edit Categories Button is clicked.
        /// </summary>
        /// <param name="sender">The Button.</param>
        /// <param name="e">The event arguments.</param>
        private void OnEditCategories(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Updates the Detect Location setting when the Detect Location property changes.
        /// </summary>
        /// <param name="sender">This Control.</param>
        /// <param name="dp">The DetectLocation property.</param>
        private void OnDetectLocationChanged(DependencyObject sender, DependencyProperty dp)
        {
            Settings.DetectLocation = DetectLocation;
        }
    }
}
