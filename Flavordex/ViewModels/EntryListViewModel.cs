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
using Flavordex.Models;
using Flavordex.Models.Data;
using Flavordex.Utilities.Databases;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;

namespace Flavordex.ViewModels
{
    /// <summary>
    /// The ViewModel for the journal entry list.
    /// </summary>
    public class EntryListViewModel : ViewModel
    {
        /// <summary>
        /// A reference to the string ResourceLoader.
        /// </summary>
        private static readonly ResourceLoader _resources =
            ResourceLoader.GetForCurrentView("EntryList");

        /// <summary>
        /// The title of the categories list.
        /// </summary>
        private static readonly string _categoriesTitle = _resources.GetString("Title/Categories");

        /// <summary>
        /// The format string for the entry list title.
        /// </summary>
        private static readonly string _entriesTitleFormat =
            _resources.GetString("Title/EntriesList");

        /// <summary>
        /// The title for the search results list.
        /// </summary>
        private static readonly string _searchTitle = _resources.GetString("Title/SearchResults");

        /// <summary>
        /// The category name for the unfiltered entry list.
        /// </summary>
        private static readonly string _all = _resources.GetString("All");

        /// <summary>
        /// The Category representing all Categories.
        /// </summary>
        private Category _allEntries =
            new Category() { Name = _resources.GetString("Category/All") };

        /// <summary>
        /// Gets the list of EntryItemViewModels to display.
        /// </summary>
        public ObservableCollection<EntryItemViewModel> Entries { get; } =
            new ObservableCollection<EntryItemViewModel>();

        /// <summary>
        /// Gets the list of CategoryItemViewModels to display.
        /// </summary>
        public ObservableCollection<CategoryItemViewModel> Categories { get; } =
            new ObservableCollection<CategoryItemViewModel>();

        /// <summary>
        /// The currently active search parameters.
        /// </summary>
        private SearchViewModel _search;

        /// <summary>
        /// Gets or sets currently active search parameters.
        /// </summary>
        public SearchViewModel Search
        {
            get
            {
                return _search;
            }
            set
            {
                _search = value;
                if (value != null)
                {
                    Settings.ListCategory = 0;
                }
            }
        }

        /// <summary>
        /// The list title text.
        /// </summary>
        private string _listTitle;

        /// <summary>
        /// Gets or sets the list title text.
        /// </summary>
        public string ListTitle
        {
            get
            {
                return _listTitle;
            }
            set
            {
                _listTitle = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets whether a Category is selected.
        /// </summary>
        public bool IsCategorySelected
        {
            get
            {
                return Settings.ListCategory > -1;
            }
        }

        /// <summary>
        /// Gets the selected Category.
        /// </summary>
        public CategoryItemViewModel SelectedCategory
        {
            get
            {
                return Categories.FirstOrDefault(e => e.Model.ID == Settings.ListCategory);
            }
        }

        /// <summary>
        /// The primary ID of the currently selected Entry.
        /// </summary>
        private long _selectedEntryId;

        /// <summary>
        /// Gets or sets the primary ID of the currently selected Entry.
        /// </summary>
        public long SelectedEntryId
        {
            get
            {
                return _selectedEntryId;
            }
            set
            {
                _selectedEntryId = value;
                RaisePropertyChanged("SelectedEntry");
            }
        }

        /// <summary>
        /// Gets the currently selected EntryItemViewModel.
        /// </summary>
        public EntryItemViewModel SelectedEntry
        {
            get
            {
                return Entries.FirstOrDefault(e => e.Model.ID == _selectedEntryId);
            }
        }

        /// <summary>
        /// Gets the Visibility of the empty list message.
        /// </summary>
        public Visibility EmptyListVisibility
        {
            get
            {
                return IsCategorySelected && Entries.Count == 0
                    ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets the empty list message.
        /// </summary>
        public string EmptyListMessage
        {
            get
            {
                return _resources.GetString(Search == null
                    ? "Message/NoEntries" : "Message/NoEntriesFilter");
            }
        }

        /// <summary>
        /// Whether export mode is enabled.
        /// </summary>
        private bool _exportMode;

        /// <summary>
        /// Gets or sets whether export mode is enabled.
        /// </summary>
        public bool ExportMode
        {
            get
            {
                return _exportMode;
            }
            set
            {
                if (_exportMode != value)
                {
                    _exportMode = value;
                    foreach (var item in Entries)
                    {
                        item.ThumbnailVisibility = value
                            ? Visibility.Collapsed : Visibility.Visible;
                    }
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public EntryListViewModel()
        {
            Settings.SettingChanged += OnSettingChanged;
            DatabaseHelper.RecordChanged += OnRecordChanged;
            Entries.CollectionChanged += OnEntriesCollectionChanged;
            LoadCategories();
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~EntryListViewModel()
        {
            Settings.SettingChanged -= OnSettingChanged;
            DatabaseHelper.RecordChanged -= OnRecordChanged;
        }

        /// <summary>
        /// Loads the list of Categories.
        /// </summary>
        private async void LoadCategories()
        {
            Categories.Add(new CategoryItemViewModel(_allEntries));

            var unsorted = new Collection<CategoryItemViewModel>();
            foreach (var item in await DatabaseHelper.GetCategoryListAsync())
            {
                _allEntries.EntryCount += item.EntryCount;
                unsorted.Add(new CategoryItemViewModel(item));
            }
            _allEntries.Changed();

            foreach (var item in unsorted.OrderBy(e => e.Name))
            {
                Categories.Add(item);
            }

            LoadEntries();
        }

        /// <summary>
        /// Loads the journal entries for the selected Category.
        /// </summary>
        private async void LoadEntries()
        {
            if (Settings.ListCategory > -1)
            {
                Entries.Clear();
                var entries = _search == null
                    ? await DatabaseHelper.GetEntryListAsync(Settings.ListCategory)
                    : await _search.GetList();
                foreach (var item in entries)
                {
                    var entry = new EntryItemViewModel(item);
                    entry.ThumbnailVisibility =
                        ExportMode ? Visibility.Collapsed : Visibility.Visible;
                    Entries.Add(entry);
                }
                SortList();

                var category = Categories.FirstOrDefault(e => e.Model.ID == Settings.ListCategory);
                if (_search != null)
                {
                    ListTitle = _searchTitle;
                }
                else if (category != null)
                {
                    ListTitle = string.Format(_entriesTitleFormat,
                        category.Model.ID == 0 ? _all : category.Name);
                }
            }
            else
            {
                ListTitle = _categoriesTitle;
                _search = null;
            }

            RaisePropertyChanged("EmptyListVisibility");
            RaisePropertyChanged("SelectedCategory");
            RaisePropertyChanged("IsCategorySelected");
        }

        /// <summary>
        /// Raises the PropertyChanged event for the SelectedEntry and EmptyListVisibility
        /// properties when the Entries Collection changes.
        /// </summary>
        /// <param name="sender">The ObservableCollection.</param>
        /// <param name="e">The event arguments.</param>
        private void OnEntriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("EmptyListVisibility");
            RaisePropertyChanged("SelectedEntry");
        }

        /// <summary>
        /// Updates the list according to the list settings when a setting changes.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSettingChanged(object sender, Settings.SettingChangedEventArgs e)
        {
            switch (e.Key)
            {
                case Settings.Key.ListSortDescending:
                case Settings.Key.ListSortField:
                    SortList();
                    break;
                case Settings.Key.ListCategory:
                    LoadEntries();
                    break;
            }
        }

        /// <summary>
        /// Updates the list when a record is inserted or deleted.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRecordChanged(object sender, RecordChangedEventArgs e)
        {
            switch (e.Action)
            {
                case RecordChangedAction.Insert:
                    if (e.Model is Entry)
                    {
                        InsertEntry(e.Model as Entry);
                    }
                    else if (e.Model is Category)
                    {
                        InsertCategory(e.Model as Category);
                    }
                    break;
                case RecordChangedAction.Update:
                    if (e.Model is Entry)
                    {
                        SortEntry(e.Model as Entry);
                    }
                    else if (e.Model is Category)
                    {
                        SortCategory(e.Model as Category);
                    }
                    break;
                case RecordChangedAction.Delete:
                    if (e.Model is Entry)
                    {
                        DeleteEntry(e.Model as Entry);
                    }
                    else if (e.Model is Category)
                    {
                        DeleteCategory(e.Model as Category);
                    }
                    break;
            }
        }

        /// <summary>
        /// Adds an Entry to the list.
        /// </summary>
        /// <param name="entry">The entry to add.</param>
        private async void InsertEntry(Entry entry)
        {
            if ((Settings.ListCategory == 0 || entry.CategoryID == Settings.ListCategory)
                && (_search == null || await _search.Matches(entry)))
            {
                Entries.Insert(FindSortedIndex(entry), new EntryItemViewModel(entry));
            }

            _allEntries.EntryCount++;
            _allEntries.Changed();
        }

        /// <summary>
        /// Adds a Category to the list.
        /// </summary>
        /// <param name="category">The Category to add.</param>
        private void InsertCategory(Category category)
        {
            Categories.Insert(FindSortedIndex(category), new CategoryItemViewModel(category));
        }

        /// <summary>
        /// Moves an Entry to its sorted position.
        /// </summary>
        /// <param name="entry">The Entry to sort.</param>
        private void SortEntry(Entry entry)
        {
            var item = Entries.FirstOrDefault(e => e.Model == entry);
            var index = Entries.IndexOf(item);
            var newIndex = FindSortedIndex(entry);
            if (index > -1 && newIndex > index)
            {
                newIndex--;
            }
            if (index < 0 || index != newIndex)
            {
                Entries.Remove(item);
                Entries.Insert(newIndex, item);
            }
        }

        /// <summary>
        /// Moves a Category to its sorted position.
        /// </summary>
        /// <param name="category">The Category to sort.</param>
        private void SortCategory(Category category)
        {
            var item = Categories.FirstOrDefault(e => e.Model == category);
            var index = Categories.IndexOf(item);
            var newIndex = FindSortedIndex(category);
            if (index > -1 && newIndex > index)
            {
                newIndex--;
            }
            if (index < 0 || index != newIndex)
            {
                Categories.Remove(item);
                Categories.Insert(newIndex, item);
            }
        }

        /// <summary>
        /// Finds the position to insert or move an Entry.
        /// </summary>
        /// <param name="entry">The Entry to position.</param>
        /// <returns>The index to place the Entry.</returns>
        private int FindSortedIndex(Entry entry)
        {
            for (var i = 0; i < Entries.Count; i++)
            {
                if (Entries[i].Model == entry)
                {
                    continue;
                }

                if (Settings.ListSortDescending)
                {
                    switch (Settings.ListSortField)
                    {
                        case Settings.SortField.Name:
                            if (Entries[i].Title.CompareTo(entry.Title) <= 0)
                            {
                                return i;
                            }
                            break;
                        case Settings.SortField.Date:
                            if (Entries[i].Date <= entry.Date)
                            {
                                return i;
                            }
                            break;
                        case Settings.SortField.Rating:
                            if (Entries[i].Rating <= entry.Rating)
                            {
                                return i;
                            }
                            break;
                    }
                }
                else
                {
                    switch (Settings.ListSortField)
                    {
                        case Settings.SortField.Name:
                            if (Entries[i].Title.CompareTo(entry.Title) >= 0)
                            {
                                return i;
                            }
                            break;
                        case Settings.SortField.Date:
                            if (Entries[i].Date >= entry.Date)
                            {
                                return i;
                            }
                            break;
                        case Settings.SortField.Rating:
                            if (Entries[i].Rating >= entry.Rating)
                            {
                                return i;
                            }
                            break;
                    }
                }
            }

            return Entries.Count;
        }

        /// <summary>
        /// Finds the position to insert or move a Category.
        /// </summary>
        /// <param name="entry">The Category to position.</param>
        /// <returns>The index to place the Category.</returns>
        private int FindSortedIndex(Category category)
        {
            for (var i = 1; i < Categories.Count; i++)
            {
                if (Categories[i].Model == category)
                {
                    continue;
                }

                if (Categories[i].Name.CompareTo(category.Name) >= 0)
                {
                    return i;
                }
            }
            return Categories.Count;
        }

        /// <summary>
        /// Removes an Entry from the list.
        /// </summary>
        /// <param name="entry">The Entry to remove.</param>
        private void DeleteEntry(Entry entry)
        {
            var item = Entries.FirstOrDefault(e => e.Model == entry);
            if (item != null)
            {
                Entries.Remove(item);

                _allEntries.EntryCount--;
                _allEntries.Changed();
            }

            if (entry.ID == SelectedEntryId)
            {
                SelectedEntryId = 0;
            }
        }

        /// <summary>
        /// Removes a Category from the list.
        /// </summary>
        /// <param name="category">The Category to remove.</param>
        private void DeleteCategory(Category category)
        {
            var item = Categories.FirstOrDefault(e => e.Model == category);
            if (item != null)
            {
                Categories.Remove(item);

                _allEntries.EntryCount -= category.EntryCount;
                _allEntries.Changed();
            }

            if (category.ID == Settings.ListCategory)
            {
                Settings.ListCategory = -1;
            }
        }

        /// <summary>
        /// Sorts the list of Entries according to the current sorting parameters.
        /// </summary>
        private void SortList()
        {
            EntryItemViewModel[] entries;
            switch (Settings.ListSortField)
            {
                case Settings.SortField.Date:
                    entries = Entries.OrderBy(key => key.Model.Date).ToArray();
                    break;
                case Settings.SortField.Rating:
                    entries = Entries.OrderBy(key => key.Model.Rating).ToArray();
                    break;
                default:
                    entries = Entries.OrderBy(key => key.Model.Title).ToArray();
                    break;
            }

            if (Settings.ListSortDescending)
            {
                entries = entries.Reverse().ToArray();
            }

            Entries.Clear();
            foreach (var item in entries)
            {
                Entries.Add(item);
            }
        }
    }
}
