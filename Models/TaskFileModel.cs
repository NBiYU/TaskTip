using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTip.Models
{
    public class TaskFileModel
    {
        public bool IsCompleted { get; set; }
        public DateTime TaskTimePlan { get; set; }
        public DateTime CompletedDateTime { get; set; }
        public string EditTextTitle { get; set; }
        public string EditTextText { get; set; }
    }
}
