using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Tolldo.ValueConverters
{
    /// <summary>
    /// A value converter that returns <see cref="Visibility.Visible"/> if value is null.
    /// </summary>
    public class ReversedNullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
