using System.Windows;
using System.Windows.Media;

namespace VectorMaker { 
    public static class ColorsReference
    {
        public static SolidColorBrush selectedTabItemBackground = (SolidColorBrush)Application.Current.FindResource("TabItemBackgroundSelectedColor");
        public static SolidColorBrush notSelectedTabItemBackground = (SolidColorBrush)Application.Current.FindResource("TabItemBackgroundNotSelectedColor");
        public static System.Windows.Media.Color magentaBaseColor = (System.Windows.Media.Color)Application.Current.FindResource("MagentaBaseColor");
        public static SolidColorBrush magentaBaseBrush = (SolidColorBrush)Application.Current.FindResource("MagentaBaseBrush");
    }
}
