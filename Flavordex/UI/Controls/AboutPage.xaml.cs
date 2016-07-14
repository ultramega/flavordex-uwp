using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace Flavordex.UI.Controls
{
    /// <summary>
    /// The about page to show some support information about the application.
    /// </summary>
    public sealed partial class AboutPage : UserControl
    {
        /// <summary>
        /// Gets the version message.
        /// </summary>
        private string Version
        {
            get
            {
                var version = Package.Current.Id.Version;
                return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor,
                    version.Build, version.Revision);
            }
        }

        /// <summary>
        /// Gets the link to send an email.
        /// </summary>
        private string EmailLink
        {
            get
            {
                return "mailto://flavordex@ultramegasoft.com?subject=Flavordex " + Version;
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
        public AboutPage()
        {
            InitializeComponent();
        }
    }
}
