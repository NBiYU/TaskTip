using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace TaskTip.Models.CommonModel
{
    public class TaskTimeModel
    {
        public IJobDetail Job { get; set; }
        public ITrigger Trigger { get; set; }
    }
}
