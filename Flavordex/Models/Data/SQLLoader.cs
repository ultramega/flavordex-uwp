using Flavordex.Utilities.Databases;
using SQLitePCL;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Resources;
using Windows.Storage;

namespace Flavordex.Models.Data
{
    /// <summary>
    /// The loader for the application.
    /// </summary>
    class SQLLoader : ISQLLoader
    {
        /// <summary>
        /// The database structure version.
        /// </summary>
        public static readonly long VERSION = 1;

        /// <summary>
        /// The database schema files.
        /// </summary>
        private static string Schema { get { return GetSQL("schema"); } }
        private static string Views { get { return GetSQL("views"); } }
        private static string Triggers { get { return GetSQL("triggers"); } }
        private static string TestData { get { return GetSQL("testdata"); } }

        /// <summary>
        /// Gets the current version of the schema.
        /// </summary>
        /// <returns>The current schema version.</returns>
        public long GetVersion()
        {
            return VERSION;
        }

        /// <summary>
        /// Creates the database from the schema files and adds preset values.
        /// </summary>
        /// <param name="conn">The database connection.</param>
        public void OnCreate(SQLiteConnection conn)
        {
            var delim = "\n--";
            var stmts = new ArrayList();
            stmts.AddRange(Regex.Split(Schema, delim));
            stmts.AddRange(Regex.Split(Views, delim));
            stmts.AddRange(Regex.Split(Triggers, delim));
            foreach (string sql in stmts)
            {
                conn.Prepare(sql).Step();
            }

            InsertBeerPreset(conn);
            InsertWinePreset(conn);
            InsertWhiskeyPreset(conn);
            InsertCoffeePreset(conn);

#if DEBUG
            foreach (var sql in Regex.Split(TestData, delim))
            {
                conn.Prepare(sql).Step();
            }
#endif
        }

        /// <summary>
        /// Upgrades the schema to the current version.
        /// </summary>
        /// <param name="conn">The database connection.</param>
        /// <param name="oldVersion">The version being upgraded from.</param>
        public void OnUpgrade(SQLiteConnection conn, long oldVersion) { }

        /// <summary>
        /// Load SQL from the assets folder.
        /// </summary>
        /// <param name="name">The base name of the schema file.</param>
        /// <returns>The text from the file.</returns>
        private static string GetSQL(string name)
        {
            var uri = new Uri("ms-appx:///Assets/SQL/" + name + ".sql");

            var file = StorageFile.GetFileFromApplicationUriAsync(uri).AsTask()
                .ConfigureAwait(false).GetAwaiter().GetResult();
            return FileIO.ReadTextAsync(file).AsTask().ConfigureAwait(false).GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Insert presets for beer entries.
        /// </summary>
        /// <param name="conn">An open database connection.</param>
        private static void InsertBeerPreset(SQLiteConnection conn)
        {
            var extras = new string[]
            {
                Tables.Extras.Beer.STYLE,
                Tables.Extras.Beer.SERVING,
                Tables.Extras.Beer.STATS_IBU,
                Tables.Extras.Beer.STATS_ABV,
                Tables.Extras.Beer.STATS_OG,
                Tables.Extras.Beer.STATS_FG
            };
            var flavors = ResourceLoader.GetForViewIndependentUse("Beer").GetString("FlavorNames")
                .Split(';');
            InsertPreset(conn, Constants.CAT_BEER, extras, flavors);
        }

        /// <summary>
        /// Insert presets for wine entries.
        /// </summary>
        /// <param name="conn">An open database connection.</param>
        private static void InsertWinePreset(SQLiteConnection conn)
        {
            var extras = new string[]
            {
                Tables.Extras.Wine.VARIETAL,
                Tables.Extras.Wine.STATS_VINTAGE,
                Tables.Extras.Wine.STATS_ABV
            };
            var flavors = ResourceLoader.GetForViewIndependentUse("Wine").GetString("FlavorNames")
                .Split(';');
            InsertPreset(conn, Constants.CAT_WINE, extras, flavors);
        }

        /// <summary>
        /// Insert presets for whiskey entries.
        /// </summary>
        /// <param name="conn">An open database connection.</param>
        private static void InsertWhiskeyPreset(SQLiteConnection conn)
        {
            var extras = new string[]
            {
                Tables.Extras.Whiskey.STYLE,
                Tables.Extras.Whiskey.STATS_AGE,
                Tables.Extras.Whiskey.STATS_ABV
            };
            var flavors = ResourceLoader.GetForViewIndependentUse("Whiskey")
                .GetString("FlavorNames").Split(';');
            InsertPreset(conn, Constants.CAT_WHISKEY, extras, flavors);
        }

        /// <summary>
        /// Insert presets for coffee entries.
        /// </summary>
        /// <param name="conn">An open database connection.</param>
        private static void InsertCoffeePreset(SQLiteConnection conn)
        {
            var extras = new string[]
            {
                Tables.Extras.Coffee.ROASTER,
                Tables.Extras.Coffee.ROAST_DATE,
                Tables.Extras.Coffee.GRIND,
                Tables.Extras.Coffee.BREW_METHOD,
                Tables.Extras.Coffee.STATS_DOSE,
                Tables.Extras.Coffee.STATS_MASS,
                Tables.Extras.Coffee.STATS_TEMP,
                Tables.Extras.Coffee.STATS_EXTIME,
                Tables.Extras.Coffee.STATS_TDS,
                Tables.Extras.Coffee.STATS_YIELD
            };
            var flavors = ResourceLoader.GetForViewIndependentUse("Coffee")
                .GetString("FlavorNames").Split(';');
            InsertPreset(conn, Constants.CAT_COFFEE, extras, flavors);
        }

        /// <summary>
        /// Insert presets for category's entries.
        /// </summary>
        /// <param name="conn">An open database connection.</param>
        /// <param name="name">The internal name of the category.</param>
        /// <param name="extras">The list of internal extra field names.</param>
        /// <param name="flavors">The list of flavor names.</param>
        private static void InsertPreset(SQLiteConnection conn, string name, string[] extras,
            string[] flavors)
        {
            var sql = string.Format("INSERT INTO {0} ({1}, {2}, {3}) VALUES (?, ?, 1);",
                Tables.Cats.TABLE_NAME, Tables.Cats.UUID, Tables.Cats.NAME, Tables.Cats.PRESET);
            using (var stmt = conn.Prepare(sql))
            {
                stmt.Bind(1, name);
                stmt.Bind(2, name);
                stmt.Step();
            }
            long id = conn.LastInsertRowId();

            sql = string.Format(
                "INSERT INTO {0} ({1}, {2}, {3}, {4}, {5}) VALUES (?, ?, ?, ?, 1);",
                Tables.Extras.TABLE_NAME, Tables.Extras.UUID, Tables.Extras.CAT,
                Tables.Extras.NAME, Tables.Extras.POS, Tables.Extras.PRESET);
            using (var stmt = conn.Prepare(sql))
            {
                stmt.Bind(2, id);
                for (var i = 0; i < extras.Length; i++)
                {
                    stmt.Bind(1, name + extras[i]);
                    stmt.Bind(3, extras[i]);
                    stmt.Bind(4, i);
                    stmt.Step();
                    stmt.Reset();
                }
            }

            sql = string.Format("INSERT INTO {0} ({1}, {2}, {3}) VALUES (?, ?, ?);",
                Tables.Flavors.TABLE_NAME, Tables.Flavors.CAT, Tables.Flavors.NAME,
                Tables.Flavors.POS);
            using (var stmt = conn.Prepare(sql))
            {
                stmt.Bind(1, id);
                for (var i = 0; i < flavors.Length; i++)
                {
                    stmt.Bind(2, flavors[i]);
                    stmt.Bind(3, i);
                    stmt.Step();
                    stmt.Reset();
                }
            }
        }
    }
}
