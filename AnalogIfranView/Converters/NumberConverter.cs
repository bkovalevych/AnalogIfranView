using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace AnalogIfranView.Converters
{
    public class NumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            var input = value as string;
            var cleaned = Regex.Replace(input, @"\D", "");
            int output;
            if (int.TryParse(cleaned, out output))
                return output;
            else
                return 0;
        }
    }
}
