using System;

namespace Flavordex.Utilities.Databases
{
    /// <summary>
    /// The base for all Models.
    /// </summary>
    public class Model
    {
        /// <summary>
        /// Occurs when the data in this Model changes.
        /// </summary>
        public event EventHandler RecordChanged = delegate { };

        /// <summary>
        /// Occurs when the record represented by this Model is deleted.
        /// </summary>
        public event EventHandler RecordDeleted = delegate { };

        /// <summary>
        /// The data backing the Model.
        /// </summary>
        protected ContentValues _data = new ContentValues();

        /// <summary>
        /// The primary ID of the table row.
        /// </summary>
        public long ID
        {
            get { return _data.GetLong(BaseColumns._ID); }
            set { _data.SetLong(BaseColumns._ID, value); }
        }

        /// <summary>
        /// Gets a copy of the data backing this Model.
        /// </summary>
        /// <returns>The data backing this Model.</returns>
        public ContentValues GetData()
        {
            return _data.Clone();
        }

        /// <summary>
        /// Sets the date backing this Model.
        /// </summary>
        /// <param name="data">The data.</param>
        public void SetData(ContentValues data)
        {
            _data = data.Clone();
        }

        /// <summary>
        /// Raises the RecordChanged event for this Model.
        /// </summary>
        public void Changed()
        {
            RecordChanged(this, new EventArgs());
        }

        /// <summary>
        /// Raises the RecordDeleted event for this Model.
        /// </summary>
        public void Deleted()
        {
            RecordDeleted(this, new EventArgs());
        }
    }
}
