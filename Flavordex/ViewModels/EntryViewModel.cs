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
        /// The list of Makers backing the maker suggestions.
        /// </summary>
        private Collection<Maker> _makers;

        /// <summary>
        /// Gets the list of maker suggestions.
        /// </summary>
        public Maker[] MakerSuggestions
        {
            get
            {
                if (_makers == null || string.IsNullOrWhiteSpace(Maker))
                {
                    return null;
                }
                return _makers.Where(e => e.Name.ToLower().Contains(Maker.ToLower())).ToArray();
            }
        }

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
                return !string.IsNullOrWhiteSpace(Model.Maker) ? Model.Maker : null;
            }
            set
            {
                Model.Maker = value;
                RaisePropertyChanged();
                RaisePropertyChanged("MakerSuggestions");
            }
        }

        /// <summary>
        /// Gets or sets the location name of the Maker for the Entry.
        /// </summary>
        public string Origin
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Model.Origin) ? Model.Origin : null;
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
                return !string.IsNullOrWhiteSpace(Model.Price) ? Model.Price : null;
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
                return !string.IsNullOrWhiteSpace(Model.Location) ? Model.Location : null;
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
                return _extras.Where(e => !e.Model.IsPreset
                && !string.IsNullOrWhiteSpace(e.Value)).ToArray();
            }
        }

        /// <summary>
        /// Gets the editable custom extra fields for the Entry.
        /// </summary>
        public EntryExtraItemViewModel[] EditableCustomFields
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
        private ObservableCollection<EntryExtraItemViewModel> _extras =
            new ObservableCollection<EntryExtraItemViewModel>();

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
        public EntryViewModel(Entry entry) : base(entry)
        {
            _extras.CollectionChanged += OnExtrasCollectionChanged;
        }

        /// <summary>
        /// Gets a new instance of a EntryViewModel of the type specific to the Entry's category.
        /// </summary>
        /// <param name="entry">The Entry to represent.</param>
        /// <returns>The EntryViewModel.</returns>
        public static EntryViewModel GetInstance(Entry entry)
        {
            switch (entry.Category)
            {
                case Constants.CAT_BEER:
                    return new BeerEntryViewModel(entry);
                case Constants.CAT_COFFEE:
                    return new CoffeeEntryViewModel(entry);
                case Constants.CAT_WHISKEY:
                    return new WhiskeyEntryViewModel(entry);
                case Constants.CAT_WINE:
                    return new WineEntryViewModel(entry);
            }
            return new EntryViewModel(entry);
        }

        /// <summary>
        /// Sets the list of Makers backing the maker suggestions.
        /// </summary>
        /// <param name="makers">The Collection of Makers.</param>
        public void SetMakers(Collection<Maker> makers)
        {
            _makers = makers;
            RaisePropertyChanged("MakerSuggestions");
        }

        /// <summary>
        /// Raises a PropertyChanged event for the CustomFields when the Extras collection changes.
        /// </summary>
        /// <param name="sender">The Extras ObservableCollection.</param>
        /// <param name="e">The event arguments.</param>
        private void OnExtrasCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("CustomFields");
            RaisePropertyChanged("EditableCustomFields");
        }

        /// <summary>
        /// Gets the value of an extra field.
        /// </summary>
        /// <param name="name">The name of the extra field.</param>
        /// <returns>The value of the extra field.</returns>
        protected string GetExtra(string name)
        {
            var extra = Extras.FirstOrDefault(e => e.Name == name);
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
            var extra = Extras.FirstOrDefault(e => e.Name == name);
            if (extra == null && value != null)
            {
                var newExtra = new EntryExtra();
                newExtra.Name = name;
                newExtra.Value = value.ToString();
                Extras.Add(new EntryExtraItemViewModel(newExtra));
            }
            else if (extra != null)
            {
                extra.Value = value != null ? value.ToString() : "";
            }
        }

        /// <summary>
        /// Makes any adjustments to the ViewModel to be used in the search form.
        /// </summary>
        public virtual void EnableSearchMode() { }
    }
}
