using System;
using System.Globalization;
using System.Windows.Data;

namespace VectorMaker.Converters
{
    public class TranslationConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return new Tuple<double,double> (
                (double)(values[0]??0d), 
                (double)(values[1]??0d));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
