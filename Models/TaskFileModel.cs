using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTip.Models
{
    public class TaskFileModel
    {
        public string GUID;
        public bool IsCompleted;
        public DateTime TaskTimePlan;
        public DateTime CompletedDateTime;
        public string EditTextTitle;
        public string EditTextText;

        public override string ToString()
        {
            return EditTextTitle;
        }
    }
}
