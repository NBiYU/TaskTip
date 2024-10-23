﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTip.Enums
{
    public enum FloatingStyleEnum
    {
        [Description("图片悬浮")]
        Image = 1,
        [Description("标题悬浮")]
        Title,
        [Description("状态悬浮")]
        Status
    }
}
