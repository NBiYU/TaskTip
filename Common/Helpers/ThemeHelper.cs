using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;


namespace TaskTip.Common.Helpers
{
    public static class ThemeHelper
    {

        public static Color GetSystemThemeColor()
        {
            var softwareKeyPath = @"SOFTWARE\Microsoft\Windows\DWM";
            int count = 0;
            using (var reg = Registry.CurrentUser.OpenSubKey(softwareKeyPath))
            {
                var newSubKeyNames = reg.GetValueNames();
                if (newSubKeyNames.Contains("ColorizationColor"))
                {
                    var rgb = reg.GetValue("ColorizationColor");
                    if (int.TryParse(rgb.ToString(), out var result))
                    {
                        return ConvertToColor(result);
                    }
                }
            }
            return Colors.AliceBlue;
        }
        private static Color ConvertToColor(int value)
        {
            return Color.FromArgb(
                (byte)(value >> 24),
                (byte)(value >> 16),
                (byte)(value >> 8),
                (byte)value
            );
        }
    }
}
