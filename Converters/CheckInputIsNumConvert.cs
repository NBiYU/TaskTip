using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TaskTip.Converters
{
    class CheckInputIsNumConvert:IValueConverter
    {
        Regex regex = new Regex(@"^[0-9]+$");
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush brush = new SolidColorBrush(Colors.Transparent);
            if (regex.IsMatch(value.ToString()!))
            {
                brush = new SolidColorBrush(Colors.Gray);
            }else
            {
                brush = new SolidColorBrush(Colors.Red);
            }

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
