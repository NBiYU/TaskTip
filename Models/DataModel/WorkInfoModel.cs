using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTip.Models.DataModel
{
    public class WorkInfoModel
    {
        private string GUID => RecordDate;
        public string RecordDate { get; set; }
        public string WorkTime { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }


        public override string ToString()
        {
            return RecordDate;
        }
    }
}
