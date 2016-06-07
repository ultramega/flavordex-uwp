using Flavordex.Utilities;
using Flavordex.Utilities.Databases;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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
        /// <param name="flavors">The flavors for the journal entry.</param>
        /// <returns>Whether the update was successful.</returns>
        public static async Task<bool> UpdateEntryAsync(Entry entry, Collection<EntryExtra> extras = null, Collection<EntryFlavor> flavors = null)
        {
            entry.Title = FilterName(entry.Title);
            if (string.IsNullOrEmpty(entry.Title))
            {
                return false;
            }

            var action = entry.ID == 0 ? RecordChangedAction.Insert : RecordChangedAction.Update;

            entry.Updated = DateTime.Now;
            entry.IsSynced = false;

            if (string.IsNullOrEmpty(entry.UUID))
            {
                entry.UUID = Guid.NewGuid().ToString();
            }

            await GetCategoryIdAsync(entry, flavors);
            if (entry.CategoryID == 0)
            {
                return false;
            }

            var values = entry.GetData();

            await GetMakerIDAsync(values);

            values.SetLong(Tables.Entries.CAT, values.GetLong(Tables.Entries.CAT_ID));
            values.Remove(Tables.Entries.CAT_ID);
            values.Remove(Tables.Entries.CAT_UUID);

            if (action == RecordChangedAction.Update)
            {
                values.Remove(Tables.Entries.UUID);
                values.Remove(Tables.Entries.CAT_ID);
                await Database.Update(Tables.Entries.TABLE_NAME, values, BaseColumns._ID + " = ?", new object[] { entry.ID });
            }
            else
            {
                entry.Title = await GetUniqueEntryTitle(entry.Title);
                values.SetString(Tables.Entries.TITLE, entry.Title);
                entry.ID = await Database.Insert(Tables.Entries.TABLE_NAME, values);
                if (entry.ID > 0)
                {
                    _entryCache.Put(entry);
                    var category = _categoryCache.Get(entry.CategoryID);
                    if (category != null)
                    {
                        category.EntryCount++;
                        category.Changed();
                    }
                }
            }

            if (entry.ID > 0)
            {
                await UpdateEntryExtrasAsync(entry.ID, entry.CategoryID, extras);
                await UpdateEntryFlavorsAsync(entry.ID, flavors);

                entry.Changed();
                RecordChanged(null, new RecordChangedEventArgs(action, entry));

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the primary ID of the category for a journal entry, creating it if needed.
        /// </summary>
        /// <param name="entry">The journal entry.</param>
        /// <param name="flavors">The flavors for the journal entry.</param>
        private static async Task GetCategoryIdAsync(Entry entry, Collection<EntryFlavor> flavors)
        {
            if (entry.CategoryID > 0)
            {
                return;
            }

            var projection = new string[] { BaseColumns._ID };
            var where = Tables.Cats.NAME + " = ?";
            var whereArgs = new object[] { entry.Category };

            var rows = await Database.Query(Tables.Cats.TABLE_NAME, projection, where, whereArgs, null, "1");
            if (rows.Length > 0)
            {
                entry.CategoryID = rows[0].GetLong(BaseColumns._ID);
            }
            else
            {
                var category = new Category()
                {
                    Name = entry.Category
                };
                Collection<Flavor> categoryFlavors = null;
                if (flavors != null)
                {
                    categoryFlavors = new Collection<Flavor>();
                    var position = 0;
                    foreach (var flavor in flavors)
                    {
                        categoryFlavors.Add(new Flavor()
                        {
                            Name = flavor.Name,
                            Position = position++
                        });
                    }
                }
                await UpdateCategoryAsync(category, null, categoryFlavors);
                entry.CategoryID = category.ID;
            }
        }

        /// <summary>
        /// Gets a title that does not exist by appending a number.
        /// </summary>
        /// <param name="title">The original title.</param>
        /// <returns>The unique title.</returns>
        private static async Task<string> GetUniqueEntryTitle(string title)
        {
            var projection = new string[] { Tables.Entries.TITLE };
            var where = Tables.Entries.TITLE + " LIKE ?";
            var whereArgs = new object[] { title + "%" };
            var sort = Tables.Entries.TITLE;

            var rows = await Database.Query(Tables.Entries.TABLE_NAME, projection, where, whereArgs, sort);
            if (rows.Length > 0)
            {
                string newTitle = title;
                var number = 2;
                while (rows.Any(e => e.GetString(Tables.Entries.TITLE) == newTitle))
                {
                    newTitle = string.Format("{0} ({1})", title, number++);
                }

                return newTitle;
            }

            return title;
        }

        /// <summary>
        /// Checks whether a journal entry exists with the specified UUID.
        /// </summary>
        /// <param name="uuid">The UUID to check.</param>
        /// <returns>Whether a journal entry exists with the UUID.</returns>
        public static async Task<bool> EntryUuidExists(string uuid)
        {
            var projection = new string[] { BaseColumns._ID };
            var where = Tables.Entries.UUID + " = ?";
            var whereArgs = new object[] { uuid };
            return (await Database.Query(Tables.Entries.TABLE_NAME, projection, where, whereArgs, null, "1")).Length > 0;
        }

        /// <summary>
        /// Updates the Extras for a journal entry.
        /// </summary>
        /// <param name="entryId">The primary ID of the journal entry.</param>
        /// <param name="categoryId">The primary ID of the category.</param>
        /// <param name="extras">The list of EntryExtras.</param>
        private static async Task UpdateEntryExtrasAsync(long entryId, long categoryId, Collection<EntryExtra> extras)
        {
            if (extras == null)
            {
                return;
            }

            var values = new ContentValues();
            values.SetLong(Tables.EntriesExtras.ENTRY, entryId);
            foreach (var extra in extras)
            {
                if (!extra.IsPreset && string.IsNullOrWhiteSpace(extra.Value))
                {
                    await Database.Delete(Tables.EntriesExtras.TABLE_NAME, BaseColumns._ID + " = ?", new object[] { extra.ID });
                    continue;
                }
                await GetExtraIdAsync(categoryId, extra);
                if (extra.ExtraID > 0)
                {
                    values.SetLong(Tables.EntriesExtras.EXTRA, extra.ExtraID);
                    values.SetString(Tables.EntriesExtras.VALUE, extra.Value);
                    await Database.Insert(Tables.EntriesExtras.TABLE_NAME, values);
                }
            }
        }

        /// <summary>
        /// Gets the primary ID of an extra field, creating one if needed.
        /// </summary>
        /// <param name="categoryId">The primary ID of the category.</param>
        /// <param name="extra">The EntryExtra.</param>
        private static async Task GetExtraIdAsync(long categoryId, EntryExtra extra)
        {
            if (extra.ExtraID > 0)
            {
                return;
            }

            var projection = new string[] { BaseColumns._ID };
            var where = Tables.Extras.CAT + " = ? AND " + Tables.Extras.NAME + " = ?";
            var whereArgs = new object[] { categoryId, extra.Name };

            var rows = await Database.Query(Tables.Extras.TABLE_NAME, projection, where, whereArgs, null, "1");
            if (rows.Length > 0)
            {
                extra.ExtraID = rows[0].GetLong(BaseColumns._ID);
            }
            else
            {
                extra.Name = FilterName(extra.Name);
                if (!string.IsNullOrEmpty(extra.Name))
                {
                    var values = new ContentValues()
                    {
                        { Tables.Extras.UUID, Guid.NewGuid().ToString() },
                        { Tables.Extras.CAT, categoryId },
                        { Tables.Extras.NAME, extra.Name },
                        { Tables.Extras.POS, await GetNextExtraPositionAsync(categoryId) }
                    };
                    extra.ExtraID = await Database.Insert(Tables.Extras.TABLE_NAME, values);
                }
            }
        }

        /// <summary>
        /// Gets the next sorting position for an extra field.
        /// </summary>
        /// <param name="categoryId">The primary ID of the category.</param>
        /// <returns>The next sorting position for an extra field.</returns>
        private static async Task<long> GetNextExtraPositionAsync(long categoryId)
        {
            var projection = new string[] { Tables.Extras.POS };
            var where = Tables.Extras.CAT + " = ?";
            var whereArgs = new object[] { categoryId };
            var sort = Tables.Extras.POS + " DESC";

            var rows = await Database.Query(Tables.Extras.TABLE_NAME, projection, where, whereArgs, sort, "1");
            if (rows.Length > 0)
            {
                return rows[0].GetLong(Tables.Extras.POS) + 1;
            }

            return 0;
        }

        /// <summary>
        /// Updates the Flavors for a journal entry.
        /// </summary>
        /// <param name="entryId">The primary ID of the journal entry.</param>
        /// <param name="flavors">The list of EntryFlavors.</param>
        public static async Task UpdateEntryFlavorsAsync(long entryId, Collection<EntryFlavor> flavors)
        {
            if (flavors == null)
            {
                return;
            }

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
                var category = _categoryCache.Get(entry.CategoryID);
                if (category != null)
                {
                    category.EntryCount--;
                    category.Changed();
                }
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
        /// <param name="filterDeleted">Whether to exclude deleted extra fields.</param>
        public static async Task<Collection<Extra>> GetCategoryExtrasAsync(long categoryId, bool filterDeleted = false)
        {
            var list = new Collection<Extra>();

            var where = Tables.Extras.CAT + " = ?";
            if (filterDeleted)
            {
                where += " AND " + Tables.Extras.DELETED + " = 0";
            }
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

            if (!category.IsPreset)
            {
                category.Name = FilterName(category.Name);
                if (string.IsNullOrEmpty(category.Name))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(category.UUID))
                {
                    category.UUID = Guid.NewGuid().ToString();
                }

                var values = category.GetData();
                values.Remove(Tables.Cats.NUM_ENTRIES);

                if (action == RecordChangedAction.Update)
                {
                    await Database.Update(Tables.Cats.TABLE_NAME, values, BaseColumns._ID + " = ?", new object[] { category.ID });
                }
                else
                {
                    category.ID = await Database.Insert(Tables.Cats.TABLE_NAME, values);
                    if (category.ID > 0)
                    {
                        _categoryCache.Put(category);
                    }
                }
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
            if (extras == null)
            {
                return;
            }

            foreach (var extra in extras)
            {
                if (extra.IsPreset)
                {
                    continue;
                }

                extra.Name = FilterName(extra.Name);

                if (extra.ID > 0)
                {
                    if (extra.IsDeleted || string.IsNullOrWhiteSpace(extra.Name))
                    {
                        await Database.Delete(Tables.Extras.TABLE_NAME, BaseColumns._ID + " = ?", new object[] { extra.ID });
                    }
                    else
                    {
                        await Database.Update(Tables.Extras.TABLE_NAME, extra.GetData(), BaseColumns._ID + " = ?", new object[] { extra.ID });
                    }
                }
                else if (!string.IsNullOrWhiteSpace(extra.Name))
                {
                    if (string.IsNullOrEmpty(extra.UUID))
                    {
                        extra.UUID = Guid.NewGuid().ToString();
                    }
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
            if (flavors == null)
            {
                return;
            }

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
            var projection = new string[] { BaseColumns._ID };
            var where = Tables.Entries.CAT + " = ?";
            var whereArgs = new object[] { category.ID };
            var rows = await Database.Query(Tables.Entries.TABLE_NAME, projection, where, whereArgs);

            if (await Database.Delete(Tables.Cats.TABLE_NAME, BaseColumns._ID + " = ?", new object[] { category.ID }) > 0)
            {
                category.Deleted();
                RecordChanged(null, new RecordChangedEventArgs(RecordChangedAction.Delete, category));

                foreach (var item in rows)
                {
                    var id = item.GetLong(BaseColumns._ID);
                    var entry = _entryCache.Get(id);
                    if (entry != null)
                    {
                        entry.Deleted();
                        RecordChanged(null, new RecordChangedEventArgs(RecordChangedAction.Delete, entry));
                    }

                    PhotoUtilities.DeleteThumbnail(id);
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the list of makers.
        /// </summary>
        /// <returns>The Collection of Makers.</returns>
        public static async Task<Collection<Maker>> GetMakersAsync()
        {
            var list = new Collection<Maker>();
            var rows = await Database.Query(Tables.Makers.TABLE_NAME, null, null, null, Tables.Makers.NAME);
            foreach (var row in rows)
            {
                list.Add(new Maker()
                {
                    Name = row.GetString(Tables.Makers.NAME),
                    Location = row.GetString(Tables.Makers.LOCATION)
                });
            }
            return list;
        }

        /// <summary>
        /// Gets the list of saved locations.
        /// </summary>
        /// <returns>The list of saved locations.</returns>
        public static async Task<Collection<Location>> GetLocationListAsync()
        {
            var list = new Collection<Location>();
            var rows = await Database.Query(Tables.Locations.TABLE_NAME);
            foreach (var row in rows)
            {
                var location = new Location();
                location.SetData(row);
                list.Add(location);
            }
            return list;
        }

        /// <summary>
        /// Inserts a new location into the database.
        /// </summary>
        /// <param name="location">The Location to insert.</param>
        public static async void InsertLocation(Location location)
        {
            await Database.Insert(Tables.Locations.TABLE_NAME, location.GetData());
        }

        /// <summary>
        /// Prepares a non-preset name for the database.
        /// </summary>
        /// <param name="input">The original name.</param>
        /// <returns>The filtered name.</returns>
        private static string FilterName(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            return input.TrimStart(' ', '_').TrimEnd(' ');
        }
    }
}
