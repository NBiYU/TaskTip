using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace TaskTip.ViewModels.Converters
{
    internal class ThemeColorConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var index = int.Parse(parameter.ToString());
                if (value is string[] array && index >= 0 && index < array.Length)
                {
                    return (LinearGradientBrush)Application.Current.Resources[array[index]];
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
