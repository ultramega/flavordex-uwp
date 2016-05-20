using System;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Bratched.Tools.RatingControl
{
    public sealed partial class RateItem : UserControl
    {
        private const int RATEITEM_SIZE = 100;

        public RateItem()
        {
            InitializeComponent();
            Width = RATEITEM_SIZE;
            Height = RATEITEM_SIZE;
        }

        private double _value;
        private SolidColorBrush _fullBackgroundColor;
        private SolidColorBrush _emptyBackgroundColor;
        private SolidColorBrush _fullOutlineColor;
        private SolidColorBrush _emptyOutlineColor;
        private double _fullOutlineThickness;
        private double _emptyOutlineThickness;
        private string _fullPathData;
        private string _emptyPathData;

        /// <summary>
        /// Value of the rate item must be between 0 and 1.
        /// </summary>
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                RectRateFull.Rect = new Rect(0, 0, Width * value, Height);
            }
        }

        /// <summary>
        /// Backgroud Color of the full rate item
        /// </summary>
        public SolidColorBrush FullBackgroundColor
        {
            get { return _fullBackgroundColor; }
            set
            {
                _fullBackgroundColor = value;
                rateFull.Fill = value;
            }
        }

        /// <summary>
        /// Background Color of the empty rate item
        /// </summary>
        public SolidColorBrush EmptyBackgroundColor
        {
            get { return _emptyBackgroundColor; }
            set
            {
                _emptyBackgroundColor = value;
                rateEmpty.Fill = value;
            }
        }

        /// <summary>
        /// Outline color of the full rate item
        /// </summary>
        public SolidColorBrush FullOutlineColor
        {
            get { return _fullOutlineColor; }
            set
            {
                _fullOutlineColor = value;
                rateFull.Stroke = value;
            }
        }

        /// <summary>
        /// Outline color of the empty rate item
        /// </summary>
        public SolidColorBrush EmptyOutlineColor
        {
            get { return _emptyOutlineColor; }
            set
            {
                _emptyOutlineColor = value;
                rateEmpty.Stroke = value;
            }
        }

        /// <summary>
        /// Outline stroke thikness of the full rate item
        /// </summary>
        public double FullOutlineThickness
        {
            get { return _fullOutlineThickness; }
            set
            {
                _fullOutlineThickness = value;
                rateFull.StrokeThickness = value;
            }
        }

        /// <summary>
        /// Outline stroke thikness of the empty rate item
        /// </summary>
        public double EmptyOutlineThickness
        {
            get { return _emptyOutlineThickness; }
            set
            {
                _emptyOutlineThickness = value;
                rateEmpty.StrokeThickness = value;
            }
        }

        /// <summary>
        /// Data Geometry Path for the full rate item
        /// </summary>
        public string FullPathData
        {
            get { return _fullPathData; }
            set
            {
                _fullPathData = value;
                rateFull.Data = StringToPath(value);
            }
        }

        /// <summary>
        /// Data Geometry Path for the empty rate item
        /// </summary>
        public string EmptyPathData
        {
            get { return _emptyPathData; }
            set
            {
                _emptyPathData = value;
                rateEmpty.Data = StringToPath(value);
            }
        }

        /// <summary>
        /// Convert string Data Path to Geometrey Data Path
        /// </summary>
        /// <param name="pathData">string data path</param>
        /// <returns></returns>
        private static Geometry StringToPath(string pathData)
        {
            if (String.IsNullOrEmpty(pathData)) return null;
            string xamlPath =
                "<Geometry xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>"
                + pathData + "</Geometry>";
            return XamlReader.Load(xamlPath) as Geometry;
        }

    }
}
