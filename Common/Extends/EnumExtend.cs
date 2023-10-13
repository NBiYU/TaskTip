using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TaskTip.Common.Extends
{
    static class EnumExtend
    {
        public static string GetDesc<TEnum>(this TEnum value) where TEnum : Enum
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

            if (fieldInfo != null)
            {
                DescriptionAttribute[] attributes =
                    (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0)
                {
                    return attributes[0].Description;
                }
            }

            return value.ToString();
        }
    }
}
