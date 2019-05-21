using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Tolldo.ValueConverters
{
    /// <summary>
    /// A value converter that returns <see cref="Visibility.Collapsed"/> if value is null.
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
