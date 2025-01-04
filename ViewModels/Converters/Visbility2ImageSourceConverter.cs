using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace TaskTip.ViewModels.Converters
{
    public class Visbility2ImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (Visibility)value;
            var imageKey = val == Visibility.Visible ? "ChevronDown" : "ChevronLeft";
            return ((BitmapImage)Application.Current.Resources[imageKey]).UriSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
