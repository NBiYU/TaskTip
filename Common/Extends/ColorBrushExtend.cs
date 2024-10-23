using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using TaskTip.Models.SettingModel;

namespace TaskTip.Common.Extends
{
    public static class ColorBrushExtend
    {
        public static GradientStopCollection GetGradientColors(this List<ColorModel> model)
        {
            var colors = new GradientStopCollection();
            foreach (var colorItem in model)
            {
                if (ColorConverter.ConvertFromString(colorItem.ColorHex) is Color color)
                {
                    colors.Add(new GradientStop(color, colorItem.Offset));
                }
            }
            return colors;
        }

        public static GradientBrush GetGradientBrush(this GradientColorModel model)
        {
            var colors = model.ColorModels.GetGradientColors();
            return model.IsLinear
            ? new LinearGradientBrush(colors, new Point(model.StartX, model.StartY), new Point(model.EndX, model.EndY))
                : new RadialGradientBrush(colors);
        }
    }
}
