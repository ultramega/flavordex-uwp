using Flavordex.Models;
using Flavordex.Utilities;
using System;
using Windows.UI.Xaml.Media;

namespace Flavordex.ViewModels
{
    /// <summary>
    /// Represents a Photo.
    /// </summary>
    public class PhotoItemViewModel : ModelViewModel<Photo>
    {
        /// <summary>
        /// The bitmap of this photo.
        /// </summary>
        private ImageSource _bitmap;

        /// <summary>
        /// Gets or sets the bitmap of this photo.
        /// </summary>
        public ImageSource Bitmap
        {
            get
            {
                if (_bitmap == null)
                {
                    LoadBitmap();
                }
                return _bitmap;
            }
            private set
            {
                _bitmap = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="photo">The Photo to represent.</param>
        public PhotoItemViewModel(Photo photo) : base(photo) { }

        /// <summary>
        /// Asynchronously loads the image from the disk.
        /// </summary>
        private async void LoadBitmap()
        {
            Bitmap = await PhotoUtilities.GetPhotoAsync(Model.Path);
        }
    }
}
