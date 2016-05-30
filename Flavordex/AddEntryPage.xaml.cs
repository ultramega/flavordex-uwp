using Flavordex.Models;
using Flavordex.Models.Data;
using Flavordex.UI;
using Flavordex.Utilities;
using Flavordex.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Flavordex
{
    /// <summary>
    /// Page for adding a new journal entry.
    /// </summary>
    public sealed partial class AddEntryPage : Page
    {
        /// <summary>
        /// Gets or sets the title of the Page in the CommandBar.
        /// </summary>
        public string PageTitle
        {
            get { return (string)GetValue(PageTitleProperty); }
            set { SetValue(PageTitleProperty, value); }
        }
        public static readonly DependencyProperty PageTitleProperty =
            DependencyProperty.Register("PageTitle", typeof(string), typeof(AddEntryPage), null);

        /// <summary>
        /// Gets or sets the Entry being created.
        /// </summary>
        public EntryViewModel Entry
        {
            get { return (EntryViewModel)GetValue(EntryProperty); }
            set { SetValue(EntryProperty, value); }
        }
        public static readonly DependencyProperty EntryProperty =
            DependencyProperty.Register("Entry", typeof(EntryViewModel), typeof(AddEntryPage), null);

        /// <summary>
        /// Gets or sets the Visibility of the photo commands in the CommandBar.
        /// </summary>
        public Visibility PhotoCommandVisibility
        {
            get { return (Visibility)GetValue(TakePhotoVisibilityProperty); }
            set { SetValue(TakePhotoVisibilityProperty, value); }
        }
        public static readonly DependencyProperty TakePhotoVisibilityProperty =
            DependencyProperty.Register("TakePhotoVisibility", typeof(Visibility), typeof(AddEntryPage), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Gets or sets the list of Flavors in the RadarGraph.
        /// </summary>
        public Collection<RadarItem> Flavors
        {
            get { return (Collection<RadarItem>)GetValue(FlavorsProperty); }
            set { SetValue(FlavorsProperty, value); }
        }
        public static readonly DependencyProperty FlavorsProperty =
            DependencyProperty.Register("Flavors", typeof(Collection<RadarItem>), typeof(AddEntryPage), null);

        /// <summary>
        /// Gets the list of Photos in the Entry.
        /// </summary>
        private ObservableCollection<KeyValuePair<StorageFile, BitmapImage>> Photos { get; } = new ObservableCollection<KeyValuePair<StorageFile, BitmapImage>>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public AddEntryPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Activates the back button and loads the Category or Category list when the Page is
        /// navigated to.
        /// </summary>
        /// <param name="e">
        /// The event arguments containing the Category ID as the Parameter.
        /// </param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.BackRequested += OnBackRequested;
            systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            if (e.Parameter is long && (long)e.Parameter > 0)
            {
                LoadCategory((long)e.Parameter);
            }
            else
            {
                PageTitle = ResourceLoader.GetForCurrentView("AddEntry").GetString("Title/SelectCategory");
                var categories = new Collection<CategoryItemViewModel>();
                foreach (var category in await DatabaseHelper.GetCategoryListAsync())
                {
                    categories.Add(new CategoryItemViewModel(category));
                }
                (FindName("CategoryList") as ListView).ItemsSource = categories.OrderBy(k => k.Name);
            }
        }

        /// <summary>
        /// Deactivates the back button when the Page is navigated away from.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            var systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.BackRequested -= OnBackRequested;
            systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        /// <summary>
        /// Navigates back when the back button is pressed.
        /// </summary>
        /// <param name="sender">The SystemNavigationManager.</param>
        /// <param name="e">The event arguments.</param>
        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
            Frame.GoBack();
        }

        /// <summary>
        /// Updates the Visibility of the photo buttons in the CommandBar when the Pivot changes
        /// tab.
        /// </summary>
        /// <param name="sender">The Pivot.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTabChanged(object sender, SelectionChangedEventArgs e)
        {
            PhotoCommandVisibility = Pivot.SelectedIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Validates and saves the new Entry when the Save button is pressed.
        /// </summary>
        /// <param name="sender">The AppBarButton.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnSaveEntry(object sender, RoutedEventArgs e)
        {
            var extras = new Collection<EntryExtra>();
            foreach (var extra in Entry.Extras)
            {
                extras.Add(extra.Model);
            }
            if (await DatabaseHelper.UpdateEntryAsync(Entry.Model, extras))
            {
                var flavors = new Collection<EntryFlavor>();
                foreach (EntryFlavorItemViewModel flavor in Flavors)
                {
                    flavors.Add(flavor.Model);
                }
                await DatabaseHelper.UpdateEntryFlavorsAsync(Entry.Model.ID, flavors);

                var position = 0;
                foreach (var item in Photos)
                {
                    await PhotoUtilities.AddPhotoAsync(item.Key, Entry.Model.ID, position++);
                }

                if (Frame.BackStack.Count > 0)
                {
                    var entry = new PageStackEntry(typeof(ViewEntryPage), Entry.Model.ID, new DrillInNavigationTransitionInfo());
                    Frame.BackStack.Add(entry);
                    Frame.GoBack();
                }
            }
        }

        /// <summary>
        /// Removes a photo when its remove button is pressed.
        /// </summary>
        /// <param name="sender">The Button.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRemovePhoto(object sender, RoutedEventArgs e)
        {
            var photo = (KeyValuePair<StorageFile, BitmapImage>)(sender as FrameworkElement).DataContext;
            Photos.Remove(photo);
        }

        /// <summary>
        /// Launches the camera to take a new photo when the Take Photo button is pressed.
        /// </summary>
        /// <param name="sender">The AppBarButton.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnTakePhoto(object sender, RoutedEventArgs e)
        {
            var file = await PhotoUtilities.CapturePhotoAsync();
            if (file != null)
            {
                AddPhoto(file);
            }
        }

        /// <summary>
        /// Launches the file selector to select a photo to add when the Select Photo button is
        /// pressed.
        /// </summary>
        /// <param name="sender">The AppBarButton.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnSelectPhoto(object sender, RoutedEventArgs e)
        {
            var files = await PhotoUtilities.GetPicker().PickMultipleFilesAsync();
            if (files.Count > 0)
            {
                foreach (var file in files)
                {
                    if (!Photos.Any(k => k.Key.Path == file.Path))
                    {
                        AddPhoto(file);
                    }
                }
            }
        }

        /// <summary>
        /// Loads the Category.
        /// </summary>
        /// <param name="categoryId">The primary ID of the Category.</param>
        private async void LoadCategory(long categoryId)
        {
            var category = await DatabaseHelper.GetCategoryAsync(categoryId);

            PageTitle = string.Format(ResourceLoader.GetForCurrentView("AddEntry").GetString("Title/AddEntry"), Presets.GetRealCategoryName(category.Name));

            var entry = new Entry() { Category = category.Name, CategoryID = categoryId, Date = DateTime.Now };
            Entry = EntryViewModel.GetInstance(entry);

            foreach (var item in await DatabaseHelper.GetCategoryExtrasAsync(categoryId))
            {
                var extra = new EntryExtra();
                extra.ExtraID = item.ID;
                extra.IsPreset = item.IsPreset;
                extra.Name = item.Name;
                Entry.Extras.Add(new EntryExtraItemViewModel(extra));
            }

            var flavors = new Collection<RadarItem>();
            foreach (var item in await DatabaseHelper.GetCategoryFlavorsAsync(categoryId))
            {
                var flavor = new EntryFlavor();
                flavor.Name = item.Name;
                flavor.Value = 0;
                flavors.Add(new EntryFlavorItemViewModel(flavor));
            }
            Flavors = flavors;

            FindName("Pivot");
        }

        /// <summary>
        /// Adds a photo from a file.
        /// </summary>
        /// <param name="file">The photo file.</param>
        private async void AddPhoto(StorageFile file)
        {
            var photo = new BitmapImage();
            photo.DecodePixelWidth = 400;
            await photo.SetSourceAsync(await file.OpenReadAsync());
            Photos.Add(new KeyValuePair<StorageFile, BitmapImage>(file, photo));
        }

        /// <summary>
        /// Loads the selected Category from the list.
        /// </summary>
        /// <param name="sender">The CategoryList.</param>
        /// <param name="e">The event arguments.</param>
        private void OnCategorySelected(object sender, ItemClickEventArgs e)
        {
            CategoryList.Visibility = Visibility.Collapsed;
            LoadCategory((e.ClickedItem as CategoryItemViewModel).Model.ID);
        }
    }
}
