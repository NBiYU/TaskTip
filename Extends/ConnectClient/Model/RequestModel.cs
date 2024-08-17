using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TaskTip.Extends.ConnectClient.Model
{
    public class RequestModel
    {
        public HttpMethod Method { get; set; }
        public string Url { get; set; }
        public string Content { get; set; }

    }
}
