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
using Flavordex.Models.Data;
using Flavordex.Utilities;
using Flavordex.Utilities.Databases;
using Flavordex.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Flavordex
{
    /// <summary>
    /// Page to display the photos for a journal entry.
    /// </summary>
    public sealed partial class ViewPhotosPage : Page
    {
        /// <summary>
        /// Gets the list of photos for the entry.
        /// </summary>
        public ObservableCollection<PhotoItemViewModel> Photos { get; } =
            new ObservableCollection<PhotoItemViewModel>();

        /// <summary>
        /// Gets or sets the Visibility of the empty list message.
        /// </summary>
        public Visibility NoPhotosVisibility
        {
            get { return (Visibility)GetValue(NoPhotosVisibilityProperty); }
            set { SetValue(NoPhotosVisibilityProperty, value); }
        }
        public static readonly DependencyProperty NoPhotosVisibilityProperty =
            DependencyProperty.Register("NoPhotosVisibility", typeof(Visibility),
                typeof(ViewPhotosPage), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// The journal entry.
        /// </summary>
        private Entry _entry;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ViewPhotosPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads the Photos for the requested Entry when the Page is navigated to.
        /// </summary>
        /// <param name="e">
        /// The event arguments containing the ID of the requested Entry as the Parameter.
        /// </param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _entry = e.Parameter as Entry;

            Photos.Clear();
            foreach (var item in await DatabaseHelper.GetEntryPhotosAsync(_entry.ID))
            {
                Photos.Add(new PhotoItemViewModel(item));
            }
            NoPhotosVisibility = Photos.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

            DatabaseHelper.RecordChanged += OnRecordChanged;
        }

        /// <summary>
        /// Removes the RecordChanged event handler when the Page is navigated away from.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            DatabaseHelper.RecordChanged -= OnRecordChanged;
        }

        /// <summary>
        /// Updates the Photos list when a database record changes.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnRecordChanged(object sender, RecordChangedEventArgs e)
        {
            if (e.Model is Photo && (e.Model as Photo).EntryID == _entry.ID)
            {
                if (e.Action == RecordChangedAction.Insert)
                {
                    var item = new PhotoItemViewModel(e.Model as Photo);
                    if (!Photos.Any(k => k.Model.Hash == item.Model.Hash))
                    {
                        Photos.Add(item);
                        FlipView.SelectedItem = item;
                        if (FlipView.SelectedIndex == 0)
                        {
                            await PhotoUtilities.DeleteThumbnailAsync(_entry.ID);
                        }
                    }
                }
                else if (e.Action == RecordChangedAction.Delete)
                {
                    var item = Photos.FirstOrDefault(k => k.Model.ID == e.Model.ID);
                    if (Photos.IndexOf(item) == 0)
                    {
                        await PhotoUtilities.DeleteThumbnailAsync(_entry.ID);
                    }
                    Photos.Remove(item);
                }
                NoPhotosVisibility = Photos.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Opens the context menu when the pointer is held down on the FlipView.
        /// </summary>
        /// <param name="sender">The FlipView.</param>
        /// <param name="e">The event arguments.</param>
        private void OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            PhotoFlyout.ShowAt(FlipView, e.GetPosition(FlipView));
        }

        /// <summary>
        /// Opens the context menu when the FlipView is right-clicked.
        /// </summary>
        /// <param name="sender">The FlipView.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            PhotoFlyout.ShowAt(FlipView, e.GetPosition(FlipView));
        }

        /// <summary>
        /// Removes the selected Photo after requesting confirmation from the user.
        /// </summary>
        /// <param name="sender">The MenuFlyoutItem.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnRemoveClick(object sender, RoutedEventArgs e)
        {
            var result = await RemoveDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var index = FlipView.SelectedIndex;
                await DatabaseHelper.DeletePhotoAsync(Photos[index].Model);
            }
        }

        /// <summary>
        /// Opens the camera app to take a new photo.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        public async void OnTakePhoto(object sender, RoutedEventArgs e)
        {
            var file = await PhotoUtilities.CapturePhotoAsync();
            if (file != null)
            {
                var position = Photos.Count > 0 ? Photos.Last().Model.Position + 1 : 0;
                var photo = await PhotoUtilities.AddPhotoAsync(file, _entry.ID, position);
            }
        }

        /// <summary>
        /// Opens the file selector to select a photo to add.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        public async void OnSelectPhoto(object sender, RoutedEventArgs e)
        {
            var files = await PhotoUtilities.GetPicker().PickMultipleFilesAsync();
            if (files.Count > 0)
            {
                var position = Photos.Count > 0 ? Photos.Last().Model.Position + 1 : 0;
                foreach (var file in files)
                {
                    var photo = await PhotoUtilities.AddPhotoAsync(file, _entry.ID, position);
                }
            }
        }

        /// <summary>
        /// Opens a file picker to replace a missing photo when the Locate Photo button is clicked.
        /// </summary>
        /// <param name="sender">The Button.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnLocatePhoto(object sender, RoutedEventArgs e)
        {
            var photo = (sender as FrameworkElement).DataContext as PhotoItemViewModel;
            var file = await PhotoUtilities.GetPicker().PickSingleFileAsync();
            if (file != null)
            {
                await PhotoUtilities.UpdatePhotoAsync(photo.Model, file);
                if (Photos.IndexOf(photo) == 0)
                {
                    await PhotoUtilities.DeleteThumbnailAsync(_entry.ID);
                }
            }
        }

        /// <summary>
        /// Removes a missing photo when the Remove Photo button is clicked.
        /// </summary>
        /// <param name="sender">The Button.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnRemovePhoto(object sender, RoutedEventArgs e)
        {
            var photo = (sender as FrameworkElement).DataContext as PhotoItemViewModel;
            await DatabaseHelper.DeletePhotoAsync(photo.Model);
        }
    }
}
