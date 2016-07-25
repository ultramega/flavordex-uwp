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
            DependencyProperty.Register("Entry", typeof(EntryViewModel), typeof(EntryTimePicker),
                null);

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
        private void OnDateChanged(CalendarDatePicker sender,
            CalendarDatePickerDateChangedEventArgs args)
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
                Entry.Date = new DateTime(date.Year, date.Month, date.Day, time.Hours,
                    time.Minutes, time.Seconds);
                DatePicker.Date = Entry.Date;
            }
        }
    }
}
