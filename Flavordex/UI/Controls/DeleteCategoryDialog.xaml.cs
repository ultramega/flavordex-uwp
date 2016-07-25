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
using Flavordex.Models;
using Flavordex.Utilities;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace Flavordex.UI.Controls
{
    /// <summary>
    /// Dialog to request user confirmation to delete a Category.
    /// </summary>
    public sealed partial class DeleteCategoryDialog : ContentDialog
    {
        /// <summary>
        /// The format string for the dialog message.
        /// </summary>
        private static readonly string _messageFormat =
            ResourceLoader.GetForCurrentView().GetString("Message/ConfirmDeleteCategory");

        /// <summary>
        /// The format string for the confirmation check box message.
        /// </summary>
        private static readonly string _confirmMessageFormat =
            ResourceLoader.GetForCurrentView().GetString("Message/DeleteCategoryEntries");

        /// <summary>
        /// Gets the dialog message.
        /// </summary>
        private string Message { get; }

        /// <summary>
        /// Gets the confirmation check box message.
        /// </summary>
        private string ConfirmMessage { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="category">The Category to be deleted.</param>
        public DeleteCategoryDialog(Category category)
        {
            InitializeComponent();
            Message = string.Format(_messageFormat, category.Name);
            if (category.EntryCount > 0)
            {
                ConfirmMessage = string.Format(_confirmMessageFormat, category.EntryCount,
                    Plurals.GetWord("entries", category.EntryCount));
            }
            else
            {
                Confirm.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Confirm.IsChecked = true;
            }
        }
    }
}
