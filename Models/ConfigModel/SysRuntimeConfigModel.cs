using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTip.Models.SettingModel;

namespace TaskTip.Models.ConfigModel
{
    public class SysRuntimeConfigModel
    {
        public string NetworkCardName { get; set; }
        public ThemeModel FontTheme { get; set; }
    }
}
