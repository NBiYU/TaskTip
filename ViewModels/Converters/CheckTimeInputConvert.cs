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
    class CheckTimeInputConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Regex isTimeFormat = new Regex(@"^\d{1,2}:\d{1,2}?$");
            Brush checkBrush = new SolidColorBrush(Colors.Transparent);
            if (isTimeFormat.IsMatch(value.ToString()!))
            {
                checkBrush = new SolidColorBrush(Colors.Gray);
                //DailyTaskEndTimeChanged?.Invoke(null,null);
            }
            else
            {
                checkBrush = new SolidColorBrush(Colors.Red);
            }
            return checkBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
