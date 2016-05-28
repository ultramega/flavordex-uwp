using Flavordex.Utilities.Databases;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Flavordex.Models.Data
{
    /// <summary>
    /// Wrapper for the Database, providing various helper methods.
    /// </summary>
    public class DatabaseHelper
    {
        /// <summary>
        /// Occurs when a record changes in the database.
        /// </summary>
        public static event RecordChangedEventHandler RecordChanged = delegate { };

        /// <summary>
        /// Handler for the RecordChanged event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        public delegate void RecordChangedEventHandler(object sender, RecordChangedEventArgs e);

        /// <summary>
        /// Cache of Entry Models.
        /// </summary>
        private static ModelCache<Entry> _entryCache = new ModelCache<Entry>();

        /// <summary>
        /// Cache of Category Models.
        /// </summary>
        private static ModelCache<Category> _categoryCache = new ModelCache<Category>();

        /// <summary>
        /// The name of the database file to use.
        /// </summary>
        private const string _databaseName = "flavordex.db";

        /// <summary>
        /// The Database.
        /// </summary>
        private static Database _database = new Database(_databaseName, new SQLLoader());

        /// <summary>
        /// Gets the Database.
        /// </summary>
        private static Database Database
        {
            get
            {
                if (_database == null)
                {
                    _database = new Database(_databaseName);
                }
                return _database;
            }
        }

        /// <summary>
        /// Closes the connection to the database.
        /// </summary>
        public static void Close()
        {
            _database.Dispose();
            _database = null;
        }

        /// <summary>
        /// Gets a list of all journal entries from one or all categories.
        /// </summary>
        /// <param name="categoryId">The primary ID of the category; 0 for all categories.</param>
        /// <returns>A Collection of Entries.</returns>
        public static async Task<Collection<Entry>> GetEntryListAsync(long categoryId)
        {
            var list = new Collection<Entry>();

            var projection = new string[]
            {
                BaseColumns._ID,
                Tables.Entries.TITLE,
                Tables.Entries.MAKER,
                Tables.Entries.ORIGIN,
                Tables.Entries.LOCATION,
                Tables.Entries.RATING,
                Tables.Entries.DATE
            };
            var where = categoryId > 0 ? Tables.Entries.CAT_ID + " = ?" : null;
            var whereArgs = categoryId > 0 ? new object[] { categoryId } : null;
            var rows = await Database.Query(Tables.Entries.VIEW_NAME, projection, where, whereArgs);
            foreach (var row in rows)
            {
                list.Add(_entryCache.Get(row));
            }

            return list;
        }

        /// <summary>
        /// Gets a journal entry from the database.
        /// </summary>
        /// <param name="entryId">The primary ID of the journal entry.</param>
        /// <returns>The Entry.</returns>
        public static async Task<Entry> GetEntryAsync(long entryId)
        {
            var where = BaseColumns._ID + " = ?";
            var whereArgs = new object[] { entryId };
            var rows = await Database.Query(Tables.Entries.VIEW_NAME, null, where, whereArgs);
            if (rows.Length > 0)
            {
                return _entryCache.Get(rows[0]);
            }

            return null;
        }

        /// <summary>
        /// Gets the extra fields for a journal entry.
        /// </summary>
        /// <param name="entryId">The primary ID of the journal entry.</param>
        /// <returns>A Collection of EntryExtras representing the flavors.</returns>
        public static async Task<Collection<EntryExtra>> GetEntryExtrasAsync(long entryId)
        {
            var list = new Collection<EntryExtra>();

            var where = Tables.EntriesExtras.ENTRY + " = ?";
            var whereArgs = new object[] { entryId };
            var sort = Tables.Extras.POS;
            var rows = await Database.Query(Tables.EntriesExtras.VIEW_NAME, null, where, whereArgs, sort);
            foreach (var row in rows)
            {
                var item = new EntryExtra();
                item.SetData(row);
                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// Gets the flavors for a journal entry.
        /// </summary>
        /// <param name="entryId">The primary ID of the journal entry.</param>
        /// <returns>A Collection of EntryFlavors representing the flavors.</returns>
        public static async Task<Collection<EntryFlavor>> GetEntryFlavorsAsync(long entryId)
        {
            var list = new Collection<EntryFlavor>();

            var where = Tables.EntriesFlavors.ENTRY + " = ?";
            var whereArgs = new object[] { entryId };
            var sort = Tables.EntriesFlavors.POS;
            var rows = await Database.Query(Tables.EntriesFlavors.TABLE_NAME, null, where, whereArgs, sort);
            foreach (var row in rows)
            {
                var item = new EntryFlavor();
                item.SetData(row);
                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// Gets the photos for a journal entry.
        /// </summary>
        /// <param name="entryId">The primary ID of the journal entry.</param>
        /// <returns>A Collection of Photos representing the photos.</returns>
        public static async Task<Collection<Photo>> GetEntryPhotosAsync(long entryId)
        {
            var list = new Collection<Photo>();

            var where = Tables.Photos.ENTRY + " = ?";
            var whereArgs = new object[] { entryId };
            var sort = Tables.Photos.POS;
            var rows = await Database.Query(Tables.Photos.TABLE_NAME, null, where, whereArgs, sort);
            foreach (var row in rows)
            {
                var item = new Photo();
                item.SetData(row);
                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// Gets the poster photo for a journal entry.
        /// </summary>
        /// <param name="entryId">The primary ID of the journal entry.</param>
        /// <returns>The path to the poster photo.</returns>
        public static async Task<string> GetPosterPhotoAsync(long entryId)
        {
            var projection = new string[]
            {
                Tables.Photos.PATH
            };
            var where = Tables.Photos.ENTRY + " = ?";
            var whereArgs = new object[] { entryId };
            var rows = await Database.Query(Tables.Photos.TABLE_NAME, projection, where, whereArgs, Tables.Photos.POS, "1");
            if (rows.Length > 0)
            {
                return rows[0].GetString(Tables.Photos.PATH);
            }

            return null;
        }

        /// <summary>
        /// Updates or inserts a journal entry in the database.
        /// </summary>
        /// <param name="entry">The Entry to update or insert.</param>
        /// <param name="extras">The extra fields for the journal entry.</param>
        /// <returns>Whether the update was successful.</returns>
        public static async Task<bool> UpdateEntryAsync(Entry entry, Collection<EntryExtra> extras)
        {
            var action = entry.ID == 0 ? RecordChangedAction.Insert : RecordChangedAction.Update;

            entry.Updated = DateTime.Now;
            entry.IsSynced = false;
            if (string.IsNullOrEmpty(entry.UUID))
            {
                entry.UUID = Guid.NewGuid().ToString();
            }

            var values = entry.GetData();

            await GetMakerIDAsync(values);

            values.SetLong(Tables.Entries.CAT, values.GetLong(Tables.Entries.CAT_ID));
            values.Remove(Tables.Entries.CAT_ID);
            values.Remove(Tables.Entries.CAT_UUID);

            if (action == RecordChangedAction.Update)
            {
                await Database.Update(Tables.Entries.TABLE_NAME, values, BaseColumns._ID + " = ?", new object[] { entry.ID });
            }
            else
            {
                entry.ID = await Database.Insert(Tables.Entries.TABLE_NAME, values);
            }

            if (entry.ID > 0)
            {
                await UpdateEntryExtrasAsync(entry.ID, extras);

                entry.Changed();
                RecordChanged(null, new RecordChangedEventArgs(action, entry));

                return true;
            }

            return false;
        }

        /// <summary>
        /// Updates the Extras for a journal entry.
        /// </summary>
        /// <param name="entryId">The primary ID of the journal entry.</param>
        /// <param name="extras">The list of EntryExtras.</param>
        private static async Task UpdateEntryExtrasAsync(long entryId, Collection<EntryExtra> extras)
        {
            await Database.Delete(Tables.EntriesExtras.TABLE_NAME, Tables.EntriesExtras.ENTRY + " = ?", new object[] { entryId });

            var values = new ContentValues();
            values.SetLong(Tables.EntriesExtras.ENTRY, entryId);
            foreach (var extra in extras)
            {
                values.SetLong(Tables.EntriesExtras.EXTRA, extra.ID);
                values.SetString(Tables.EntriesExtras.VALUE, extra.Value);
                await Database.Insert(Tables.EntriesExtras.TABLE_NAME, values);
            }
        }

        /// <summary>
        /// Updates the Flavors for a journal entry.
        /// </summary>
        /// <param name="entryId">The primary ID of the journal entry.</param>
        /// <param name="flavors">The list of EntryFlavors.</param>
        public static async Task UpdateEntryFlavorsAsync(long entryId, Collection<EntryFlavor> flavors)
        {
            await Database.Delete(Tables.EntriesFlavors.TABLE_NAME, Tables.EntriesFlavors.ENTRY + " = ?", new object[] { entryId });

            var position = 0;
            foreach (var flavor in flavors)
            {
                flavor.EntryID = entryId;
                flavor.Position = position++;
                await Database.Insert(Tables.EntriesFlavors.TABLE_NAME, flavor.GetData());
            }

            _entryCache.Changed(entryId);
        }

        /// <summary>
        /// Inserts a Photo into the database.
        /// </summary>
        /// <param name="photo">The Photo to insert.</param>
        /// <returns>Whether the Photo was successfully added.</returns>
        public static async Task<bool> InsertPhotoAsync(Photo photo)
        {
            photo.ID = await Database.Insert(Tables.Photos.TABLE_NAME, photo.GetData());
            RecordChanged(null, new RecordChangedEventArgs(RecordChangedAction.Insert, photo));
            photo.Changed();
            _entryCache.Changed(photo.EntryID);
            return photo.ID > 0;
        }

        /// <summary>
        /// Deletes a photo from the database.
        /// </summary>
        /// <param name="photo">The Photo.</param>
        /// <returns>Whether the Photo was successfully deleted.</returns>
        public static async Task<bool> DeletePhotoAsync(Photo photo)
        {
            if (await Database.Delete(Tables.Photos.TABLE_NAME, BaseColumns._ID + " = ?", new object[] { photo.ID }) > 0)
            {
                RecordChanged(null, new RecordChangedEventArgs(RecordChangedAction.Delete, photo));
                photo.Deleted();
                _entryCache.Changed(photo.EntryID);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get the primary ID of a Maker, creating one if necessary.
        /// </summary>
        /// <param name="values">Data from an Entry.</param>
        private static async Task GetMakerIDAsync(ContentValues values)
        {
            var name = values.GetString(Tables.Entries.MAKER, "");
            var location = values.GetString(Tables.Entries.ORIGIN, "");
            values.Remove(Tables.Entries.MAKER_ID);
            values.Remove(Tables.Entries.ORIGIN);

            var projection = new string[] { BaseColumns._ID };
            var where = Tables.Makers.NAME + " = ? AND " + Tables.Makers.LOCATION + " = ?";
            var whereArgs = new object[] { name, location };
            var rows = await Database.Query(Tables.Makers.TABLE_NAME, projection, where, whereArgs);
            if (rows.Length > 0)
            {
                values.SetLong(Tables.Entries.MAKER, rows[0].GetLong(BaseColumns._ID));
            }
            else
            {
                var newMaker = new ContentValues()
                {
                    { Tables.Makers.NAME, name },
                    { Tables.Makers.LOCATION, location }
                };
                values.SetLong(Tables.Entries.MAKER, await Database.Insert(Tables.Makers.TABLE_NAME, newMaker));
            }
        }

        /// <summary>
        /// Deletes a journal entry from the database.
        /// </summary>
        /// <param name="entry">The journal entry.</param>
        /// <returns>Whether the journal entry was successfully deleted.</returns>
        public static async Task<bool> DeleteEntryAsync(Entry entry)
        {
            if (await Database.Delete(Tables.Entries.TABLE_NAME, BaseColumns._ID + " = ?", new object[] { entry.ID }) > 0)
            {
                entry.Deleted();
                RecordChanged(null, new RecordChangedEventArgs(RecordChangedAction.Delete, entry));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the list of categories.
        /// </summary>
        /// <returns>A Collection of Categories.</returns>
        public static async Task<Collection<Category>> GetCategoryListAsync()
        {
            var list = new Collection<Category>();

            var rows = await Database.Query(Tables.Cats.VIEW_NAME);
            foreach (var row in rows)
            {
                list.Add(_categoryCache.Get(row));
            }

            return list;
        }

        /// <summary>
        /// Gets a category from the database.
        /// </summary>
        /// <param name="categoryId">The primary ID of the category.</param>
        /// <returns>The Category.</returns>
        public static async Task<Category> GetCategoryAsync(long categoryId)
        {
            var where = BaseColumns._ID + " = ?";
            var whereArgs = new object[] { categoryId };
            var rows = await Database.Query(Tables.Cats.VIEW_NAME, null, where, whereArgs);
            foreach (var row in rows)
            {
                return _categoryCache.Get(row);
            }

            return null;
        }

        /// <summary>
        /// Gets the extra fields for a category.
        /// </summary>
        /// <param name="categoryId">The primary ID of the category.</param>
        public static async Task<Collection<Extra>> GetCategoryExtrasAsync(long categoryId)
        {
            var list = new Collection<Extra>();

            var where = Tables.Extras.CAT + " = ?";
            var whereArgs = new object[] { categoryId };
            var sort = Tables.Extras.POS;
            var rows = await Database.Query(Tables.Extras.TABLE_NAME, null, where, whereArgs, sort);
            foreach (var row in rows)
            {
                var item = new Extra();
                item.SetData(row);
                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// Gets the flavors for a category.
        /// </summary>
        /// <param name="categoryId">The primary ID of the category.</param>
        /// <returns>The list of Flavors for the Category.</returns>
        public static async Task<Collection<Flavor>> GetCategoryFlavorsAsync(long categoryId)
        {
            var list = new Collection<Flavor>();

            var where = Tables.Flavors.CAT + " = ?";
            var whereArgs = new object[] { categoryId };
            var sort = Tables.Flavors.POS;
            var rows = await Database.Query(Tables.Flavors.TABLE_NAME, null, where, whereArgs, sort);
            foreach (var row in rows)
            {
                var item = new Flavor();
                item.SetData(row);
                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// Updates or inserts a Category in the database.
        /// </summary>
        /// <param name="category">The Category to update or insert.</param>
        /// <param name="extras">The extra fields for the category.</param>
        /// <param name="flavors">The flavors for the category.</param>
        /// <returns>Whether the update was successful.</returns>
        public static async Task<bool> UpdateCategoryAsync(Category category, Collection<Extra> extras, Collection<Flavor> flavors)
        {
            var action = category.ID == 0 ? RecordChangedAction.Insert : RecordChangedAction.Update;
            category.Updated = DateTime.Now;
            category.IsSynced = false;

            var values = category.GetData();
            values.Remove(Tables.Cats.NUM_ENTRIES);

            if (string.IsNullOrEmpty(category.UUID))
            {
                category.UUID = Guid.NewGuid().ToString();
            }

            if (action == RecordChangedAction.Update)
            {
                await Database.Update(Tables.Cats.TABLE_NAME, values, BaseColumns._ID + " = ?", new object[] { category.ID });
            }
            else
            {
                category.ID = await Database.Insert(Tables.Cats.TABLE_NAME, values);
            }

            if (category.ID > 0)
            {
                await UpdateCategoryExtrasAsync(category.ID, extras);
                await UpdateCategoryFlavorsAsync(category.ID, flavors);

                category.Changed();
                RecordChanged(null, new RecordChangedEventArgs(action, category));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Updates the extra fields for a category.
        /// </summary>
        /// <param name="categoryId">The primary ID of the category.</param>
        /// <param name="extras">The list of Extras.</param>
        private static async Task UpdateCategoryExtrasAsync(long categoryId, Collection<Extra> extras)
        {
            foreach (var extra in extras)
            {
                if (string.IsNullOrEmpty(extra.UUID))
                {
                    extra.UUID = Guid.NewGuid().ToString();
                }
                if (extra.ID > 0)
                {
                    if (extra.IsDeleted)
                    {
                        await Database.Delete(Tables.Extras.TABLE_NAME, BaseColumns._ID + " = ?", new object[] { extra.ID });
                    }
                    else
                    {
                        await Database.Update(Tables.Extras.TABLE_NAME, extra.GetData(), BaseColumns._ID + " = ?", new object[] { extra.ID });
                    }
                }
                else
                {
                    extra.CategoryID = categoryId;
                    await Database.Insert(Tables.Extras.TABLE_NAME, extra.GetData());
                }
            }
        }

        /// <summary>
        /// Updates the flavors for a category.
        /// </summary>
        /// <param name="categoryId">The primary ID of the category.</param>
        /// <param name="flavors">The list of Flavors.</param>
        private static async Task UpdateCategoryFlavorsAsync(long categoryId, Collection<Flavor> flavors)
        {
            await Database.Delete(Tables.Flavors.TABLE_NAME, Tables.Flavors.CAT + " = ?", new object[] { categoryId });

            foreach (var flavor in flavors)
            {
                flavor.CategoryID = categoryId;
                await Database.Insert(Tables.Flavors.TABLE_NAME, flavor.GetData());
            }
        }

        /// <summary>
        /// Deletes a category from the database.
        /// </summary>
        /// <param name="category">The Category.</param>
        /// <returns>Whether the category was successfully deleted.</returns>
        public static async Task<bool> DeleteCategoryAsync(Category category)
        {
            if (await Database.Delete(Tables.Cats.TABLE_NAME, BaseColumns._ID + " = ?", new object[] { category.ID }) > 0)
            {
                category.Deleted();
                RecordChanged(null, new RecordChangedEventArgs(RecordChangedAction.Delete, category));
                return true;
            }
            return false;
        }
    }
}
