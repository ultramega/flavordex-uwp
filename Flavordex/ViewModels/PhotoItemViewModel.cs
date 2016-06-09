using Flavordex.Models;
using Flavordex.Utilities;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media;

namespace Flavordex.ViewModels
{
    /// <summary>
    /// Represents a Photo.
    /// </summary>
    public class PhotoItemViewModel : ModelViewModel<Photo>
    {
        /// <summary>
        /// Gets the name of the photo file.
        /// </summary>
        public string FileName
        {
            get
            {
                return Model.Path;
            }
        }

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
                RaisePropertyChanged("PhotoNotFound");
            }
        }

        /// <summary>
        /// Gets whether the photo file was not found.
        /// </summary>
        public bool PhotoNotFound
        {
            get
            {
                return Bitmap == PhotoUtilities.EmptyBitmap;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="photo">The Photo to represent.</param>
        public PhotoItemViewModel(Photo photo) : base(photo) { }

        /// <summary>
        /// Clears the cached Bitmap when all properties change.
        /// </summary>
        /// <param name="name">The name of the property that changed.</param>
        protected override void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            if (name == null)
            {
                _bitmap = null;
            }
            base.RaisePropertyChanged(name);
        }

        /// <summary>
        /// Asynchronously loads the image from the disk.
        /// </summary>
        private async void LoadBitmap()
        {
            Bitmap = await PhotoUtilities.GetPhotoAsync(Model.Path);
        }
    }
}
