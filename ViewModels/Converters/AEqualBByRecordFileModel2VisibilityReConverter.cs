using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using TaskTip.Models.DataModel;

namespace TaskTip.ViewModels.Converters
{
    public class AEqualBByRecordFileModel2VisibilityReConverter: IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length >= 2 && values[1] is TreeInfo info)
            {
                return values[0].Equals(info.GUID) ? Visibility.Collapsed : Visibility.Visible;
            }
            return false ? Visibility.Collapsed : Visibility.Visible;

        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
