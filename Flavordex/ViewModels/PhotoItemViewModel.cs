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
