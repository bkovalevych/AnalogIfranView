using System;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Data;

namespace AnalogIfranView.Converters
{
    public class NumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var input = value as string;
            var cleaned = Regex.Replace(input, @"\D", "");
            if(int.TryParse(cleaned, out int output))
                return output;
            else
                return 0;
        }
    }
}
