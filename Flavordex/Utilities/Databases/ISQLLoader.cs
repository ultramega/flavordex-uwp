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
