using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace VectorMaker { 
    /// <summary>
    /// This class holds references to color resources for whole app.
    /// </summary>
    public static class ColorsReference
    {
        public static SolidColorBrush selectedTabItemBackground = (SolidColorBrush)Application.Current.FindResource("TabItemBackgroundSelectedColor");
        public static SolidColorBrush notSelectedTabItemBackground = (SolidColorBrush)Application.Current.FindResource("TabItemBackgroundNotSelectedColor");
        public static System.Windows.Media.Color magentaBaseColor = (System.Windows.Media.Color)Application.Current.FindResource("MagentaBaseColor");
        public static SolidColorBrush magentaBaseBrush = (SolidColorBrush)Application.Current.FindResource("MagentaBaseBrush");
        public static GradientBrush valueGradientBrush = (GradientBrush)Application.Current.FindResource("ValueGradient");
        public static GradientStopCollection valueGradientStopCollection = valueGradientBrush.GradientStops;
        public static List<GradientStop> valueGradientStopListSegregated = valueGradientBrush.GradientStops
            .OrderBy(x=>x.Offset).ToList();
    }
}
