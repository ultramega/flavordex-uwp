using Flavordex.Models.Data;
using Flavordex.Utilities.Databases;

namespace Flavordex.Models
{
    /// <summary>
    /// Model representing a flavor value.
    /// </summary>
    public class EntryFlavor : Model
    {
        /// <summary>
        /// The primary ID of the journal entry this flavor is associated with.
        /// </summary>
        public long EntryID
        {
            get { return _data.GetLong(Tables.EntriesFlavors.ENTRY); }
            set { _data.SetLong(Tables.EntriesFlavors.ENTRY, value); }
        }

        /// <summary>
        /// The name of the flavor.
        /// </summary>
        public string Name
        {
            get { return _data.GetString(Tables.EntriesFlavors.FLAVOR); }
            set { _data.SetString(Tables.EntriesFlavors.FLAVOR, value); }
        }

        /// <summary>
        /// The value of the flavor.
        /// </summary>
        public long Value
        {
            get { return _data.GetLong(Tables.EntriesFlavors.VALUE); }
            set { _data.SetLong(Tables.EntriesFlavors.VALUE, value); }
        }

        /// <summary>
        /// The sorting position in the list of flavors.
        /// </summary>
        public long Position
        {
            get { return _data.GetLong(Tables.EntriesFlavors.POS); }
            set { _data.SetLong(Tables.EntriesFlavors.POS, value); }
        }
    }
}
