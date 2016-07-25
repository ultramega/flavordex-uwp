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
using Flavordex.Utilities.Databases;

namespace Flavordex.Models.Data
{
    /// <summary>
    /// Constants for accessing database records.
    /// </summary>
    public class Tables
    {
        /// <summary>
        /// Data contract for the 'entries' table and view.
        /// </summary>
        public class Entries : BaseColumns
        {
            /// <summary>
            /// Table names.
            /// </summary>
            public const string TABLE_NAME = "entries";
            public const string VIEW_NAME = "view_entry";

            /// <summary>
            /// Column names.
            /// </summary>
            public const string UUID = "uuid";
            public const string TITLE = "title";
            public const string CAT_ID = "cat_id";
            public const string CAT_UUID = "cat_uuid";
            public const string CAT = "cat";
            public const string MAKER_ID = "maker_id";
            public const string MAKER = "maker";
            public const string ORIGIN = "origin";
            public const string PRICE = "price";
            public const string LOCATION = "location";
            public const string DATE = "date";
            public const string RATING = "rating";
            public const string NOTES = "notes";
            public const string UPDATED = "updated";
            public const string PUBLISHED = "published";
            public const string SYNCED = "synced";
        }

        /// <summary>
        /// Data contract for the 'entries_extras' table and view.
        /// </summary>
        public class EntriesExtras : BaseColumns
        {
            /// <summary>
            /// Table names.
            /// </summary>
            public const string TABLE_NAME = "entries_extras";
            public const string VIEW_NAME = "view_entry_extra";

            /// <summary>
            /// Column names.
            /// </summary>
            public const string ENTRY = "entry";
            public const string EXTRA = "extra";
            public const string VALUE = "value";
        }

        /// <summary>
        /// Data contract for the 'entries_flavors' table.
        /// </summary>
        public class EntriesFlavors : BaseColumns
        {
            /// <summary>
            /// Table names.
            /// </summary>
            public const string TABLE_NAME = "entries_flavors";

            /// <summary>
            /// Column names.
            /// </summary>
            public const string ENTRY = "entry";
            public const string FLAVOR = "flavor";
            public const string VALUE = "value";
            public const string POS = "pos";
        }

        /// <summary>
        /// Data contract for the 'extras' table.
        /// </summary>
        public class Extras : BaseColumns
        {
            /// <summary>
            /// Table names.
            /// </summary>
            public const string TABLE_NAME = "extras";

            /// <summary>
            /// Column names.
            /// </summary>
            public const string UUID = "uuid";
            public const string CAT = "cat";
            public const string NAME = "name";
            public const string POS = "pos";
            public const string PRESET = "preset";
            public const string DELETED = "deleted";

            /// <summary>
            /// Beer preset extra names.
            /// </summary>
            public class Beer
            {
                public const string STYLE = "_style";
                public const string SERVING = "_serving";
                public const string STATS_IBU = "_stats_ibu";
                public const string STATS_ABV = "_stats_abv";
                public const string STATS_OG = "_stats_og";
                public const string STATS_FG = "_stats_fg";
            }

            /// <summary>
            /// Wine preset extra names.
            /// </summary>
            public class Wine
            {
                public const string VARIETAL = "_varietal";
                public const string STATS_VINTAGE = "_stats_vintage";
                public const string STATS_ABV = "_stats_abv";
            }

            /// <summary>
            /// Whiskey preset extra names.
            /// </summary>
            public class Whiskey
            {
                public const string STYLE = "_style";
                public const string STATS_AGE = "_stats_age";
                public const string STATS_ABV = "_stats_abv";
            }

            /// <summary>
            /// Coffee preset extra names.
            /// </summary>
            public class Coffee
            {
                public const string ROASTER = "_roaster";
                public const string ROAST_DATE = "_roast_date";
                public const string GRIND = "_grind";
                public const string BREW_METHOD = "_brew_method";
                public const string STATS_DOSE = "_stats_dose";
                public const string STATS_MASS = "_stats_mass";
                public const string STATS_TEMP = "_stats_temp";
                public const string STATS_EXTIME = "_stats_extime";
                public const string STATS_TDS = "_stats_tds";
                public const string STATS_YIELD = "_stats_yield";
            }
        }

        /// <summary>
        /// Data contract for the 'flavors' table.
        /// </summary>
        public class Flavors : BaseColumns
        {
            /// <summary>
            /// Table names.
            /// </summary>
            public const string TABLE_NAME = "flavors";

            /// <summary>
            /// Column names.
            /// </summary>
            public const string CAT = "cat";
            public const string NAME = "name";
            public const string POS = "pos";
        }

        /// <summary>
        /// Data contract for the 'makers' table.
        /// </summary>
        public class Makers : BaseColumns
        {
            /// <summary>
            /// Table names.
            /// </summary>
            public const string TABLE_NAME = "makers";

            /// <summary>
            /// Column names.
            /// </summary>
            public const string NAME = "name";
            public const string LOCATION = "location";
        }

        /// <summary>
        /// Data contract for the 'photos' table.
        /// </summary>
        public class Photos : BaseColumns
        {
            /// <summary>
            /// Table names.
            /// </summary>
            public const string TABLE_NAME = "photos";

            /// <summary>
            /// Column names.
            /// </summary>
            public const string ENTRY = "entry";
            public const string HASH = "hash";
            public const string PATH = "path";
            public const string DRIVE_ID = "drive_id";
            public const string POS = "pos";
        }

        /// <summary>
        /// Data contract for the 'locations' table.
        /// </summary>
        public class Locations : BaseColumns
        {
            /// <summary>
            /// Table names.
            /// </summary>
            public const string TABLE_NAME = "locations";

            /// <summary>
            /// Column names.
            /// </summary>
            public const string LATITUDE = "lat";
            public const string LONGITUDE = "lon";
            public const string NAME = "name";
        }

        /// <summary>
        /// Data contract for the 'cats' table and view.
        /// </summary>
        public class Cats : BaseColumns
        {
            /// <summary>
            /// Table names.
            /// </summary>
            public const string TABLE_NAME = "cats";
            public const string VIEW_NAME = "view_cat";

            /// <summary>
            /// Column names.
            /// </summary>
            public const string UUID = "uuid";
            public const string NAME = "name";
            public const string PRESET = "preset";
            public const string UPDATED = "updated";
            public const string PUBLISHED = "published";
            public const string SYNCED = "synced";
            public const string NUM_ENTRIES = "num_entries";
        }

        /// <summary>
        /// Data contract for the 'deleted' table.
        /// </summary>
        public class Deleted : BaseColumns
        {
            /// <summary>
            /// Table names.
            /// </summary>
            public const string TABLE_NAME = "deleted";

            /// <summary>
            /// Column names.
            /// </summary>
            public const string TYPE = "type";
            public const string CAT = "cat";
            public const string UUID = "uuid";
            public const string TIME = "time";

            /// <summary>
            /// Values for the 'type' column.
            /// </summary>
            public const int TYPE_CAT = 0;
            public const int TYPE_ENTRY = 1;
            public const int TYPE_PHOTO = 2;
        }
    }
}
