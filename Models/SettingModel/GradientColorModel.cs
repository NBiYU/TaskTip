using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTip.Models.SettingModel
{
    public class ThemeModel
    {
        public string ThemeName { get; set; }
        public bool IsCustom { get; set; }
        public List<GradientColorModel> CategoryThemes { get; set; }
    }

    public class GradientColorModel
    {
        public int ID { get; set; }
        public string CategoryName { get; set; }
        public bool IsLinear { get; set; }
        public double StartX { get; set; }
        public double StartY { get; set; }
        public double EndX { get; set; }
        public double EndY { get; set; }
        public List<ColorModel> ColorModels { get; set; }

    }

    public class ColorModel
    {
        public string ColorHex { get; set; }
        public double Offset { get; set; }
    }

}
