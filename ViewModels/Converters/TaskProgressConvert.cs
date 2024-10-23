using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TaskTip.Enums;

namespace TaskTip.ViewModels.Converters
{
    public class TaskProgressConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not TaskStatusType val) return null;

            var obj = Application.Current.Resources["ThemeColorArray"];
            if (obj is string[] colorKeys)
            {
                return value switch
                {
                    TaskStatusType.Runtime => (LinearGradientBrush)Application.Current.Resources[colorKeys[1]],
                    TaskStatusType.Timeout => (LinearGradientBrush)Application.Current.Resources[colorKeys[2]],
                    TaskStatusType.Undefined => (LinearGradientBrush)Application.Current.Resources[colorKeys[3]],
                    TaskStatusType.Complete => (LinearGradientBrush)Application.Current.Resources[colorKeys[4]],
                    _ => Colors.Black,
                };
            }

            return Colors.Black;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
