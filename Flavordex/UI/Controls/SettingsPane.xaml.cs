using System;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

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
            DependencyProperty.Register("DetectLocation", typeof(bool), typeof(SettingsPane),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the Visibility of the Detect Location ToggleSwitch.
        /// </summary>
        public Visibility DetectLocationVisibility
        {
            get { return (Visibility)GetValue(DetectLocationVisibilityProperty); }
            set { SetValue(DetectLocationVisibilityProperty, value); }
        }
        public static readonly DependencyProperty DetectLocationVisibilityProperty =
            DependencyProperty.Register("DetectLocationVisibility", typeof(Visibility),
                typeof(SettingsPane), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// A reference to the CategoryListDialog.
        /// </summary>
        private CategoryListDialog _categoryListDialog;

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
        /// Restores state when the Control is loaded.
        /// </summary>
        /// <param name="sender">This Control.</param>
        /// <param name="e">The event arguments.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_categoryListDialog != null)
            {
                OpenCategoryListDialog();
            }
        }

        /// <summary>
        /// Opens the Edit Categories Dialog when the Edit Categories Button is clicked.
        /// </summary>
        /// <param name="sender">The Button.</param>
        /// <param name="e">The event arguments.</param>
        private void OnEditCategories(object sender, RoutedEventArgs e)
        {
            _categoryListDialog = new CategoryListDialog();
            OpenCategoryListDialog();
        }

        /// <summary>
        /// Opens the Category list dialog.
        /// </summary>
        private async void OpenCategoryListDialog()
        {
            var result = await _categoryListDialog.ShowAsync();
            if (result == ContentDialogResult.Secondary)
            {
                _categoryListDialog = null;
            }
        }

        /// <summary>
        /// Updates the Detect Location setting when the Detect Location property changes.
        /// </summary>
        /// <param name="sender">This Control.</param>
        /// <param name="dp">The DetectLocation property.</param>
        private async void OnDetectLocationChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (DetectLocation
                && await Geolocator.RequestAccessAsync() != GeolocationAccessStatus.Allowed)
            {
                DetectLocation = false;
                FlyoutBase.ShowAttachedFlyout(DetectLocationToggle);
            }
            Settings.DetectLocation = DetectLocation;
        }
    }
}
