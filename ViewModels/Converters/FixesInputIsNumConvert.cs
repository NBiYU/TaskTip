using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TaskTip.ViewModels.Converters
{
    class FixesInputIsNumConvert : IValueConverter
    {
        Regex regex = new Regex(@"^^[0-9]+(\.[0-9]+){0,1}$");
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Regex.Replace(value.ToString()!, regex.ToString(), "");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Regex.Replace(value.ToString()!, regex.ToString(), "");
        }
    }
}
