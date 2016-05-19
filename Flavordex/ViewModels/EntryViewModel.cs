using Flavordex.Models;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Flavordex.ViewModels
{
    /// <summary>
    /// Represents a generic Entry.
    /// </summary>
    public class EntryViewModel : ModelViewModel<Entry>
    {
        /// <summary>
        /// Gets or sets the title of the Entry.
        /// </summary>
        public string Title
        {
            get
            {
                return Model.Title;
            }
            set
            {
                Model.Title = value;
                RaisePropertyChanged();
                RaisePropertyChanged("IsValid");
            }
        }

        /// <summary>
        /// Gets or sets the name of the Maker for the Entry.
        /// </summary>
        public string Maker
        {
            get
            {
                return Model.Maker;
            }
            set
            {
                Model.Maker = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the location name of the Maker for the Entry.
        /// </summary>
        public string Origin
        {
            get
            {
                return Model.Origin;
            }
            set
            {
                Model.Origin = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the price of the Entry.
        /// </summary>
        public string Price
        {
            get
            {
                return Model.Price;
            }
            set
            {
                Model.Price = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the location of the Entry.
        /// </summary>
        public string Location
        {
            get
            {
                return Model.Location;
            }
            set
            {
                Model.Location = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the date of the Entry.
        /// </summary>
        public DateTime Date
        {
            get
            {
                return Model.Date;
            }
            set
            {
                Model.Date = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the rating of the Entry.
        /// </summary>
        public double Rating
        {
            get
            {
                return Model.Rating;
            }
            set
            {
                Model.Rating = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the notes for the Entry.
        /// </summary>
        public string Notes
        {
            get
            {
                return Model.Notes;
            }
            set
            {
                Model.Notes = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets whether the Entry has valid data.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Model.Title);
            }
        }

        /// <summary>
        /// Gets the custom extra fields for the Entry.
        /// </summary>
        public EntryExtraItemViewModel[] CustomFields
        {
            get
            {
                if (_extras == null)
                {
                    return null;
                }
                return _extras.Where(e => !e.Model.IsPreset).ToArray();
            }
        }

        /// <summary>
        /// The list of ExtraItemViewModels for the Entry.
        /// </summary>
        private ObservableCollection<EntryExtraItemViewModel> _extras = new ObservableCollection<EntryExtraItemViewModel>();

        /// <summary>
        /// Gets or sets the list of ExtraItemViewModels for the Entry.
        /// </summary>
        public Collection<EntryExtraItemViewModel> Extras
        {
            get
            {
                return _extras;
            }
            set
            {
                _extras.CollectionChanged -= OnExtrasCollectionChanged;
                _extras = new ObservableCollection<EntryExtraItemViewModel>(value);
                _extras.CollectionChanged += OnExtrasCollectionChanged;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entry">The Entry to represent.</param>
        public EntryViewModel(Entry entry) : base(entry) { }

        /// <summary>
        /// Raises a PropertyChanged event for the CustomFields when the Extras collection changes.
        /// </summary>
        /// <param name="sender">The Extras ObservableCollection.</param>
        /// <param name="e">The event arguments.</param>
        private void OnExtrasCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("CustomFields");
        }

        /// <summary>
        /// Gets the value of an extra field.
        /// </summary>
        /// <param name="name">The name of the extra field.</param>
        /// <returns>The value of the extra field.</returns>
        protected string GetExtra(string name)
        {
            var extra = Extras.Where(e => e.Name == name).FirstOrDefault();
            if (extra != null)
            {
                return extra.Value;
            }
            return null;
        }

        /// <summary>
        /// Sets the value of an extra field.
        /// </summary>
        /// <param name="name">The name of the extra field.</param>
        /// <param name="value">The value of the extra field.</param>
        protected void SetExtra(string name, object value)
        {
            var extra = Extras.Where(e => e.Name == name).FirstOrDefault();
            if (extra == null)
            {
                var newExtra = new EntryExtra();
                newExtra.Name = name;
                newExtra.Value = value.ToString();
                Extras.Add(new EntryExtraItemViewModel(newExtra));
            }
            else
            {
                extra.Value = value.ToString();
            }
        }
    }
}
