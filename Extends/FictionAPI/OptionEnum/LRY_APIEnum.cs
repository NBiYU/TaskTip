using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TaskTip.Extends.FictionAPI.OptionEnum
{
    public static class EnumExtend
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
        public static string[] GetEnumAllDesc<TEnum>()
        {
            var fieldInfos = typeof(TEnum).GetFields();
            var desc = new string[fieldInfos.Length - 1];
            for (int i = 1; i < fieldInfos.Length; i++)
            {
                DescriptionAttribute[] attributes =
                    (DescriptionAttribute[])fieldInfos[i].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes != null)
                {
                    desc[i - 1] = attributes[0].Description;
                }
            }
            return desc;
        }

        public static T TryGetEnum<T>(string description)
        {
            Type type = typeof(T);

            if (!type.IsEnum)
            {
                throw new ArgumentException($"Type '{type}' is not an enum.");
            }

            foreach (FieldInfo fieldInfo in type.GetFields())
            {
                if (fieldInfo.Name == description)
                {
                    return (T)fieldInfo.GetValue(null);
                }
                else if (Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                    {
                        return (T)fieldInfo.GetValue(null);
                    }
                }
            }

            throw new ArgumentException($"No matching enum value found for description '{description}'.");

        }

    }


    public enum LRY_APIOptionEnum
    {
        [Description("标题查询")]
        SearchTitle,
        [Description("作者查询")]
        SearchAuthor,
        [Description("分类查询")]
        SearchCategory
    }

    public enum LRY_APIFictionCagetory
    {
        [Description("网游小说")]
        NetworkGameFiction,
        [Description("武侠修真")]
        SwordsmenFiction,
        [Description("玄幻小说")]
        FantasyFiction,
        [Description("都市小说")]
        MetropolitanFiction,
        [Description("女生频道")]
        GirlsChannel,
        [Description("其他小说")]
        OtherFiction,
    }
}
