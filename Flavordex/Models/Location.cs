using Flavordex.Models.Data;
using Flavordex.Utilities.Databases;

namespace Flavordex.Models
{
    /// <summary>
    /// Model representing a tasting location.
    /// </summary>
    public class Location : Model
    {
        /// <summary>
        /// The location's latitude.
        /// </summary>
        public double Latitude
        {
            get { return _data.GetDouble(Tables.Locations.LATITUDE); }
            set { _data.SetDouble(Tables.Locations.LATITUDE, value); }
        }

        /// <summary>
        /// The location's longitude.
        /// </summary>
        public double Longitude
        {
            get { return _data.GetDouble(Tables.Locations.LONGITUDE); }
            set { _data.SetDouble(Tables.Locations.LONGITUDE, value); }
        }

        /// <summary>
        /// The name associated with the location.
        /// </summary>
        public string Name
        {
            get { return _data.GetString(Tables.Locations.NAME); }
            set { _data.SetString(Tables.Locations.NAME, value); }
        }
    }
}
