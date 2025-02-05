﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaskTip.Models.Enums;
using TaskTip.ViewModels;

namespace TaskTip.Models.DataModel
{
    public class CorrespondenceModel
    {
        public string GUID { get; set; }
        public OperationRequestType Operation { get; set; }
        public object Message { get; set; }
    }
}
