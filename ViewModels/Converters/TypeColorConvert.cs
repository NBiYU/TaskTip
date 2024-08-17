using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TaskTip.ViewModels.Converters
{
    public class TypeColorConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (JTokenType)value;
            var color = type switch { 
                JTokenType.None => Colors.Black,
                JTokenType.Object => Colors.Black,
                JTokenType.String => Colors.Orange,
                JTokenType.Integer => Colors.DeepSkyBlue,
                JTokenType.Boolean => Colors.Blue,
                JTokenType.Float => Colors.DarkCyan,
                JTokenType.Date => Colors.Green,
                JTokenType.Null => Colors.Red,
                _ => Colors.Black
            };

            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
