using Flavordex.Models;
using Flavordex.Utilities;
using System;
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
