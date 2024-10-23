using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTip.Common.Extends
{
    public static class StringExtend
    {
        public static string[]? ChunkString(this string? str,int size)
        {
            var strs = new List<string>();
            if (str != null)
            {
                while (str.Length > size)
                {
                    strs.Add(str.Substring(0, size));
                    str = str.Substring(size);
                }
                strs.Add(str);
                return strs.ToArray();
            }
            return default;
            
        }
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
    }
}
