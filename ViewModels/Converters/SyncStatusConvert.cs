using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using TaskTip.ViewModels.WindowModel;

namespace TaskTip.ViewModels.Converters
{
    class SyncStatusConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CompareStatus status)
            {
                return status switch {

                    CompareStatus.Default => ((BitmapImage)Application.Current.Resources["Minimize"]).UriSource,
                    CompareStatus.Conflict => ((BitmapImage)Application.Current.Resources["Conflict"]).UriSource,
                    CompareStatus.Merger => ((BitmapImage)Application.Current.Resources["Merger"]).UriSource,
                    CompareStatus.Finish => ((BitmapImage)Application.Current.Resources["Success"]).UriSource,
                    _ => ((BitmapImage)Application.Current.Resources["Error"]).UriSource,
                };
            }
            return ((BitmapImage)Application.Current.Resources["Error"]).UriSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
