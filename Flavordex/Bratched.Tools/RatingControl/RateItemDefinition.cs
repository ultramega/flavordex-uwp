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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Bratched.Tools.RatingControl
{
    public partial class RateItemDefinition : FrameworkElement, IRateItemDefinition//  DependencyObject //
    {
        public RateItemDefinition()
        {

        }

        public static RatingControl ParentRatingControl { get; set; }

        /// <summary>
        /// Background Color for the rate item
        /// </summary>
        public SolidColorBrush BackgroundColor
        {
            get { return (SolidColorBrush)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        /// <summary>
        /// Outline Color for the rate item
        /// </summary>
        public SolidColorBrush OutlineColor
        {
            get { return (SolidColorBrush)GetValue(OutlineColorProperty); }
            set { SetValue(OutlineColorProperty, value); }
        }

        /// <summary>
        /// Outline thikness for the rate Item
        /// </summary>
        public object OutlineThickness
        {
            get { return GetValue(OutlineThicknessProperty); }
            set { SetValue(OutlineThicknessProperty, value); }
        }

        /// <summary>
        /// defines rate aspect
        /// </summary>
        public object PathData
        {
            get { return GetValue(PathDataProperty); }
            set { SetValue(PathDataProperty, value); }
        }

        /// <summary>
        /// Generate event change and redraw RatingControl
        /// </summary>
        /// <param name="d">define rate element</param>
        /// <param name="e">value property event </param>
        private static void AspectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Binding && ParentRatingControl != null)
                BindingOperations.SetBinding(d, BackgroundColorProperty, (Binding)e.NewValue);
            if (ParentRatingControl != null)
                ParentRatingControl.GenerateItems();
        }

        private static void AspectChanged2(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Binding && ParentRatingControl != null)
                BindingOperations.SetBinding(d, OutlineColorProperty, (Binding)e.NewValue);
            if (ParentRatingControl != null)
                ParentRatingControl.GenerateItems();
        }

        /// <summary>
        /// Generate event change for OutlineThicknessProperty and redraw RatingControl
        /// </summary>
        /// <param name="d">define rate element</param>
        /// <param name="e">value property event </param>
        private static void OutlineThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (e.NewValue is Binding && ParentRatingControl != null)
                BindingOperations.SetBinding(d, OutlineThicknessProperty, (Binding)e.NewValue);
            if (e.OldValue != e.NewValue && ParentRatingControl != null)
                ParentRatingControl.GenerateItems();
        }

        /// <summary>
        /// Generate event change for PathDataProperty and redraw RatingControl
        /// </summary>
        /// <param name="d">define rate element</param>
        /// <param name="e">value property event </param>
        private static void PathDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (e.NewValue is Binding)
                BindingOperations.SetBinding(d, PathDataProperty, (Binding)e.NewValue);
            if (e.OldValue != e.NewValue && ParentRatingControl != null)
                ParentRatingControl.GenerateItems();
        }

        public static readonly DependencyProperty BackgroundColorProperty =
        DependencyProperty.RegisterAttached("BackgroundColor", typeof(SolidColorBrush), typeof(RateItemDefinition),
        new PropertyMetadata(null, AspectChanged));

        public static readonly DependencyProperty OutlineColorProperty =
        DependencyProperty.RegisterAttached("OutlineColor", typeof(SolidColorBrush), typeof(RateItemDefinition),
        new PropertyMetadata(null, AspectChanged2));

        public static readonly DependencyProperty OutlineThicknessProperty =
         DependencyProperty.Register("OutlineThickness", typeof(object), typeof(RateItemDefinition),
         new PropertyMetadata(null, OutlineThicknessChanged));

        public static readonly DependencyProperty PathDataProperty =
         DependencyProperty.RegisterAttached("PathData", typeof(object), typeof(RateItemDefinition),
          new PropertyMetadata(null, PathDataChanged));

    }
}
