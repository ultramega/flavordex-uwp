using Flavordex.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Flavordex.UI.Controls
{
    /// <summary>
    /// Control for displaying a radar graph of Flavors.
    /// </summary>
    public sealed partial class RadarGraph : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// The minimum value an item can have.
        /// </summary>
        public static readonly int MinimumValue = 0;

        /// <summary>
        /// The maximum value an item can have.
        /// </summary>
        public static readonly int MaximumValue = 5;

        /// <summary>
        /// The list of RadarItems being rendered in the graph.
        /// </summary>
        private Collection<RadarItem> _items;

        /// <summary>
        /// Gets or sets the list of RadarItems to render in the graph.
        /// </summary>
        public Collection<RadarItem> Items
        {
            get { return (Collection<RadarItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(Collection<RadarItem>), typeof(RadarGraph), null);

        /// <summary>
        /// Gets the currently active items.
        /// </summary>
        private Collection<RadarItem> ActiveItems { get; } = new Collection<RadarItem>();

        /// <summary>
        /// Gets or sets whether the graph is in interactive mode.
        /// </summary>
        public bool IsInteractive
        {
            get { return (bool)GetValue(IsInteractiveProperty); }
            set { SetValue(IsInteractiveProperty, value); }
        }
        public static readonly DependencyProperty IsInteractiveProperty =
            DependencyProperty.Register("IsInteractive", typeof(bool), typeof(RadarGraph), new PropertyMetadata(false));

        /// <summary>
        /// The index of the currently selected item.
        /// </summary>
        private int _selectedIndex;

        /// <summary>
        /// Gets or sets the index of the currently selected item.
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(RadarGraph), new PropertyMetadata(-1));

        /// <summary>
        /// Gets or sets the currently selected RadarItem.
        /// </summary>
        public RadarItem SelectedItem
        {
            get { return (RadarItem)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(RadarItem), typeof(RadarGraph), null);

        /// <summary>
        /// Gets whether the graph currently has data.
        /// </summary>
        public bool HasItems
        {
            get
            {
                return ActiveItems.Count > 0;
            }
        }

        /// <summary>
        /// Gets the list of vertices of the Polyline graphing the values.
        /// </summary>
        private PointCollection PolygonPoints { get; } = new PointCollection();

        /// <summary>
        /// Gets or sets the radius of the outermost circle.
        /// </summary>
        private double OuterCircleRadius { get; set; }

        /// <summary>
        /// Gets or sets the position of the selection indicator.
        /// </summary>
        private Point SelectedPosition { get; set; }

        /// <summary>
        /// Points of all the intersections on the graph.
        /// </summary>
        private Point[,] _cachedPoints;

        /// <summary>
        /// The longest label; used for measuring the padding.
        /// </summary>
        private string _longestLabel;

        /// <summary>
        /// The amount of padding around the outer circle to make room for the labels.
        /// </summary>
        private Thickness _padding = new Thickness(0);

        /// <summary>
        /// The center point of the graph.
        /// </summary>
        private Point _center = new Point();

        /// <summary>
        /// The distance between values on the graph.
        /// </summary>
        private double _scale;

        /// <summary>
        /// Whether the graph is running an animation.
        /// </summary>
        private bool _isAnimating;

        /// <summary>
        /// Constructor.
        /// </summary>
        public RadarGraph()
        {
            InitializeComponent();
            RegisterPropertyChangedCallback(ItemsProperty, OnItemsPropertyChanged);
            RegisterPropertyChangedCallback(IsInteractiveProperty, OnIsInteractivePropertyChanged);
            RegisterPropertyChangedCallback(SelectedIndexProperty, OnSelectedIndexPropertyChanged);
            RegisterPropertyChangedCallback(SelectedItemProperty, OnSelectedItemPropertyChanged);
            RotateTransform.RegisterPropertyChangedCallback(RotateTransform.AngleProperty, OnAngleChanged);
        }

        /// <summary>
        /// Turns the graph to the next item.
        /// </summary>
        public void SelectNextItem()
        {
            if (IsInteractive)
            {
                SelectedIndex = SelectedIndex == ActiveItems.Count - 1 ? 0 : SelectedIndex + 1;
            }
        }

        /// <summary>
        /// Turns the graph to the previous item.
        /// </summary>
        public void SelectPreviousItem()
        {
            if (IsInteractive)
            {
                SelectedIndex = SelectedIndex == 0 ? ActiveItems.Count - 1 : SelectedIndex - 1;
            }
        }

        /// <summary>
        /// Reads the list of Items and renders the graph.
        /// </summary>
        private void ReadItems()
        {
            if (Items != null)
            {
                ActiveItems.Clear();
                foreach (var item in Items)
                {
                    if (!string.IsNullOrWhiteSpace(item.Name))
                    {
                        ActiveItems.Add(item);
                    }
                    item.PropertyChanged -= OnSingleItemPropertyChanged;
                    item.PropertyChanged += OnSingleItemPropertyChanged;
                }
            }

            AddLabelsAndLines();
            MeasurePadding();
            PositionElements();

            if (HasItems)
            {
                SelectedIndex = 0;
            }
            else
            {
                SelectedIndex = -1;
                PolygonPoints.Clear();
                PropertyChanged(this, new PropertyChangedEventArgs("PolygonPoints"));
            }
        }

        /// <summary>
        /// Adds the label TextBlocks and Lines to the layout.
        /// </summary>
        private void AddLabelsAndLines()
        {
            LineGrid.Children.Clear();
            LabelCanvas.Children.Clear();
            _longestLabel = "";

            if (!HasItems)
            {
                return;
            }

            foreach (var item in ActiveItems)
            {
                var text = new TextBlock()
                {
                    Text = item.Name,
                    FontSize = FontSize
                };
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                LabelCanvas.Children.Add(text);

                if (item.Name.Length > _longestLabel.Length)
                {
                    _longestLabel = item.Name;
                }

                LineGrid.Children.Add(new Line());
            }
        }

        /// <summary>
        /// Calculates the padding required based on the longest label.
        /// </summary>
        private void MeasurePadding()
        {
            if (_longestLabel == null || _longestLabel.Length == 0)
            {
                _padding.Left = _padding.Right = _padding.Top = _padding.Bottom = 0;
            }
            else
            {
                var text = new TextBlock()
                {
                    Text = _longestLabel,
                    FontSize = FontSize
                };
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                _padding.Left = _padding.Right = text.ActualWidth;
                _padding.Top = _padding.Bottom = text.ActualHeight;
            }
        }

        /// <summary>
        /// Calculates the position of all the points on the graph.
        /// </summary>
        private void CalculatePoints()
        {
            _center.X = ContentGrid.ActualWidth / 2;
            _center.Y = ContentGrid.ActualHeight / 2;

            if (HasItems)
            {
                _scale = (_center.X - _padding.Left) / MaximumValue;

                var count = ActiveItems.Count;
                _cachedPoints = new Point[count, MaximumValue + 1];
                for (var i = 0; i < count; i++)
                {
                    _cachedPoints[i, 0] = _center;

                    var angle = i * 2 * Math.PI / -count + Math.PI / 2;
                    var cos = Math.Cos(angle);
                    var sin = Math.Sin(angle);
                    var radius = 0.0;

                    for (var j = 1; j <= MaximumValue; j++)
                    {
                        radius = _scale * j;
                        _cachedPoints[i, j] = new Point()
                        {
                            X = _center.X + radius * cos,
                            Y = _center.Y - radius * sin
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Calculates and sets the position of all the elements in the graph.
        /// </summary>
        private void PositionElements()
        {
            PolygonPoints.Clear();

            CalculatePoints();

            OuterCircleRadius = Math.Max(0, _center.X - _padding.Left);
            PropertyChanged(this, new PropertyChangedEventArgs("OuterCircleRadius"));

            if (ActiveItems != null)
            {
                for (var i = 0; i < ActiveItems.Count; i++)
                {
                    var point = _cachedPoints[i, ActiveItems[i].Value];

                    PolygonPoints.Add(point);

                    if (i == _selectedIndex)
                    {
                        SelectedPosition = point;
                        PropertyChanged(this, new PropertyChangedEventArgs("SelectedPosition"));
                    }

                    var line = LineGrid.Children[i] as Line;
                    line.X1 = _cachedPoints[i, 0].X;
                    line.Y1 = _cachedPoints[i, 0].Y;
                    line.X2 = _cachedPoints[i, MaximumValue].X;
                    line.Y2 = _cachedPoints[i, MaximumValue].Y;
                }

                PositionLabels();

                if (PolygonPoints.Count > 2)
                {
                    PolygonPoints.Add(PolygonPoints[0]);
                }
            }

            PropertyChanged(this, new PropertyChangedEventArgs("PolygonPoints"));
        }

        /// <summary>
        /// Sets the positions of the label TextBlocks.
        /// </summary>
        private void PositionLabels()
        {
            var count = LabelCanvas.Children.Count;
            for (var i = 0; i < count; i++)
            {
                var offset = Math.PI / 2 + (360 - RotateTransform.Angle) * Math.PI / 180;
                var angle = i * 2 * Math.PI / -count + offset;
                var cos = Math.Cos(angle);
                var sin = Math.Sin(angle);
                var radius = _scale * (MaximumValue + 1);

                var text = LabelCanvas.Children[i] as TextBlock;

                var x = _center.X + (radius - _scale / 3) * cos;
                var y = _center.Y - radius * sin - _padding.Top / 2;

                if (Math.Abs(x - _center.X) < _scale)
                {
                    x -= text.ActualWidth / 2;
                }
                else if (x < _center.X)
                {
                    x -= text.ActualWidth;
                }

                Canvas.SetLeft(text, x);
                Canvas.SetTop(text, y);
            }
        }

        /// <summary>
        /// Rotates the graph to the specified item.
        /// </summary>
        private void TurnToSelectedItem()
        {
            if (_isAnimating)
            {
                return;
            }

            _isAnimating = true;

            var oldAngle = RotateTransform.Angle;
            var newAngle = 360 - (360D / ActiveItems.Count) * SelectedIndex;

            if (newAngle - oldAngle > 180)
            {
                newAngle -= 360;
            }

            RotateAnimation.To = newAngle;
            RotateStoryboard.Begin();
        }

        /// <summary>
        /// Calculates the FontSize and size of the Content container when the size of the Control
        /// changes.
        /// </summary>
        /// <param name="sender">This Control.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRootSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var size = Math.Min(e.NewSize.Width, e.NewSize.Height);

            FontSize = size / 24;

            MeasurePadding();

            ContentGrid.Width = size;
            ContentGrid.Height = size - (_padding.Left - _padding.Top);
        }

        /// <summary>
        /// Repositions the elements of the graph when the size of the Content container changes.
        /// </summary>
        /// <param name="sender">This Control.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            PositionElements();
        }

        /// <summary>
        /// Repositions the labels when the angle of the graph changes.
        /// </summary>
        /// <param name="sender">The RotateTransform.</param>
        /// <param name="dp">The Angle DependencyProperty.</param>
        private void OnAngleChanged(DependencyObject sender, DependencyProperty dp)
        {
            PositionLabels();
        }

        /// <summary>
        /// Corrects angle overflows when a rotation animation completes.
        /// </summary>
        /// <param name="sender">The Storyboard.</param>
        /// <param name="e">The event argument.</param>
        private void OnRotationCompleted(object sender, object e)
        {
            if (RotateTransform.Angle >= 360)
            {
                RotateTransform.Angle -= 360;
            }
            else if (RotateTransform.Angle < 0)
            {
                RotateTransform.Angle += 360;
            }

            _isAnimating = false;
        }

        /// <summary>
        /// Updates the graph when the Items property changes.
        /// </summary>
        /// <param name="sender">The RadarGraph.</param>
        /// <param name="dp">The ItemsProperty.</param>
        private void OnItemsPropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (_items != null)
            {
                foreach (var item in _items)
                {
                    item.PropertyChanged -= OnSingleItemPropertyChanged;
                }

                if (_items is ObservableCollection<RadarItem>)
                {
                    (_items as ObservableCollection<RadarItem>).CollectionChanged -= OnItemsCollectionChanged;
                }
            }

            _items = Items;

            if (Items is ObservableCollection<RadarItem>)
            {
                (Items as ObservableCollection<RadarItem>).CollectionChanged += OnItemsCollectionChanged;
            }

            ReadItems();
        }

        /// <summary>
        /// Updates the graph when the Collection of Items changes.
        /// </summary>
        /// <param name="sender">The ObservableCollection.</param>
        /// <param name="e">The event arguments.</param>
        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (RadarItem item in e.OldItems)
                {
                    item.PropertyChanged -= OnSingleItemPropertyChanged;
                }
            }

            ReadItems();
        }

        /// <summary>
        /// Updates the graph when an item changes.
        /// </summary>
        /// <param name="sender">The RadarItem.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSingleItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as RadarItem;
            var index = ActiveItems.IndexOf(sender as FlavorItemViewModel);
            if (e.PropertyName.Equals("Name"))
            {
                if (index == -1 && !string.IsNullOrWhiteSpace(item.Name))
                {
                    ReadItems();
                    return;
                }
                else if (index > -1 && string.IsNullOrWhiteSpace(item.Name))
                {
                    ReadItems();
                    return;
                }

                (LabelCanvas.Children[index] as TextBlock).Text = item.Name;
                if (Name.Length > _longestLabel.Length)
                {
                    _longestLabel = Name;
                    MeasurePadding();
                    PositionElements();
                }
                else
                {
                    PositionLabels();
                }
            }
            else if (e.PropertyName.Equals("Value") && index > -1)
            {
                var value = _items[index].Value;
                PolygonPoints[index] = _cachedPoints[index, value];
                PolygonPoints[PolygonPoints.Count - 1] = PolygonPoints[0];
                PropertyChanged(this, new PropertyChangedEventArgs("PolygonPoints"));
                if (index == _selectedIndex)
                {
                    SelectedPosition = _cachedPoints[index, value];
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedPosition"));
                }
            }
        }

        /// <summary>
        /// Updates the graph to indicate the selected item when the selected SelectedIndex
        /// property changes.
        /// </summary>
        /// <param name="sender">The RadarGraph.</param>
        /// <param name="dp">The SelectedIndexProperty.</param>
        private void OnSelectedIndexPropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (_selectedIndex > -1 && _selectedIndex < LineGrid.Children.Count)
            {
                (LineGrid.Children[_selectedIndex] as FrameworkElement).Style = DefaultLineStyle;
                (LabelCanvas.Children[_selectedIndex] as FrameworkElement).Style = DefaultTextStyle;
            }

            if (!HasItems || SelectedIndex == -1)
            {
                _selectedIndex = SelectedIndex;
                SelectedItem = null;
                RotateTransform.Angle = 0;
            }
            else
            {
                if (SelectedIndex < 0 || SelectedIndex >= ActiveItems.Count)
                {
                    SelectedIndex = _selectedIndex;
                    return;
                }

                if (IsInteractive)
                {
                    (LineGrid.Children[SelectedIndex] as FrameworkElement).Style = SelectedLineStyle;
                    (LabelCanvas.Children[SelectedIndex] as FrameworkElement).Style = SelectedLabelStyle;
                }

                SelectedPosition = PolygonPoints[SelectedIndex];
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedPosition"));

                _selectedIndex = SelectedIndex;
                SelectedItem = ActiveItems[SelectedIndex];
                TurnToSelectedItem();
            }
        }

        /// <summary>
        /// Sets the selected index to match the selected item when the SelectedItem property
        /// changes.
        /// </summary>
        /// <param name="sender">The RadarGraph.</param>
        /// <param name="dp">The SelectedItemProperty.</param>
        private void OnSelectedItemPropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (!HasItems || SelectedItem == null)
            {
                SelectedIndex = -1;
                SelectedItem = null;
            }
            else if (SelectedIndex == -1 || ActiveItems[SelectedIndex] != SelectedItem)
            {
                SelectedIndex = ActiveItems.IndexOf(SelectedItem);
            }
        }

        /// <summary>
        /// Updates the graph to reflect the interactive state when the IsInteractive property
        /// changes.
        /// </summary>
        /// <param name="sender">The RadarGraph.</param>
        /// <param name="dp">The IsInteractiveProperty.</param>
        private void OnIsInteractivePropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (IsInteractive)
            {
                VisualStateManager.GoToState(this, "InteractiveState", true);
                if (SelectedIndex > -1 && LineGrid.Children.Count > SelectedIndex)
                {
                    (LineGrid.Children[SelectedIndex] as FrameworkElement).Style = SelectedLineStyle;
                    (LabelCanvas.Children[SelectedIndex] as FrameworkElement).Style = SelectedLabelStyle;
                }
            }
            else
            {
                VisualStateManager.GoToState(this, "NormalState", true);
                if (SelectedIndex > -1 && LineGrid.Children.Count > SelectedIndex)
                {
                    (LineGrid.Children[SelectedIndex] as FrameworkElement).Style = DefaultLineStyle;
                    (LabelCanvas.Children[SelectedIndex] as FrameworkElement).Style = DefaultTextStyle;
                }
                SelectedIndex = 0;
            }
        }
    }
}
