/*
  The MIT License (MIT)
  Copyright © 2016 Steve Guidetti

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the “Software”), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in
  all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
  THE SOFTWARE.
*/
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
