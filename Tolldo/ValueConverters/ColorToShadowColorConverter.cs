using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Tolldo.ValueConverters
{
    /// <summary>
    /// A value converter that returns a color from <see cref="SolidColorBrush"/>.
    /// </summary>
    public class ColorToShadowColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush)
            {
                Color color = ((SolidColorBrush)value).Color;
                var r = Transform(color.R);
                var g = Transform(color.G);
                var b = Transform(color.B);

                return Color.FromArgb(color.A, r, g, b);
            }

            return value;
        }

        /// <summary>
        /// Transform SolidColorBrush color to <see cref="Color"/> value.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private byte Transform(byte source)
        {
            return (byte)(Math.Pow(source / 255d, 1 / 2.2d) * 255);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
