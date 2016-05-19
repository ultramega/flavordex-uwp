using SQLitePCL;

namespace Flavordex.Utilities.Databases
{
    /// <summary>
    /// Callbacks for the Database object to call when opening a database connection. This is used
    /// to initialize and update the database structure.
    /// </summary>
    public interface ISQLLoader
    {
        /// <summary>
        /// Get the version number of the database structure.
        /// </summary>
        /// <returns>The database structure version.</returns>
        long GetVersion();

        /// <summary>
        /// Called when the database has not been initialized. This is where the database structure
        /// should be created.
        /// </summary>
        /// <param name="conn">An open database connection.</param>
        void OnCreate(SQLiteConnection conn);

        /// <summary>
        /// Called when the database structure needs to be updated.
        /// </summary>
        /// <param name="conn">An open database connection.</param>
        /// <param name="oldVersion">The old database structure version.</param>
        void OnUpgrade(SQLiteConnection conn, long oldVersion);
    }
}
