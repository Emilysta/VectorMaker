using System;
using System.Globalization;
using System.Windows.Data;
using VectorMaker.ViewModel;

namespace VectorMaker.Utility
{
    public class DockingDocumentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DrawingCanvasViewModel)
                return value;

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DrawingCanvasViewModel)
                return value;

            return Binding.DoNothing;
        }
    }
}
