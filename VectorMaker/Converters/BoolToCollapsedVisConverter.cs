using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VectorMaker.Converters
{
    /// <summary>
    /// This class defines converter from bool to Visibility.Collapsed type and vice versa
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToCollapsedVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target must be a boolean");

            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a Visibility");

            return (Visibility)value == Visibility.Visible;
        }

    }
}
