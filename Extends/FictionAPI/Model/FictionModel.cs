using System;
using System.Collections.Generic;
using System.Text;

namespace TaskTip.Extends.FictionAPI.Model
{
    public class FictionResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Error { get; set; }
        public object Content { get; set; }
    }
}
