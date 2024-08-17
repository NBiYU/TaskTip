using System;
using System.Collections.Generic;
using System.Text;

namespace TaskTip.Extends.ConnectClient.Model
{
    public class ResponseModel
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Content { get; set; }
    }
}
