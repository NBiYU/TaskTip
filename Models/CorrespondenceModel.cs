using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTip.Models
{
    public class CorrespondenceModel
    {
        public string GUID { get; set; }
        public string Operation { get; set; }
        public object Message { get; set; }
    }
}
