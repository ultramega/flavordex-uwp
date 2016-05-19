using Flavordex.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Flavordex.UI.Controls
{
    /// <summary>
    /// Control to set the date and time of a journal entry.
    /// </summary>
    public sealed partial class EntryTimePicker : UserControl
    {
        /// <summary>
        /// Gets or sets the Entry to edit.
        /// </summary>
        public EntryViewModel Entry
        {
            get { return (EntryViewModel)GetValue(EntryProperty); }
            set { SetValue(EntryProperty, value); }
        }
        public static readonly DependencyProperty EntryProperty =
            DependencyProperty.Register("Entry", typeof(EntryViewModel), typeof(EntryTimePicker), null);

        /// <summary>
        /// Gets or sets the header of the Control.
        /// </summary>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(EntryTimePicker), null);

        /// <summary>
        /// Constructor.
        /// </summary>
        public EntryTimePicker()
        {
            InitializeComponent();
            DatePicker.Date = DatePicker.MaxDate = DateTimeOffset.Now;
            RegisterPropertyChangedCallback(EntryProperty, OnEntryPropertyChanged);
        }

        /// <summary>
        /// Sets the values of the pickers when the Entry property changes.
        /// </summary>
        /// <param name="sender">This Control.</param>
        /// <param name="dp">The EntryProperty.</param>
        private void OnEntryPropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (Entry != null)
            {
                DatePicker.Date = Entry.Date;
                TimePicker.Time = Entry.Date.TimeOfDay;
            }
        }

        /// <summary>
        /// Sets the date of the Entry when the date picker's date changes.
        /// </summary>
        /// <param name="sender">The DatePicker.</param>
        /// <param name="args">The event arguments.</param>
        private void OnDateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (!args.NewDate.HasValue)
            {
                DatePicker.Date = args.OldDate;
                return;
            }

            if (Entry != null)
            {
                Entry.Date = args.NewDate.Value.DateTime;
            }
        }

        /// <summary>
        /// Sets the time of the Entry when the time picker's time changes.
        /// </summary>
        /// <param name="sender">The TimePicker.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            if (Entry != null)
            {
                var date = Entry.Date;
                var time = e.NewTime;
                Entry.Date = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);
                DatePicker.Date = Entry.Date;
            }
        }
    }
}
