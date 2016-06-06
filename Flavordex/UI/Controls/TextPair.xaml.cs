using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Flavordex.UI.Controls
{
    /// <summary>
    /// Control to display a primary and secondary line of text.
    /// </summary>
    public sealed partial class TextPair : UserControl
    {
        /// <summary>
        /// Gets or sets the Style to apply to the primary TextBlock.
        /// </summary>
        public Style PrimaryStyle
        {
            get { return (Style)GetValue(PrimaryStyleProperty); }
            set { SetValue(PrimaryStyleProperty, value); }
        }
        public static readonly DependencyProperty PrimaryStyleProperty =
            DependencyProperty.Register("PrimaryStyle", typeof(Style), typeof(TextPair), null);

        /// <summary>
        /// Gets or sets the Text of the primary TextBlock.
        /// </summary>
        public string PrimaryText
        {
            get { return (string)GetValue(PrimaryTextProperty); }
            set { SetValue(PrimaryTextProperty, value); }
        }
        public static readonly DependencyProperty PrimaryTextProperty =
            DependencyProperty.Register("PrimaryText", typeof(string), typeof(TextPair), null);

        /// <summary>
        /// Gets or sets the Style to apply to the secondary TextBlock.
        /// </summary>
        public Style SecondaryStyle
        {
            get { return (Style)GetValue(SecondaryStyleProperty); }
            set { SetValue(SecondaryStyleProperty, value); }
        }
        public static readonly DependencyProperty SecondaryStyleProperty =
            DependencyProperty.Register("SecondaryStyle", typeof(Style), typeof(TextPair), null);

        /// <summary>
        /// Gets or sets the Text of the secondary TextBlock.
        /// </summary>
        public string SecondaryText
        {
            get { return (string)GetValue(SecondaryTextProperty); }
            set { SetValue(SecondaryTextProperty, value); }
        }
        public static readonly DependencyProperty SecondaryTextProperty =
            DependencyProperty.Register("SecondaryText", typeof(string), typeof(TextPair), null);

        /// <summary>
        /// Constructor.
        /// </summary>
        public TextPair()
        {
            InitializeComponent();
            RegisterPropertyChangedCallback(PrimaryTextProperty, OnTextChanged);
            RegisterPropertyChangedCallback(SecondaryTextProperty, OnTextChanged);
        }

        /// <summary>
        /// Updates the TextBlocks and layout when a text property changes.
        /// </summary>
        /// <param name="sender">The Control.</param>
        /// <param name="dp">The event arguments.</param>
        private void OnTextChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (string.IsNullOrEmpty(PrimaryText))
            {
                if (!string.IsNullOrEmpty(SecondaryText))
                {
                    Text1.Text = SecondaryText;
                }
                else
                {
                    Text1.Text = "?";
                }
                Text2.Text = "";
                Text2.Visibility = Visibility.Collapsed;
            }
            else
            {
                Text1.Text = PrimaryText;
                if (!string.IsNullOrEmpty(SecondaryText))
                {
                    Text2.Text = SecondaryText;
                    Text2.Visibility = Visibility.Visible;
                }
                else
                {
                    Text2.Text = "";
                    Text2.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
