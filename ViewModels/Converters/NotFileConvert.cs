using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace TaskTip.ViewModels.Converters
{
    class NotFileConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable enumerable && enumerable.Cast<object>().Any())
            {
                return ((BitmapImage)Application.Current.Resources["Empty"]);
            }
            return ((BitmapImage)Application.Current.Resources["NotFile"]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
