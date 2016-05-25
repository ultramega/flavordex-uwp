using Flavordex.UI;
using Flavordex.Utilities.Databases;
using Flavordex.ViewModels;
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
        /// <returns>A Collection of EntryItemViewModels.</returns>
        public static async Task<Collection<EntryItemViewModel>> GetEntryListAsync(long categoryId)
        {
            var list = new Collection<EntryItemViewModel>();

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
                var entry = _entryCache.Get(row);
                list.Add(new EntryItemViewModel(entry));
            }

            return list;
        }

        /// <summary>
        /// Gets a journal entry from the database.
        /// </summary>
        /// <param name="entryId">The primary ID of the journal entry.</param>
        /// <returns>The EntryViewModel.</returns>
        public static async Task<EntryViewModel> GetEntryAsync(long entryId)
        {
            var where = BaseColumns._ID + " = ?";
            var whereArgs = new object[] { entryId };
            var rows = await Database.Query(Tables.Entries.VIEW_NAME, null, where, whereArgs);
            if (rows.Length > 0)
            {
                var entry = _entryCache.Get(rows[0]);

                EntryViewModel viewModel;
                switch (entry.Category)
                {
                    case Constants.CAT_BEER:
                        viewModel = new BeerEntryViewModel(entry);
                        break;
                    case Constants.CAT_COFFEE:
                        viewModel = new CoffeeEntryViewModel(entry);
                        break;
                    case Constants.CAT_WHISKEY:
                        viewModel = new WhiskeyEntryViewModel(entry);
                        break;
                    case Constants.CAT_WINE:
                        viewModel = new WineEntryViewModel(entry);
                        break;
                    default:
                        viewModel = new EntryViewModel(entry);
                        break;
                }
                await GetEntryExtrasAsync(viewModel);
                return viewModel;
            }

            return null;
        }

        /// <summary>
        /// Gets the extra fields for a journal entry.
        /// </summary>
        /// <param name="entry">The journal entry.</param>
        private static async Task GetEntryExtrasAsync(EntryViewModel entry)
        {
            var list = new Collection<EntryExtraItemViewModel>();

            var where = Tables.EntriesExtras.ENTRY + " = ?";
            var whereArgs = new object[] { entry.Model.ID };
            var sort = Tables.Extras.POS;
            var rows = await Database.Query(Tables.EntriesExtras.VIEW_NAME, null, where, whereArgs, sort);
            foreach (var row in rows)
            {
                var item = new EntryExtra();
                item.SetData(row);
                list.Add(new EntryExtraItemViewModel(item));
            }

            entry.Extras = list;
        }

        /// <summary>
        /// Gets the flavors for a journal entry.
        /// </summary>
        /// <param name="entryId">The primary ID of the journal entry.</param>
        /// <returns>A Collection of FlavorItemViewModels representing the flavors.</returns>
        public static async Task<Collection<EntryFlavorItemViewModel>> GetEntryFlavorsAsync(long entryId)
        {
            var list = new Collection<EntryFlavorItemViewModel>();

            var where = Tables.EntriesFlavors.ENTRY + " = ?";
            var whereArgs = new object[] { entryId };
            var sort = Tables.EntriesFlavors.POS;
            var rows = await Database.Query(Tables.EntriesFlavors.TABLE_NAME, null, where, whereArgs, sort);
            foreach (var row in rows)
            {
                var item = new EntryFlavor();
                item.SetData(row);
                list.Add(new EntryFlavorItemViewModel(item));
            }

            return list;
        }

        /// <summary>
        /// Gets the photos for a journal entry.
        /// </summary>
        /// <param name="entryId">The primary ID of the journal entry.</param>
        /// <returns>A Collection of PhotoItemViewModels representing the photos.</returns>
        public static async Task<Collection<PhotoItemViewModel>> GetEntryPhotosAsync(long entryId)
        {
            var list = new Collection<PhotoItemViewModel>();

            var where = Tables.Photos.ENTRY + " = ?";
            var whereArgs = new object[] { entryId };
            var sort = Tables.Photos.POS;
            var rows = await Database.Query(Tables.Photos.TABLE_NAME, null, where, whereArgs, sort);
            foreach (var row in rows)
            {
                var item = new Photo();
                item.SetData(row);
                list.Add(new PhotoItemViewModel(item));
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
        /// <returns>Whether the update was successful.</returns>
        public static async Task<bool> UpdateEntryAsync(EntryViewModel entry)
        {
            var action = entry.Model.ID == 0 ? RecordChangedAction.Insert : RecordChangedAction.Update;

            entry.Model.Updated = DateTime.Now;
            entry.Model.IsSynced = false;
            if (string.IsNullOrEmpty(entry.Model.UUID))
            {
                entry.Model.UUID = Guid.NewGuid().ToString();
            }

            var values = entry.Model.GetData();

            await GetMakerIDAsync(values);

            values.SetLong(Tables.Entries.CAT, values.GetLong(Tables.Entries.CAT_ID));
            values.Remove(Tables.Entries.CAT_ID);
            values.Remove(Tables.Entries.CAT_UUID);

            if (action == RecordChangedAction.Update)
            {
                await Database.Update(Tables.Entries.TABLE_NAME, values, BaseColumns._ID + " = ?", new object[] { entry.Model.ID });
            }
            else
            {
                entry.Model.ID = await Database.Insert(Tables.Entries.TABLE_NAME, values);
            }

            if (entry.Model.ID > 0)
            {
                await Database.Delete(Tables.EntriesExtras.TABLE_NAME, Tables.EntriesExtras.ENTRY + " = ?", new object[] { entry.Model.ID });
                values.Clear();
                values.SetLong(Tables.EntriesExtras.ENTRY, entry.Model.ID);
                foreach (var extra in entry.Extras)
                {
                    values.SetLong(Tables.EntriesExtras.EXTRA, extra.Model.ID);
                    values.SetString(Tables.EntriesExtras.VALUE, extra.Model.Value);
                    await Database.Insert(Tables.EntriesExtras.TABLE_NAME, values);
                }

                entry.Model.Changed();
                RecordChanged(null, new RecordChangedEventArgs(action, entry.Model));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Inserts the Flavors for a journal entry into the database.
        /// </summary>
        /// <param name="entryId">The primary ID of the journal entry.</param>
        /// <param name="flavors">The list of Flavors.</param>
        /// <returns>Whether the Flavors were inserted successfully.</returns>
        public static async Task<bool> InsertFlavorsAsync(long entryId, Collection<EntryFlavorItemViewModel> flavors)
        {
            await Database.Delete(Tables.EntriesFlavors.TABLE_NAME, Tables.EntriesFlavors.ENTRY + " = ?", new object[] { entryId });

            var position = 0;
            foreach (var flavor in flavors)
            {
                flavor.Model.EntryID = entryId;
                flavor.Model.Position = position++;
                await Database.Insert(Tables.EntriesFlavors.TABLE_NAME, flavor.Model.GetData());
            }

            _entryCache.Changed(entryId);
            return true;
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
        /// <returns>A Collection of CategoryItemViewModels.</returns>
        public static async Task<Collection<CategoryItemViewModel>> GetCategoryListAsync()
        {
            var list = new Collection<CategoryItemViewModel>();

            var rows = await Database.Query(Tables.Cats.VIEW_NAME);
            foreach (var row in rows)
            {
                var item = _categoryCache.Get(row);
                list.Add(new CategoryItemViewModel(item));
            }

            return new Collection<CategoryItemViewModel>(list.OrderBy(e => e.Name).ToArray());
        }

        /// <summary>
        /// Gets a category from the database.
        /// </summary>
        /// <param name="categoryId">The primary ID of the category.</param>
        /// <returns>The CategoryViewModel.</returns>
        public static async Task<CategoryViewModel> GetCategoryAsync(long categoryId)
        {
            var where = BaseColumns._ID + " = ?";
            var whereArgs = new object[] { categoryId };
            var rows = await Database.Query(Tables.Cats.VIEW_NAME, null, where, whereArgs);
            foreach (var row in rows)
            {
                var category = _categoryCache.Get(row);
                var viewModel = new CategoryViewModel(category);
                await GetCategoryExtrasAsync(viewModel);
                viewModel.Flavors = await GetCategoryFlavorsAsync(category.ID);
                return viewModel;
            }

            return null;
        }

        /// <summary>
        /// Gets the extra fields for a category.
        /// </summary>
        /// <param name="category">The category.</param>
        private static async Task GetCategoryExtrasAsync(CategoryViewModel category)
        {
            var list = new ObservableCollection<ExtraItemViewModel>();

            var where = Tables.Extras.CAT + " = ?";
            var whereArgs = new object[] { category.Model.ID };
            var sort = Tables.Extras.POS;
            var rows = await Database.Query(Tables.Extras.TABLE_NAME, null, where, whereArgs, sort);
            foreach (var row in rows)
            {
                var item = new Extra();
                item.SetData(row);
                list.Add(new ExtraItemViewModel(item));
            }

            category.Extras = list;
        }

        /// <summary>
        /// Gets the flavors for a category.
        /// </summary>
        /// <param name="categoryId">The primary ID of the category.</param>
        /// <returns>The list of Flavors for the Category.</returns>
        public static async Task<ObservableCollection<FlavorItemViewModel>> GetCategoryFlavorsAsync(long categoryId)
        {
            var list = new ObservableCollection<FlavorItemViewModel>();

            var where = Tables.Flavors.CAT + " = ?";
            var whereArgs = new object[] { categoryId };
            var sort = Tables.Flavors.POS;
            var rows = await Database.Query(Tables.Flavors.TABLE_NAME, null, where, whereArgs, sort);
            foreach (var row in rows)
            {
                var item = new Flavor();
                item.SetData(row);
                list.Add(new FlavorItemViewModel(item));
            }

            return list;
        }

        /// <summary>
        /// Updates or inserts a Category in the database.
        /// </summary>
        /// <param name="category">The Category to update or insert.</param>
        /// <returns>Whether the update was successful.</returns>
        public static async Task<bool> UpdateCategoryAsync(CategoryViewModel category)
        {
            var action = category.Model.ID == 0 ? RecordChangedAction.Insert : RecordChangedAction.Update;
            category.Model.Updated = DateTime.Now;
            category.Model.IsSynced = false;

            var values = category.Model.GetData();
            values.Remove(Tables.Cats.NUM_ENTRIES);

            if (string.IsNullOrEmpty(category.Model.UUID))
            {
                category.Model.UUID = Guid.NewGuid().ToString();
            }

            if (action == RecordChangedAction.Update)
            {
                await Database.Update(Tables.Cats.TABLE_NAME, values, BaseColumns._ID + " = ?", new object[] { category.Model.ID });
            }
            else
            {
                category.Model.ID = await Database.Insert(Tables.Cats.TABLE_NAME, values);
            }

            if (category.Model.ID > 0)
            {
                foreach (var extra in category.Extras)
                {
                    if (string.IsNullOrEmpty(extra.Model.UUID))
                    {
                        extra.Model.UUID = Guid.NewGuid().ToString();
                    }
                    if (extra.Model.ID > 0)
                    {
                        if (extra.Model.IsDeleted)
                        {
                            await Database.Delete(Tables.Extras.TABLE_NAME, BaseColumns._ID + " = ?", new object[] { extra.Model.ID });
                        }
                        else
                        {
                            await Database.Update(Tables.Extras.TABLE_NAME, extra.Model.GetData(), BaseColumns._ID + " = ?", new object[] { extra.Model.ID });
                        }
                    }
                    else
                    {
                        extra.Model.CategoryID = category.Model.ID;
                        await Database.Insert(Tables.Extras.TABLE_NAME, extra.Model.GetData());
                    }
                }

                await Database.Delete(Tables.Flavors.TABLE_NAME, Tables.Flavors.CAT + " = ?", new object[] { category.Model.ID });
                foreach (var flavor in category.Flavors)
                {
                    flavor.Model.CategoryID = category.Model.ID;
                    await Database.Insert(Tables.Flavors.TABLE_NAME, flavor.Model.GetData());
                }

                category.Model.Changed();
                RecordChanged(null, new RecordChangedEventArgs(action, category.Model));
                return true;
            }

            return false;
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
