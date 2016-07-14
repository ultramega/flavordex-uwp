using Flavordex.Models;
using Flavordex.Models.Data;
using Flavordex.UI;
using Flavordex.UI.Controls;
using Flavordex.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Flavordex
{
    /// <summary>
    /// Page for adding or editing a Category.
    /// </summary>
    public sealed partial class EditCategoryPage : Page
    {
        /// <summary>
        /// Gets or sets the Page title.
        /// </summary>
        public string PageTitle
        {
            get { return (string)GetValue(PageTitleProperty); }
            set { SetValue(PageTitleProperty, value); }
        }
        public static readonly DependencyProperty PageTitleProperty =
            DependencyProperty.Register("PageTitle", typeof(string), typeof(EditCategoryPage),
                null);

        /// <summary>
        /// Gets or sets the Category.
        /// </summary>
        public CategoryViewModel Category
        {
            get { return (CategoryViewModel)GetValue(CategoryProperty); }
            set { SetValue(CategoryProperty, value); }
        }
        public static readonly DependencyProperty CategoryProperty =
            DependencyProperty.Register("Category", typeof(CategoryViewModel),
                typeof(EditCategoryPage), null);

        /// <summary>
        /// Gets or sets the visibility of the delete button.
        /// </summary>
        public Visibility DeleteButtonVisibility
        {
            get { return (Visibility)GetValue(DeleteButtonVisibilityProperty); }
            set { SetValue(DeleteButtonVisibilityProperty, value); }
        }
        public static readonly DependencyProperty DeleteButtonVisibilityProperty =
            DependencyProperty.Register("DeleteButtonVisibility", typeof(Visibility),
                typeof(EditCategoryPage), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Gets or sets the default fields for the Category.
        /// </summary>
        public string[] Fields
        {
            get { return (string[])GetValue(FieldsProperty); }
            set { SetValue(FieldsProperty, value); }
        }
        public static readonly DependencyProperty FieldsProperty =
            DependencyProperty.Register("Fields", typeof(string[]), typeof(EditCategoryPage),
                null);

        /// <summary>
        /// Gets the list of items to show on the RadarGraph.
        /// </summary>
        private ObservableCollection<RadarItem> RadarItems { get; } =
            new ObservableCollection<RadarItem>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public EditCategoryPage()
        {
            InitializeComponent();
            RegisterPropertyChangedCallback(CategoryProperty, OnCategoryChanged);
        }

        /// <summary>
        /// Loads the fields and adds event listeners when the Category property is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dp"></param>
        private void OnCategoryChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (Category != null)
            {
                string fields;
                switch (Category.Model.Name)
                {
                    case Constants.CAT_BEER:
                        fields = ResourceLoader.GetForCurrentView("Beer").GetString("Fields");
                        break;
                    case Constants.CAT_WINE:
                        fields = ResourceLoader.GetForCurrentView("Wine").GetString("Fields");
                        break;
                    case Constants.CAT_WHISKEY:
                        fields = ResourceLoader.GetForCurrentView("Whiskey").GetString("Fields");
                        break;
                    case Constants.CAT_COFFEE:
                        fields = ResourceLoader.GetForCurrentView("Coffee").GetString("Fields");
                        break;
                    default:
                        fields = ResourceLoader.GetForCurrentView().GetString("Fields");
                        break;
                }
                Fields = fields.Split(';');

                foreach (var item in Category.Extras)
                {
                    item.PropertyChanged += OnFieldPropertyChanged;
                }

                RadarItems.Clear();
                foreach (var item in Category.Flavors)
                {
                    item.PropertyChanged += OnFieldPropertyChanged;
                    RadarItems.Add(item);
                }

                DeleteButtonVisibility = Category.Model.ID > 0 && !Category.IsPreset
                    ? Visibility.Visible : Visibility.Collapsed;

                if (!Category.IsPreset)
                {
                    if (Category.Name != null)
                    {
                        TitleField.Text = Category.Name;
                    }
                    TitleField.SelectionStart = TitleField.Text.Length;
                    TitleField.Focus(FocusState.Programmatic);
                }
            }
        }

        /// <summary>
        /// Activates the back button and loads the Category to edit when the Page is navigated to.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is long)
            {
                PageTitle = ResourceLoader.GetForCurrentView("Category").GetString("Title/Edit");
                LoadCategory((long)e.Parameter);
            }
            else
            {
                PageTitle = ResourceLoader.GetForCurrentView("Category").GetString("Title/Add");
                var category = new CategoryViewModel(new Category());
                category.Flavors.Add(new FlavorItemViewModel(new Flavor()));
                Category = category;
            }

            var systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.BackRequested += OnBackRequested;
            systemNavigationManager.AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Visible;
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
            systemNavigationManager.AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Collapsed;
        }

        /// <summary>
        /// Navigates back when the back button is pressed.
        /// </summary>
        /// <param name="sender">The SystemNavigationManager.</param>
        /// <param name="e">The event arguments.</param>
        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame.GoBack();
            e.Handled = true;
        }

        /// <summary>
        /// Sets focus on the title field when it is loaded.
        /// </summary>
        /// <param name="sender">The title TextBox.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTitleLoaded(object sender, RoutedEventArgs e)
        {
            var field = sender as TextBox;
            field.Focus(FocusState.Programmatic);
            field.SelectionStart = field.Text.Length;
        }

        /// <summary>
        /// Updates the title when the field text is changed.
        /// </summary>
        /// <param name="sender">The TextBox.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTitleKeyUp(object sender, KeyRoutedEventArgs e)
        {
            Category.Name = (sender as TextBox).Text;
        }

        /// <summary>
        /// Adds an extra field to the Category.
        /// </summary>
        /// <param name="extra">The Extra to add.</param>
        private void AddExtra(Extra extra)
        {
            var viewModel = new ExtraItemViewModel(extra);
            viewModel.PropertyChanged += OnFieldPropertyChanged;
            Category.Extras.Add(viewModel);
        }

        /// <summary>
        /// Adds a flavor to the Category.
        /// </summary>
        /// <param name="flavor">The Flavor to add.</param>
        private void AddFlavor(Flavor flavor)
        {
            var viewModel = new FlavorItemViewModel(flavor);
            viewModel.PropertyChanged += OnFieldPropertyChanged;
            Category.Flavors.Add(viewModel);
            RadarItems.Add(viewModel);
        }

        /// <summary>
        /// Checks the status of the field when the IsDeleted property changes, removing empty
        /// deleted fields and updating the RadarGraph as needed.
        /// </summary>
        /// <param name="sender">The ViewModel.</param>
        /// <param name="e">The event arguments.</param>
        private void OnFieldPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDeleted")
            {
                if (sender is ExtraItemViewModel)
                {
                    var extra = sender as ExtraItemViewModel;
                    if (extra.IsDeleted && extra.Model.ID == 0
                        && string.IsNullOrWhiteSpace(extra.Name))
                    {
                        Category.Extras.Remove(extra);
                    }
                }
                else if (sender is FlavorItemViewModel)
                {
                    var flavor = sender as FlavorItemViewModel;
                    if (flavor.IsDeleted)
                    {
                        if (string.IsNullOrWhiteSpace(flavor.Name))
                        {
                            Category.Flavors.Remove(flavor);
                        }
                        RadarItems.Remove(flavor);
                    }
                    else
                    {
                        RadarItems.Insert((int)flavor.Model.Position, flavor);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a blank extra field when the add field Button is clicked.
        /// </summary>
        /// <param name="sender">The add field Button.</param>
        /// <param name="e">The event arguments.</param>
        private void OnAddExtra(object sender, RoutedEventArgs e)
        {
            if (Category.Extras.Count == 0
                || !string.IsNullOrWhiteSpace(Category.Extras.Last().Name))
            {
                AddExtra(new Extra());
            }
        }

        /// <summary>
        /// Adds a blank flavor field when the add flavor Button is clicked.
        /// </summary>
        /// <param name="sender">The add flavor Button.</param>
        /// <param name="e">The event arguments.</param>
        private void OnAddFlavor(object sender, RoutedEventArgs e)
        {
            if (Category.Flavors.Count == 0
                || !string.IsNullOrWhiteSpace(Category.Flavors.Last().Name))
            {
                AddFlavor(new Flavor() { Position = Category.Flavors.Count });
            }
        }

        /// <summary>
        /// Gives focus to a field when it is added.
        /// </summary>
        /// <param name="sender">The FieldEditor.</param>
        /// <param name="e">The event arguments.</param>
        private void OnFieldLoaded(object sender, RoutedEventArgs e)
        {
            var field = sender as FieldEditor;
            if (string.IsNullOrWhiteSpace(field.Value)
                && TitleField.FocusState == FocusState.Unfocused)
            {
                (field.FindName("Field") as TextBox).Focus(FocusState.Programmatic);
            }
        }

        /// <summary>
        /// Asynchronously loads the Category to edit.
        /// </summary>
        /// <param name="categoryId">The primary ID of the Category.</param>
        private async void LoadCategory(long categoryId)
        {
            var category =
                new CategoryViewModel(await DatabaseHelper.GetCategoryAsync(categoryId));

            foreach (var extra in await DatabaseHelper.GetCategoryExtrasAsync(categoryId))
            {
                category.Extras.Add(new ExtraItemViewModel(extra));
            }

            foreach (var flavor in await DatabaseHelper.GetCategoryFlavorsAsync(categoryId))
            {
                category.Flavors.Add(new FlavorItemViewModel(flavor));
            }

            Category = category;
        }

        /// <summary>
        /// Asynchronously updates the Category database record.
        /// </summary>
        /// <returns>Whether the update was successful.</returns>
        private async Task<bool> UpdateCategory()
        {
            var extras = new Collection<Extra>();
            var position = 0;
            foreach (var extra in Category.Extras)
            {
                extra.Model.Position = position++;
                extras.Add(extra.Model);
            }

            var flavors = new Collection<Flavor>();
            position = 0;
            foreach (var flavor in Category.Flavors)
            {
                if (!flavor.IsDeleted)
                {
                    flavor.Model.Position = position++;
                    flavors.Add(flavor.Model);
                }
            }

            return await DatabaseHelper.UpdateCategoryAsync(Category.Model, extras, flavors);
        }

        /// <summary>
        /// Called when the Delete Category is clicked in the CommandBar.
        /// </summary>
        /// <param name="sender">The AppBarButton.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnDeleteCategory(object sender, RoutedEventArgs e)
        {
            if (Category.Model.IsPreset)
            {
                return;
            }

            var result = await new DeleteCategoryDialog(Category.Model).ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                await DatabaseHelper.DeleteCategoryAsync(Category.Model);
                Frame.GoBack();
            }
        }

        /// <summary>
        /// Called when the Save Category is clicked in the CommandBar.
        /// </summary>
        /// <param name="sender">The AppBarButton.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnSaveCategory(object sender, RoutedEventArgs e)
        {
            if (await UpdateCategory())
            {
                Settings.ListCategory = Category.Model.ID;
                Frame.GoBack();
            }
        }
    }
}
