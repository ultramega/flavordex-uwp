using Flavordex.Models;
using Flavordex.Models.Data;
using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Flavordex.Utilities
{
    /// <summary>
    /// Utilities for handling photos.
    /// </summary>
    class PhotoUtilities
    {
        /// <summary>
        /// ObjectCache implementation for BitmapImages.
        /// </summary>
        private class ImageCache : ObjectCache<BitmapImage>
        {
            /// <summary>
            /// Gets the size of a stored image.
            /// </summary>
            /// <param name="item">The image to measure.</param>
            /// <returns>The number of bytes the image occupies.</returns>
            protected override ulong GetSizeOf(BitmapImage item)
            {
                if (item == null)
                {
                    return 0UL;
                }

                return (ulong)(item.PixelHeight * item.PixelWidth * 4);
            }
        }

        /// <summary>
        /// An empty bitmap.
        /// </summary>
        public static readonly BitmapImage EmptyBitmap = new BitmapImage();

        /// <summary>
        /// Occurs when the thumbnail for a journal entry changes.
        /// </summary>
        public static event ThumbnailChangedEventHandler ThumbnailChanged = delegate { };

        /// <summary>
        /// Handler for the ThumbnailChanged event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        public delegate void ThumbnailChangedEventHandler(object sender, ThumbnailChangedEventArgs e);

        /// <summary>
        /// The arguments for the ThumbnailChanged event.
        /// </summary>
        public class ThumbnailChangedEventArgs : EventArgs
        {
            /// <summary>
            /// The primary ID of the journal entry.
            /// </summary>
            public long EntryId { get; }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="entryId">The primary ID of the journal entry.</param>
            internal ThumbnailChangedEventArgs(long entryId)
            {
                EntryId = entryId;
            }
        }

        /// <summary>
        /// The folder where images are stored.
        /// </summary>
        private const string _albumDirectory = "Flavordex";

        /// <summary>
        /// The height and width of thumbnail images.
        /// </summary>
        private const uint _thumbnailSize = 40;

        /// <summary>
        /// Memory cache for thumbnail images.
        /// </summary>
        private static ImageCache _thumbnailCache = new ImageCache();

        /// <summary>
        /// Gets a file picker to select a photo.
        /// </summary>
        /// <returns>The configured FileOpenPicker.</returns>
        public static FileOpenPicker GetPicker()
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            return picker;
        }

        /// <summary>
        /// Captures a new photo with the camera.
        /// </summary>
        /// <returns>The captured StorageFile.</returns>
        public static async Task<StorageFile> CapturePhoto()
        {
            var capture = new CameraCaptureUI();
            capture.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            var file = await capture.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (file != null)
            {
                await file.RenameAsync("IMG_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".jpg");
            }
            return file;
        }

        /// <summary>
        /// Adds a Photo to a journal entry.
        /// </summary>
        /// <param name="name">The name of the photo file to add.</param>
        /// <param name="entryId">The primary ID of the journal entry.</param>
        /// <param name="position">The sorting position of the Photo.</param>
        /// <returns>The Photo that was added.</returns>
        public static async Task<Photo> AddPhoto(string name, long entryId, long position)
        {
            var file = await GetPhotoFileAsync(name);
            if (file == null)
            {
                return null;
            }

            return await AddPhoto(file, entryId, position);
        }

        /// <summary>
        /// Adds a Photo to a journal entry.
        /// </summary>
        /// <param name="file">The photo file to add.</param>
        /// <param name="entryId">The primary ID of the journal entry.</param>
        /// <param name="position">The sorting position of the Photo.</param>
        /// <returns>The Photo that was added.</returns>
        public static async Task<Photo> AddPhoto(StorageFile file, long entryId, long position)
        {
            var photo = new Photo();
            photo.EntryID = entryId;
            photo.Path = await SavePhoto(file);
            photo.Hash = await GetMD5Hash(file);
            photo.Position = position;
            await DatabaseHelper.InsertPhotoAsync(photo);

            if (position == 0)
            {
                ThumbnailChanged(null, new ThumbnailChangedEventArgs(entryId));
            }

            return photo;
        }

        /// <summary>
        /// Gets the MD5 hash of a file as a 32 character hex string.
        /// </summary>
        /// <param name="file">The file to hash.</param>
        /// <returns>The MD5 hash of the file.</returns>
        public static async Task<string> GetMD5Hash(StorageFile file)
        {
            var md5 = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5).CreateHash();
            var buffer = new Windows.Storage.Streams.Buffer(8192);
            using (var stream = await file.OpenReadAsync())
            {
                do
                {
                    await stream.ReadAsync(buffer, 8192, InputStreamOptions.None);
                    md5.Append(buffer);
                }
                while (buffer.Length > 0);
            }
            return CryptographicBuffer.EncodeToHexString(md5.GetValueAndReset());
        }

        /// <summary>
        /// Saves a photo to the album if it does not already exist.
        /// </summary>
        /// <param name="source">The source file.</param>
        /// <returns>The name of the saved file.</returns>
        public static async Task<string> SavePhoto(StorageFile source)
        {
            var folder = await KnownFolders.PicturesLibrary.CreateFolderAsync(_albumDirectory, CreationCollisionOption.OpenIfExists);
            var destination = await folder.TryGetItemAsync(source.Name) as StorageFile;
            if (destination == null)
            {
                destination = await source.CopyAsync(folder);
            }
            return destination.Name;
        }

        /// <summary>
        /// Gets a photo from the album.
        /// </summary>
        /// <param name="name">The name of the image file.</param>
        /// <returns>The BitmapImage containing the photo.</returns>
        public static async Task<BitmapImage> GetPhoto(string name)
        {
            var file = await GetPhotoFileAsync(name);
            if (file != null)
            {
                var bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(await file.OpenReadAsync());
                return bitmap;
            }
            return null;
        }

        /// <summary>
        /// Gets a photo file from the album directory.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        /// <returns>The StorageFile representing the photo, or null if it is not found.</returns>
        private static async Task<StorageFile> GetPhotoFileAsync(string name)
        {
            var folder = await KnownFolders.PicturesLibrary.TryGetItemAsync(_albumDirectory) as StorageFolder;
            if (folder != null)
            {
                return await folder.TryGetItemAsync(name) as StorageFile;
            }
            return null;
        }

        /// <summary>
        /// Get the thumbnail for a journal entry.
        /// </summary>
        /// <param name="entryId">The primary ID of the Entry.</param>
        /// <returns>A BitmapImage containing the thumbnail.</returns>
        public static async Task<BitmapImage> GetThumbnailAsync(long entryId)
        {
            if (_thumbnailCache.KeyExists(entryId))
            {
                return _thumbnailCache.Get(entryId);
            }

            var name = string.Format("thumb_{0}.jpg", entryId);
            var file = await ApplicationData.Current.TemporaryFolder.TryGetItemAsync(name) as StorageFile;

            if (file == null)
            {
                file = await GenerateThumbnailAsync(entryId, name);
            }

            if (file == null)
            {
                _thumbnailCache.Store(entryId, EmptyBitmap);
                return EmptyBitmap;
            }

            var bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(await file.OpenReadAsync());
            _thumbnailCache.Store(entryId, bitmap);
            return bitmap;
        }

        /// <summary>
        /// Generates a thumbnail for a journal entry.
        /// </summary>
        /// <param name="entryId">The primary ID for the Entry.</param>
        /// <param name="fileName">The name of the image file to create.</param>
        /// <returns>The StorageFile for the saved file.</returns>
        public static async Task<StorageFile> GenerateThumbnailAsync(long entryId, string fileName)
        {
            var path = await DatabaseHelper.GetPosterPhotoAsync(entryId);
            if (path == null)
            {
                return null;
            }

            var sourceDirectory = await KnownFolders.PicturesLibrary.CreateFolderAsync(_albumDirectory, CreationCollisionOption.OpenIfExists);
            var sourceFile = await sourceDirectory.TryGetItemAsync(path) as StorageFile;
            if (sourceFile == null)
            {
                return null;
            }

            using (var stream = await sourceFile.OpenReadAsync())
            {
                var decoder = await BitmapDecoder.CreateAsync(stream);
                var sourceBitmap = await decoder.GetSoftwareBitmapAsync();
                var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                await SaveThumbnailAsync(sourceBitmap, file);
                return file;
            }
        }

        /// <summary>
        /// Saves a thumbnail of the source bitmap to a file.
        /// </summary>
        /// <param name="sourceBitmap">The source bitmap.</param>
        /// <param name="outputFile">The destination file.</param>
        private static async Task SaveThumbnailAsync(SoftwareBitmap sourceBitmap, StorageFile outputFile)
        {
            using (var stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                encoder.SetSoftwareBitmap(sourceBitmap);

                uint width, height;
                if (sourceBitmap.PixelHeight > sourceBitmap.PixelWidth)
                {
                    width = _thumbnailSize;
                    height = (uint)(sourceBitmap.PixelHeight * width / sourceBitmap.PixelWidth);
                }
                else
                {
                    height = _thumbnailSize;
                    width = (uint)(sourceBitmap.PixelWidth * height / sourceBitmap.PixelHeight);
                }
                encoder.BitmapTransform.ScaledWidth = width;
                encoder.BitmapTransform.ScaledHeight = height;
                encoder.BitmapTransform.Bounds = new BitmapBounds()
                {
                    Height = _thumbnailSize,
                    Width = _thumbnailSize,
                    X = (width - _thumbnailSize) / 2,
                    Y = (height - _thumbnailSize) / 2
                };
                encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;

                await encoder.FlushAsync();
            }
        }

        /// <summary>
        /// Deletes the thumbnail for a journal entry.
        /// </summary>
        /// <param name="entryId">The primary ID of the entry.</param>
        public static async void DeleteThumbnail(long entryId)
        {
            _thumbnailCache.Remove(entryId);
            var name = string.Format("thumb_{0}.jpg", entryId);
            var file = await ApplicationData.Current.TemporaryFolder.TryGetItemAsync(name);
            if (file != null)
            {
                await file.DeleteAsync();
            }
            ThumbnailChanged(null, new ThumbnailChangedEventArgs(entryId));
        }
    }
}
