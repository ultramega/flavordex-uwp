using Flavordex.Models.Data;
using Flavordex.Utilities.Databases;

namespace Flavordex.Models
{
    /// <summary>
    /// Model representing a flavor.
    /// </summary>
    public class Flavor : Model
    {
        /// <summary>
        /// The primary ID of the category this flavor is associated with.
        /// </summary>
        public long CategoryID
        {
            get { return _data.GetLong(Tables.Flavors.CAT); }
            set { _data.SetLong(Tables.Flavors.CAT, value); }
        }

        /// <summary>
        /// The name of the flavor.
        /// </summary>
        public string Name
        {
            get { return _data.GetString(Tables.Flavors.NAME); }
            set { _data.SetString(Tables.Flavors.NAME, value); }
        }

        /// <summary>
        /// The sorting position in the list of flavors.
        /// </summary>
        public long Position
        {
            get { return _data.GetLong(Tables.Flavors.POS); }
            set { _data.SetLong(Tables.Flavors.POS, value); }
        }
    }
}
