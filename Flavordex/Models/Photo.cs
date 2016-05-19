using Flavordex.Models.Data;
using Flavordex.Utilities.Databases;

namespace Flavordex.Models
{
    /// <summary>
    /// Model representing a photo.
    /// </summary>
    public class Photo : Model
    {
        /// <summary>
        /// The primary ID of the journal entry this photo is associated with.
        /// </summary>
        public long EntryID
        {
            get { return _data.GetLong(Tables.Photos.ENTRY); }
            set { _data.SetLong(Tables.Photos.ENTRY, value); }
        }

        /// <summary>
        /// The hash of the photo file.
        /// </summary>
        public string Hash
        {
            get { return _data.GetString(Tables.Photos.HASH); }
            set { _data.SetString(Tables.Photos.HASH, value); }
        }

        /// <summary>
        /// The path to the photo file.
        /// </summary>
        public string Path
        {
            get { return _data.GetString(Tables.Photos.PATH); }
            set { _data.SetString(Tables.Photos.PATH, value); }
        }

        /// <summary>
        /// The Google Drive ID.
        /// </summary>
        public string DriveID
        {
            get { return _data.GetString(Tables.Photos.DRIVE_ID); }
            set { _data.SetString(Tables.Photos.DRIVE_ID, value); }
        }

        /// <summary>
        /// The sorting position in the list of photos.
        /// </summary>
        public long Position
        {
            get { return _data.GetLong(Tables.Photos.POS); }
            set { _data.SetLong(Tables.Photos.POS, value); }
        }
    }
}
