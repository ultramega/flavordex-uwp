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
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Flavordex.ViewModels
{
    /// <summary>
    /// Represents an Entry in a list.
    /// </summary>
    public class EntryItemViewModel : ModelViewModel<Entry>
    {
        /// <summary>
        /// Gets the title of the Entry.
        /// </summary>
        public string Title
        {
            get
            {
                return Model.Title;
            }
        }

        /// <summary>
        /// Gets the name of the Maker for the Entry.
        /// </summary>
        public string Maker
        {
            get
            {
                return Model.Maker;
            }
        }

        /// <summary>
        /// Gets the rating of the Entry.
        /// </summary>
        public double Rating
        {
            get
            {
                return Model.Rating;
            }
        }

        /// <summary>
        /// Gets the date of the Entry.
        /// </summary>
        public DateTime Date
        {
            get
            {
                return Model.Date;
            }
        }

        /// <summary>
        /// The thumbnail representing the Entry.
        /// </summary>
        private ImageSource _thumbnail;

        /// <summary>
        /// Gets or sets the thumbnail representing the Entry.
        /// </summary>
        public ImageSource Thumbnail
        {
            get
            {
                if (_thumbnail == null)
                {
                    LoadThumbnail();
                }
                return _thumbnail;
            }
            private set
            {
                _thumbnail = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The Visibility of the thumbnail.
        /// </summary>
        private Visibility _thumbnailVisibility = Visibility.Visible;

        /// <summary>
        /// Gets or sets the Visibility of the thumbnail.
        /// </summary>
        public Visibility ThumbnailVisibility
        {
            get
            {
                return _thumbnailVisibility;
            }
            set
            {
                _thumbnailVisibility = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entry">The Entry to represent.</param>
        public EntryItemViewModel(Entry entry) : base(entry)
        {
            PhotoUtilities.ThumbnailChanged += OnThumbnailChanged;
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~EntryItemViewModel()
        {
            PhotoUtilities.ThumbnailChanged -= OnThumbnailChanged;
        }

        /// <summary>
        /// Resets the thumbnail bitmap when the thumbnail changes.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event parameters.</param>
        private void OnThumbnailChanged(object sender, PhotoUtilities.ThumbnailChangedEventArgs e)
        {
            if (e.EntryId == Model.ID)
            {
                Thumbnail = null;
            }
        }

        /// <summary>
        /// Asynchronously loads the thumbnail image for the Entry.
        /// </summary>
        private async void LoadThumbnail()
        {
            Thumbnail = await PhotoUtilities.GetThumbnailAsync(Model.ID);
        }
    }
}
