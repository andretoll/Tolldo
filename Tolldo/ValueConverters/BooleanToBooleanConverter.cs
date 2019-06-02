using System;
using System.Globalization;
using System.Windows.Data;

namespace Tolldo.ValueConverters
{
    /// <summary>
    /// A value converter that returns false if value true.
    /// </summary>
    public class BooleanToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
