using System;
using System.Globalization;
using System.Windows.Data;

namespace Tolldo.ValueConverters
{
    /// <summary>
    /// A value converter that returns an acronym from a string. Based on the first two letters of a single word or the first two letters of multiple words.
    /// </summary>
    public class AcronymConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] words = value.ToString().Split();

            string acronym = "";

            try
            {
                foreach (var word in words)
                {
                    acronym += word.ToUpper()[0];
                    if (acronym.Length == 2)
                        break;
                }

                if (acronym.Length < 2 & value.ToString().Length > 1)
                    acronym += value.ToString().ToUpper()[1];
            }
            catch (Exception)
            {
                return "";
            }

            return acronym;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
