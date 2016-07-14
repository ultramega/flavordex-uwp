using Flavordex.Models;
using Flavordex.Models.Data;
using Flavordex.Utilities.Databases;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Flavordex.ViewModels
{
    /// <summary>
    /// Contains entry search parameters.
    /// </summary>
    public class SearchViewModel : ViewModel
    {
        /// <summary>
        /// The types of comparisons to be used in the where clause.
        /// </summary>
        private struct Comparison
        {
            public const string Like = "LIKE";
            public const string Equal = "=";
        }

        /// <summary>
        /// The ViewModel containing the main search parameters.
        /// </summary>
        private EntryViewModel _entry = new EntryViewModel(new Entry());

        /// <summary>
        /// Gets or sets the ViewModel containing the main search parameters.
        /// </summary>
        public EntryViewModel Entry
        {
            get
            {
                return _entry;
            }
            set
            {
                _entry = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The start date.
        /// </summary>
        private DateTimeOffset? _startDate;

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTimeOffset? StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The end date.
        /// </summary>
        private DateTimeOffset? _endDate;

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public DateTimeOffset? EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The minimum rating.
        /// </summary>
        private double _minimumRating = 0;

        /// <summary>
        /// Gets or sets the minimum rating.
        /// </summary>
        public double MinimumRating
        {
            get
            {
                return _minimumRating;
            }
            set
            {
                _minimumRating = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The maximum rating.
        /// </summary>
        private double _maximumRating = 5;

        /// <summary>
        /// Gets or sets the maximum rating.
        /// </summary>
        public double MaximumRating
        {
            get
            {
                return _maximumRating;
            }
            set
            {
                _maximumRating = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The where clause.
        /// </summary>
        private StringBuilder _where = new StringBuilder();

        /// <summary>
        /// The arguments for the where clause.
        /// </summary>
        private Collection<object> _whereArgs = new Collection<object>();

        /// <summary>
        /// Checks whether a journal entry matches the current search parameters.
        /// </summary>
        /// <param name="entry">
        /// The journal entry to check.
        /// </param>
        /// <returns>Whether the journal entry matches the current search parameters.</returns>
        public async Task<bool> Matches(Entry entry)
        {
            var where = BaseColumns._ID + " = ?";
            if (_where.Length > 0)
            {
                where += " AND (" + _where.ToString() + ")";
            }
            var whereArgs = new object[_whereArgs == null ? 1 : _whereArgs.Count + 1];
            whereArgs[0] = entry.ID;
            if (_whereArgs != null)
            {
                _whereArgs.CopyTo(whereArgs, 1);
            }
            return (await DatabaseHelper.GetEntryListAsync(where, whereArgs)).Count > 0;
        }

        /// <summary>
        /// Gets the list of journal entries matching the current search parameters.
        /// </summary>
        /// <returns>The list of journal entries matching the current search parameters.</returns>
        public async Task<Collection<Entry>> GetList()
        {
            ParseFields();
            return await DatabaseHelper.GetEntryListAsync(_where.ToString(), _whereArgs.ToArray());
        }

        /// <summary>
        /// Parses all the searchable fields and generates the where clause and arguments.
        /// </summary>
        private void ParseFields()
        {
            _where.Clear();
            _whereArgs.Clear();

            ParseTextField(Entry.Title, Tables.Entries.TITLE);
            ParseTextField(Entry.Maker, Tables.Entries.MAKER);
            ParseTextField(Entry.Origin, Tables.Entries.ORIGIN);
            ParseTextField(Entry.Price, Tables.Entries.PRICE);
            ParseTextField(Entry.Location, Tables.Entries.LOCATION);
            ParseTextField(Entry.Notes, Tables.Entries.NOTES);

            if (_startDate.HasValue || _endDate.HasValue)
            {
                if (_startDate.HasValue)
                {
                    _where.Append(Tables.Entries.DATE).Append(" >= ? AND ");
                    _whereArgs.Add(_startDate.Value);
                }
                if (_endDate.HasValue)
                {
                    _where.Append(Tables.Entries.DATE).Append(" < ? AND ");
                    _whereArgs.Add(_endDate.Value.AddDays(1));
                }
            }

            if (_minimumRating > 0)
            {
                _where.Append(Tables.Entries.RATING).Append(" >= ? AND ");
                _whereArgs.Add(_minimumRating);
            }
            if (_maximumRating < 5)
            {
                _where.Append(Tables.Entries.RATING).Append(" <= ? AND ");
                _whereArgs.Add(_maximumRating);
            }

            foreach (var extra in Entry.Extras)
            {
                if (!extra.Model.IsPreset || !ParsePresetField(extra))
                {
                    ParseExtraField(extra, Comparison.Like);
                }
            }

            if (_where.Length > 5)
            {
                _where.Remove(_where.Length - 5, 5);
            }

            if (Entry.Model.CategoryID > 0)
            {
                if (_where.Length > 0)
                {
                    _where.Insert(0, Tables.Entries.CAT_ID + " = ? AND (").Append(')');
                }
                else
                {
                    _where.Append(Tables.Entries.CAT_ID + " = ?");
                }
                _whereArgs.Insert(0, Entry.Model.CategoryID);
            }
        }

        /// <summary>
        /// Parses a preset extra field.
        /// </summary>
        /// <param name="extra">The extra field to parse.</param>
        /// <returns>Whether the extra field was handled.</returns>
        private bool ParsePresetField(EntryExtraItemViewModel extra)
        {
            switch (extra.Model.Name)
            {
                case Tables.Extras.Beer.SERVING:
                case Tables.Extras.Coffee.BREW_METHOD:
                    if (extra.Value != "0")
                    {
                        ParseExtraField(extra, Comparison.Equal);
                    }
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Parses a regular text field.
        /// </summary>
        /// <param name="field">The value of the text field.</param>
        /// <param name="fieldName">The name of the text field.</param>
        private void ParseTextField(string field, string fieldName)
        {
            if (!string.IsNullOrWhiteSpace(field))
            {
                _where.Append('(');
                foreach (var word in Regex.Split(field, "\\s+"))
                {
                    _where.Append(fieldName).Append(" LIKE ? AND ");
                    _whereArgs.Add('%' + word + '%');
                }
                _where.Remove(_where.Length - 5, 5);
                _where.Append(") AND ");
            }
        }

        /// <summary>
        /// Parses an extra text field.
        /// </summary>
        /// <param name="extra">The extra field.</param>
        /// <param name="comparison">The type of comparison to perform.</param>
        private void ParseExtraField(EntryExtraItemViewModel extra, string comparison)
        {
            if (!string.IsNullOrWhiteSpace(extra.Value))
            {
                _where.Append("(SELECT 1 FROM ").Append(Tables.EntriesExtras.TABLE_NAME).Append(" WHERE ").Append(Tables.EntriesExtras.EXTRA).Append(" = ? AND ");
                _whereArgs.Add(extra.Model.ExtraID);
                if (comparison == Comparison.Like)
                {
                    foreach (var word in Regex.Split(extra.Value, "\\s+"))
                    {
                        _where.Append(Tables.EntriesExtras.VALUE).Append(" LIKE ? AND ");
                        _whereArgs.Add('%' + word + '%');
                    }
                }
                else
                {
                    _where.Append(Tables.EntriesExtras.VALUE).Append(' ').Append(comparison).Append(" ? AND ");
                    _whereArgs.Add(extra.Value);
                }
                _where.Remove(_where.Length - 4, 4);
                _where.Append("LIMIT 1) AND ");
            }
        }
    }
}
