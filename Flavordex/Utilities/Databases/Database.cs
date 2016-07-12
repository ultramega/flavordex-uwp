using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flavordex.Utilities.Databases
{
    /// <summary>
    /// Wrapper for a SQLite connection that provides helpful methods for querying and data
    /// manipulation.
    /// </summary>
    public class Database : IDisposable
    {
        /// <summary>
        /// Whether this disposable object has been disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Gets or sets the database connection.
        /// </summary>
        public SQLiteConnection Connection { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the database file.</param>
        /// <param name="loader">The database connection callbacks.</param>
        public Database(string name, ISQLLoader loader = null)
        {
            Connection = new SQLiteConnection(name);

            if (loader != null)
            {
                var dbVersion = GetVersion();
                if (dbVersion == 0)
                {
                    loader.OnCreate(Connection);
                    SetVersion(loader.GetVersion());
                }
                else if (loader.GetVersion() > dbVersion)
                {
                    loader.OnUpgrade(Connection, dbVersion);
                    SetVersion(loader.GetVersion());
                }
            }
        }

        /// <summary>
        /// Gets the database structure version number.
        /// </summary>
        /// <returns>The database structure version number.</returns>
        public long GetVersion()
        {
            using (var stmt = Connection.Prepare("PRAGMA user_version;"))
            {
                if (SQLiteResult.ROW == stmt.Step())
                {
                    return (long)stmt[0];
                }
            }
            throw new SQLiteException("Failed to read the database version");
        }

        /// <summary>
        /// Sets the database structure version number.
        /// </summary>
        /// <param name="version">The database structure version number to set.</param>
        public void SetVersion(long version)
        {
            using (var stmt = Connection.Prepare(string.Format("PRAGMA user_version = {0};", version)))
            {
                stmt.Step();
            }
        }

        /// <summary>
        /// Queries the database.
        /// </summary>
        /// <param name="table">The table to query.</param>
        /// <returns>The results of the query.</returns>
        public async Task<ContentValues[]> Query(string table)
        {
            return await Query(table, null, null, null, null);
        }

        /// <summary>
        /// Queries the database.
        /// </summary>
        /// <param name="table">The table to query.</param>
        /// <param name="projection">The list of columns to fetch.</param>
        /// <returns>The results of the query.</returns>
        public async Task<ContentValues[]> Query(string table, string[] projection)
        {
            return await Query(table, projection, null, null, null);
        }

        /// <summary>
        /// Queries the database.
        /// </summary>
        /// <param name="table">The table to query.</param>
        /// <param name="projection">The list of columns to fetch.</param>
        /// <param name="where">The where clause.</param>
        /// <param name="whereArgs">The values for the parameters in the where clause.</param>
        /// <returns>The results of the query.</returns>
        public async Task<ContentValues[]> Query(string table, string[] projection, string where, object[] whereArgs)
        {
            return await Query(table, projection, where, whereArgs, null);
        }

        /// <summary>
        /// Queries the database.
        /// </summary>
        /// <param name="table">The table to query.</param>
        /// <param name="projection">The list of columns to fetch.</param>
        /// <param name="where">The where clause.</param>
        /// <param name="whereArgs">The values for the parameters in the where clause.</param>
        /// <param name="sort">The parameters of the order by clause.</param>
        /// <returns>The results of the query.</returns>
        public async Task<ContentValues[]> Query(string table, string[] projection, string where, object[] whereArgs, string sort)
        {
            return await Query(table, projection, where, whereArgs, sort, null);
        }

        /// <summary>
        /// Queries the database.
        /// </summary>
        /// <param name="table">The table to query.</param>
        /// <param name="projection">The list of columns to fetch.</param>
        /// <param name="where">The where clause.</param>
        /// <param name="whereArgs">The values for the parameters in the where clause.</param>
        /// <param name="sort">The parameters of the order by clause.</param>
        /// <param name="limit">The parameters of the limit clause.</param>
        /// <returns>The results of the query.</returns>
        public async Task<ContentValues[]> Query(string table, string[] projection, string where, object[] whereArgs, string sort, string limit)
        {
            return await Task.Run(() =>
            {
                var cols = projection == null || projection.Length == 0 ? "*" : string.Join(", ", projection);
                where = string.IsNullOrWhiteSpace(where) ? "" : " WHERE " + where;
                sort = string.IsNullOrWhiteSpace(sort) ? "" : " ORDER BY " + sort;
                limit = string.IsNullOrWhiteSpace(limit) ? "" : " LIMIT " + limit;
                var sql = string.Format("SELECT {0} FROM {1}{2}{3}{4};", cols, table, where, sort, limit);

                using (var stmt = Connection.Prepare(sql))
                {
                    if (whereArgs != null)
                    {
                        for (var i = 0; i < whereArgs.Length; i++)
                        {
                            stmt.Bind(i + 1, whereArgs[i]);
                        }
                    }

                    var results = new List<ContentValues>();
                    while (stmt.Step() == SQLiteResult.ROW)
                    {
                        var values = new ContentValues();
                        for (var i = 0; i < stmt.ColumnCount; i++)
                        {
                            values[stmt.ColumnName(i)] = stmt[i];
                        }
                        results.Add(values);
                    }
                    return results.ToArray();
                }
            });
        }

        /// <summary>
        /// Inserts a row into the table.
        /// </summary>
        /// <param name="table">The table to insert into.</param>
        /// <param name="values">The values for the columns of the table.</param>
        /// <returns>The ID of the inserted row, or -1 on failure.</returns>
        public async Task<long> Insert(string table, ContentValues values)
        {
            return await Task.Run(() =>
            {
                var cols = string.Join(", ", values.Keys);
                var vals = string.Join(", ", new string('?', values.Count).ToCharArray());
                var sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2});", table, cols, vals);

                using (var stmt = Connection.Prepare(sql))
                {
                    var i = 1;
                    foreach (var v in values.Values)
                    {
                        stmt.Bind(i++, v);
                    }
                    if (SQLiteResult.DONE == stmt.Step())
                    {
                        return Connection.LastInsertRowId();
                    }
                }
                return -1;
            });
        }

        /// <summary>
        /// Updates rows in the table.
        /// </summary>
        /// <param name="table">The table to update.</param>
        /// <param name="values">The values of the columns to update.</param>
        /// <param name="where">The where clause.</param>
        /// <param name="whereArgs">The values for the parameters in the where clause.</param>
        /// <returns>The number of rows modified.</returns>
        public async Task<int> Update(string table, ContentValues values, string where, object[] whereArgs)
        {
            return await Task.Run(() =>
            {
                if(whereArgs == null)
                {
                    whereArgs = new object[0];
                }
                var cols = string.Join(" = ?, ", values.Keys) + " = ?";
                var args = new object[values.Count + whereArgs.Length];
                values.Values.CopyTo(args, 0);
                where = string.IsNullOrWhiteSpace(where) ? "" : " WHERE " + where;
                if (whereArgs != null)
                {
                    whereArgs.CopyTo(args, values.Count);
                }
                var sql = string.Format("UPDATE {0} SET {1}{2};", table, cols, where);

                using (var stmt = Connection.Prepare(sql))
                {
                    var i = 1;
                    foreach (var v in args)
                    {
                        stmt.Bind(i++, v);
                    }
                    stmt.Step();
                    return Connection.ChangesCount();
                }
            });
        }

        /// <summary>
        /// Deletes rows from the table.
        /// </summary>
        /// <param name="table">The table to delete from.</param>
        /// <param name="where">The where clause.</param>
        /// <param name="whereArgs">The values for the parameters in the where clause.</param>
        /// <returns>The number of deleted rows.</returns>
        public async Task<int> Delete(string table, string where, object[] whereArgs)
        {
            return await Task.Run(() =>
            {
                where = string.IsNullOrWhiteSpace(where) ? "" : " WHERE " + where;
                var sql = string.Format("DELETE FROM {0}{1};", table, where);

                using (var stmt = Connection.Prepare(sql))
                {
                    if (whereArgs != null)
                    {
                        var i = 1;
                        foreach (var v in whereArgs)
                        {
                            stmt.Bind(i++, v);
                        }
                    }
                    stmt.Step();
                    return Connection.ChangesCount();
                }
            });
        }

        /// <summary>
        /// Disposes this disposable object.
        /// </summary>
        /// <param name="disposing">Whether to clean up managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Connection.Dispose();
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Disposes this disposable object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
    }
}
