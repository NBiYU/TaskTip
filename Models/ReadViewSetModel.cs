using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTip.Models
{
    public class ReadViewSetModel
    {
        public double WindowHeight { get; set; }
        public double WindowWidth { get; set; }
        public int ContentGap { get; set; }
        public int ReadSpeed { get; set; }
        public int FictionFontSize { get; set; }
        public bool IsTopmost { get; set; }
        public string BackgroundColorHex { get; set; }
        public string FontColorHex { get; set; }
    }
}
