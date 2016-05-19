using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Flavordex.UI.Controls
{
    /// <summary>
    /// Control to display the interface for filtering the entry list.
    /// </summary>
    public sealed partial class EntryListFilterForm : UserControl
    {
        /// <summary>
        /// Gets or sets the value of the Title TextBox.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(EntryListFilterForm), null);

        /// <summary>
        /// Gets or sets the value of the Maker TextBox.
        /// </summary>
        public string Maker
        {
            get { return (string)GetValue(MakerProperty); }
            set { SetValue(MakerProperty, value); }
        }
        public static readonly DependencyProperty MakerProperty =
            DependencyProperty.Register("Maker", typeof(string), typeof(EntryListFilterForm), null);

        /// <summary>
        /// Gets or sets the value of the Origin TextBox.
        /// </summary>
        public string Origin
        {
            get { return (string)GetValue(OriginProperty); }
            set { SetValue(OriginProperty, value); }
        }
        public static readonly DependencyProperty OriginProperty =
            DependencyProperty.Register("Origin", typeof(string), typeof(EntryListFilterForm), null);

        /// <summary>
        /// Gets or sets the value of the Location TextBox.
        /// </summary>
        public string Location
        {
            get { return (string)GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }
        public static readonly DependencyProperty LocationProperty =
            DependencyProperty.Register("Location", typeof(string), typeof(EntryListFilterForm), null);

        /// <summary>
        /// Gets or sets the value of the Starting Date CalendarDatePicker.
        /// </summary>
        public DateTimeOffset? StartDate
        {
            get { return (DateTimeOffset?)GetValue(StartDateProperty); }
            set { SetValue(StartDateProperty, value); }
        }
        public static readonly DependencyProperty StartDateProperty =
            DependencyProperty.Register("StartDate", typeof(DateTimeOffset?), typeof(EntryListFilterForm), null);

        /// <summary>
        /// Gets or sets the value of the Ending Date CalendarDatePicker.
        /// </summary>
        public DateTimeOffset? EndDate
        {
            get { return (DateTimeOffset?)GetValue(EndDateProperty); }
            set { SetValue(EndDateProperty, value); }
        }
        public static readonly DependencyProperty EndDateProperty =
            DependencyProperty.Register("EndDate", typeof(DateTimeOffset?), typeof(EntryListFilterForm), null);

        /// <summary>
        /// Constructor.
        /// </summary>
        public EntryListFilterForm()
        {
            InitializeComponent();
            StartDatePicker.MaxDate = EndDatePicker.MaxDate = DateTimeOffset.Now;
            RegisterPropertyChangedCallback(StartDateProperty, OnDatePropertyChange);
            RegisterPropertyChangedCallback(EndDateProperty, OnDatePropertyChange);
        }

        /// <summary>
        /// Sets the value of the CalendarDatePickers when a date property changes.
        /// </summary>
        /// <param name="sender">This Control.</param>
        /// <param name="dp">The DependencyProperty that changed.</param>
        private void OnDatePropertyChange(DependencyObject sender, DependencyProperty dp)
        {
            if (dp == StartDateProperty)
            {
                StartDatePicker.Date = StartDate;
            }
            else if (dp == EndDateProperty)
            {
                EndDatePicker.Date = EndDate;
            }
        }

        /// <summary>
        /// Updates the StartDate property when the StartDatePicker's date changes and ensures the
        /// EndDate is after the StartDate.
        /// </summary>
        /// <param name="sender">The StartDatePicker.</param>
        /// <param name="args">The event arguments.</param>
        private void OnStartDateChange(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (args.OldDate.Equals(args.NewDate))
            {
                return;
            }
            StartDate = args.NewDate;
            if (StartDate > EndDate)
            {
                EndDate = StartDate;
            }
        }

        /// <summary>
        /// Updates the EndDate property when the EndDatePicker's date changes and ensures the
        /// EndDate is after the StartDate.
        /// </summary>
        /// <param name="sender">The EndDatePicker.</param>
        /// <param name="args">The event arguments.</param>
        private void OnEndDateChange(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (args.OldDate.Equals(args.NewDate))
            {
                return;
            }
            EndDate = args.NewDate;
            if (StartDate > EndDate)
            {
                StartDate = EndDate;
            }
        }
    }
}
