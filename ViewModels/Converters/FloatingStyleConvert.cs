using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TaskTip.Enums;
using TaskTip.Models.CommonModel;

namespace TaskTip.ViewModels.Converters
{
    public class FloatingStyleConvert : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (OptionModel<FloatingStyleEnum>)values[1];
            if(targetType.Name == "Visibility")
            {
                if ((string)values[0] == $"{val.Value}")
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
