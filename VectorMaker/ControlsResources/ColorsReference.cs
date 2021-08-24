using System.Windows;
using System.Windows.Media;

namespace VectorMaker { 
    public static class ColorsReference
    {
        public static SolidColorBrush selectedTabItemBackground = (SolidColorBrush)Application.Current.FindResource("TabItemBackgroundSelectedColor");
        public static SolidColorBrush notSelectedTabItemBackground = (SolidColorBrush)Application.Current.FindResource("TabItemBackgroundNotSelectedColor");
    }
}
