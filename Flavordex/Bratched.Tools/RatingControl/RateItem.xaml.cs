/*
  Microsoft Public License (MS-PL)

  This license governs use of the accompanying software. If you use the software, you
  accept this license. If you do not accept the license, do not use the software.

  1. Definitions
  The terms "reproduce," "reproduction," "derivative works," and "distribution" have the
  same meaning here as under U.S. copyright law.
  A "contribution" is the original software, or any additions or changes to the software.
  A "contributor" is any person that distributes its contribution under this license.
  "Licensed patents" are a contributor's patent claims that read directly on its contribution.

  2. Grant of Rights
  (A) Copyright Grant- Subject to the terms of this license, including the license conditions and
  limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free
  copyright license to reproduce its contribution, prepare derivative works of its contribution,
  and distribute its contribution or any derivative works that you create.
  (B) Patent Grant- Subject to the terms of this license, including the license conditions and
  limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free
  license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or
  otherwise dispose of its contribution in the software or derivative works of the contribution in
  the software.

  3. Conditions and Limitations
  (A) No Trademark License- This license does not grant you rights to use any contributors' name,
  logo, or trademarks.
  (B) If you bring a patent claim against any contributor over patents that you claim are infringed
  by the software, your patent license from such contributor to the software ends automatically.
  (C) If you distribute any portion of the software, you must retain all copyright, patent,
  trademark, and attribution notices that are present in the software.
  (D) If you distribute any portion of the software in source code form, you may do so only under
  this license by including a complete copy of this license with your distribution. If you
  distribute any portion of the software in compiled or object code form, you may only do so under
  a license that complies with this license.
  (E) The software is licensed "as-is." You bear the risk of using it. The contributors give no
  express warranties, guarantees or conditions. You may have additional consumer rights under your
  local laws which this license cannot change. To the extent permitted under your local laws, the
  contributors exclude the implied warranties of merchantability, fitness for a particular purpose
  and non-infringement.
*/
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
