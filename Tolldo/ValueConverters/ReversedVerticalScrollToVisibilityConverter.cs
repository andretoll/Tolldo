using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Tolldo.ValueConverters
{
    /// <summary>
    /// A value converter that returns <see cref="Visibility.Collapsed"/> if scrollbar is at the bottom.
    /// </summary>
    public class ReversedVerticalScrollToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Visibility)values[2] == Visibility.Visible)
            {
                return Visibility.Collapsed;
            }

            if ((double)values[0] != (double)values[1])
            {
                return Visibility.Collapsed;
            }
            else return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
