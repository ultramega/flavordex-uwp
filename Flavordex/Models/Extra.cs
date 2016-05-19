using Flavordex.Models.Data;
using Flavordex.Utilities.Databases;

namespace Flavordex.Models
{
    /// <summary>
    /// Model representing an extra field.
    /// </summary>
    public class Extra : Model
    {
        /// <summary>
        /// The globally unique identifier string.
        /// </summary>
        public string UUID
        {
            get { return _data.GetString(Tables.Extras.UUID); }
            set { _data.SetString(Tables.Extras.UUID, value); }
        }

        /// <summary>
        /// The primary ID of the category this extra field is associated with.
        /// </summary>
        public long CategoryID
        {
            get { return _data.GetLong(Tables.Extras.CAT); }
            set { _data.SetLong(Tables.Extras.CAT, value); }
        }

        /// <summary>
        /// The name of the extra field.
        /// </summary>
        public string Name
        {
            get { return _data.GetString(Tables.Extras.NAME); }
            set { _data.SetString(Tables.Extras.NAME, value); }
        }

        /// <summary>
        /// The sorting position in the list of fields.
        /// </summary>
        public long Position
        {
            get { return _data.GetLong(Tables.Extras.POS); }
            set { _data.SetLong(Tables.Extras.POS, value); }
        }

        /// <summary>
        /// Whether this is a preset field.
        /// </summary>
        public bool IsPreset
        {
            get { return _data.GetBool(Tables.Extras.PRESET); }
            set { _data.SetBool(Tables.Extras.PRESET, value); }
        }

        /// <summary>
        /// Whether this extra field has been deleted from the category.
        /// </summary>
        public bool IsDeleted
        {
            get { return _data.GetBool(Tables.Extras.DELETED); }
            set { _data.SetBool(Tables.Extras.DELETED, value); }
        }
    }
}
