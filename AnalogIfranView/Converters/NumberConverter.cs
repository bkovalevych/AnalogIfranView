using System;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Data;

namespace AnalogIfranView.Converters
{
    public class NumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var input = value.ToString();
            return Regex.Replace(input, @"\D", "");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var input = value as string;
            var cleaned = Regex.Replace(input, @"\D", "");
            _ = int.TryParse(cleaned, out int output);
            return output;
        }
    }
}
