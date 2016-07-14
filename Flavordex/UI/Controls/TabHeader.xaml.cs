using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Flavordex.UI.Controls
{
    /// <summary>
    /// Control to display a tab indicator in a Pivot Control.
    /// </summary>
    public sealed partial class TabHeader : UserControl
    {
        /// <summary>
        /// Gets or sets the Symbol to display.
        /// </summary>
        public Symbol Symbol
        {
            get { return (Symbol)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }
        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register("Symbol", typeof(Symbol), typeof(TabHeader),
                new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets the label text.
        /// </summary>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(TabHeader), null);

        /// <summary>
        /// Constructor.
        /// </summary>
        public TabHeader()
        {
            InitializeComponent();
        }
    }
}
