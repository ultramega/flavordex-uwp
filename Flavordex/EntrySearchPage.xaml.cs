using Flavordex.Models;
using Flavordex.Models.Data;
using Flavordex.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Flavordex
{
    /// <summary>
    /// Page for searching journal entries.
    /// </summary>
    public sealed partial class EntrySearchPage : Page
    {
        /// <summary>
        /// Gets or sets the search parameters.
        /// </summary>
        public SearchViewModel Data
        {
            get { return (SearchViewModel)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(SearchViewModel), typeof(EntrySearchPage),
                null);

        /// <summary>
        /// Gets the list of Categories.
        /// </summary>
        private ObservableCollection<CategoryItemViewModel> Categories { get; } =
            new ObservableCollection<CategoryItemViewModel>();

        /// <summary>
        /// The currently selected Category.
        /// </summary>
        private Category _selectedCategory = new Category()
        {
            Name = ResourceLoader.GetForCurrentView("EntryList").GetString("Category/All")
        };

        /// <summary>
        /// The CalendarDatePicker for the start date.
        /// </summary>
        private CalendarDatePicker StartDatePicker;

        /// <summary>
        /// The CalendarDatePicker for the end date.
        /// </summary>
        private CalendarDatePicker EndDatePicker;

        /// <summary>
        /// Constructor.
        /// </summary>
        public EntrySearchPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads the list of Categories and the current search parameters when the Page is loaded.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        private async void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            Data = (Application.Current as App).Search;
            if (Data == null)
            {
                Data = new SearchViewModel();
                LoadForm();
            }

            if (Categories.Count == 0)
            {
                Categories.Add(new CategoryItemViewModel(_selectedCategory));
                var unsorted = new Collection<CategoryItemViewModel>();
                foreach (var category in await DatabaseHelper.GetCategoryListAsync())
                {
                    unsorted.Add(new CategoryItemViewModel(category));
                }
                foreach (var category in unsorted.OrderBy(k => k.Name))
                {
                    Categories.Add(category);
                }
            }
            CategoryComboBox.SelectedItem =
                Categories.FirstOrDefault(k => k.Model.ID == Data.Entry.Model.CategoryID);
            CategoryComboBox.SelectionChanged += OnCategoryChanged;
        }

        /// <summary>
        /// Loads the form for the selected Category when it is selected from the CategoryComboBox.
        /// </summary>
        /// <param name="sender">The CategoryComboBox.</param>
        /// <param name="e">The event arguments.</param>
        private void OnCategoryChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedCategory = (e.AddedItems[0] as CategoryItemViewModel).Model;
            LoadForm();
        }

        /// <summary>
        /// Initializes the start and end date pickers when they are loaded..
        /// </summary>
        /// <param name="sender">The CalendarDatePicker.</param>
        /// <param name="e">The event arguments.</param>
        private void DatePickerLoaded(object sender, RoutedEventArgs e)
        {
            if ((sender as CalendarDatePicker).Name == "StartDatePicker")
            {
                StartDatePicker = sender as CalendarDatePicker;
                StartDatePicker.MaxDate = DateTimeOffset.Now;
                StartDatePicker.Date = Data.StartDate;
            }
            if ((sender as CalendarDatePicker).Name == "EndDatePicker")
            {
                EndDatePicker = sender as CalendarDatePicker;
                EndDatePicker.MaxDate = DateTimeOffset.Now;
                EndDatePicker.Date = Data.EndDate;
            }
        }

        /// <summary>
        /// Updates the StartDate property when the StartDatePicker's date changes and ensures the
        /// EndDate is after the StartDate.
        /// </summary>
        /// <param name="sender">The StartDatePicker.</param>
        /// <param name="args">The event arguments.</param>
        private void OnStartDateChange(CalendarDatePicker sender,
            CalendarDatePickerDateChangedEventArgs args)
        {
            if (args.OldDate.Equals(args.NewDate))
            {
                return;
            }
            Data.StartDate = args.NewDate;
            if (Data.StartDate > Data.EndDate)
            {
                Data.EndDate = Data.StartDate;
                EndDatePicker.Date = Data.EndDate;
            }
        }

        /// <summary>
        /// Updates the EndDate property when the EndDatePicker's date changes and ensures the
        /// EndDate is after the StartDate.
        /// </summary>
        /// <param name="sender">The EndDatePicker.</param>
        /// <param name="args">The event arguments.</param>
        private void OnEndDateChange(CalendarDatePicker sender,
            CalendarDatePickerDateChangedEventArgs args)
        {
            if (args.OldDate.Equals(args.NewDate))
            {
                return;
            }
            Data.EndDate = args.NewDate;
            if (Data.StartDate > Data.EndDate)
            {
                Data.StartDate = Data.EndDate;
                StartDatePicker.Date = Data.StartDate;
            }
        }

        /// <summary>
        /// Sets the origin field when a maker suggestion is selected.
        /// </summary>
        /// <param name="sender">The maker AutoSuggestBox.</param>
        /// <param name="args">The event arguments.</param>
        private void OnMakerSelected(AutoSuggestBox sender,
            AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var maker = args.ChosenSuggestion as Maker;
            Data.Entry.Origin = maker.Location;

            var children = (sender.Parent as Panel).Children;
            var index = children.IndexOf(sender);
            (children[index + 2] as Control).Focus(FocusState.Programmatic);
        }

        /// <summary>
        /// Loads the initial form for the selected Category.
        /// </summary>
        private async void LoadForm()
        {
            var entry = EntryViewModel.GetInstance(new Entry()
            {
                Category = _selectedCategory.Name,
                CategoryID = _selectedCategory.ID
            });
            if (_selectedCategory.ID > 0)
            {
                var extras =
                    await DatabaseHelper.GetCategoryExtrasAsync(_selectedCategory.ID, true);
                foreach (var extra in extras)
                {
                    entry.Extras.Add(new EntryExtraItemViewModel(new EntryExtra()
                    {
                        ExtraID = extra.ID,
                        Name = extra.Name,
                        IsPreset = extra.IsPreset
                    }));
                }
            }
            Data.Entry = entry;
        }

        /// <summary>
        /// Resets the form when the Clear button is pressed.
        /// </summary>
        /// <param name="sender">The Clear Button.</param>
        /// <param name="e">The event arguments.</param>
        private void OnClear(object sender, RoutedEventArgs e)
        {
            StartDatePicker.Date = null;
            EndDatePicker.Date = null;
            Data = new SearchViewModel();
            LoadForm();
        }

        /// <summary>
        /// Submits the new search parameters when the Search Entries button is pressed.
        /// </summary>
        /// <param name="sender">The Search Entries Button.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSearch(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).Search = Data;
        }
    }
}
