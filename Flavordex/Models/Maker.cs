using Flavordex.Models.Data;
using Flavordex.Utilities.Databases;

namespace Flavordex.Models
{
    /// <summary>
    /// Model representing a product maker.
    /// </summary>
    public class Maker : Model
    {
        /// <summary>
        /// The name of the maker.
        /// </summary>
        public string Name
        {
            get { return _data.GetString(Tables.Makers.NAME); }
            set { _data.SetString(Tables.Makers.NAME, value); }
        }

        /// <summary>
        /// The location of the maker.
        /// </summary>
        public string Location
        {
            get { return _data.GetString(Tables.Makers.LOCATION); }
            set { _data.SetString(Tables.Makers.LOCATION, value); }
        }
    }
}
