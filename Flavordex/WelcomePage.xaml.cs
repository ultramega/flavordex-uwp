using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace Flavordex
{
    /// <summary>
    /// A Page containing a welcome message.
    /// </summary>
    public sealed partial class WelcomePage : Page
    {
        /// <summary>
        /// Gets the version message.
        /// </summary>
        private string Version
        {
            get
            {
                var format = ResourceLoader.GetForCurrentView().GetString("Message/Version");
                var version = Package.Current.Id.Version;
                return string.Format(format, version.Major, version.Minor, version.Build,
                    version.Revision);
            }
        }

        /// <summary>
        /// Gets the copyright message.
        /// </summary>
        private string Copyright
        {
            get
            {
                var format = ResourceLoader.GetForCurrentView().GetString("Message/Copyright");
                return string.Format(format, DateTime.Now.Year);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public WelcomePage()
        {
            InitializeComponent();
        }
    }
}
