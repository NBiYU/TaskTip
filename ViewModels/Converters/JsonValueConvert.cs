using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TaskTip.Models;

namespace TaskTip.ViewModels.Converters
{
    public class JsonValueConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is JsonEntityModel model)
            {
                if (model.IsArrayType)
                {
                    return $"Array[{model.SubsetList.Count}]";
                }
                else
                {
                    return model.VariableValue;
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
