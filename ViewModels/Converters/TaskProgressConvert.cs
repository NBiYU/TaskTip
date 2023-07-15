using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TaskTip.ViewModels.Converters
{
    public class TaskProgressConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not TaskStatus val) return null;

            return value switch
            {
                TaskStatus.Runtime => Colors.DodgerBlue,
                TaskStatus.Timeout => Colors.Red,
                TaskStatus.Undefined => Colors.Azure,
                TaskStatus.Complete => "#FF7EC7",
                _ => Colors.Blue
            };
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
