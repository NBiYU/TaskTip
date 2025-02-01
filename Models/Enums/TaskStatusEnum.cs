using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaskTip.Models.Entities;
using TaskTip.Models.ViewDataModels;

namespace TaskTip.Models.Enums
{
    public enum TaskStatusType
    {
        [Description("默认")]
        Default,
        [Description("完成")]
        Complete,
        [Description("超时")]
        Timeout,
        [Description("执行中")]
        Runtime,
        [Description("未定义")]
        Undefined
    }
    public static class TaskStatusEnumExtend
    {
        public static TaskStatusType BizTask2TaskStatusType(this BizTask bizTask)
        {
            return bizTask.TaskTimePlan == DateTime.MinValue ? TaskStatusType.Undefined :
                    bizTask.IsCompleted == 1 ? TaskStatusType.Complete :
                    bizTask.TaskTimePlan > DateTime.Now ? TaskStatusType.Runtime : TaskStatusType.Timeout;
        }
        public static TaskStatusType TaskFileModel2TaskStatusType(this TaskFileModel model)
        {
            return model.TaskTimePlan == DateTime.MinValue ? TaskStatusType.Undefined :
                model.IsCompleted ? TaskStatusType.Complete :
                model.TaskTimePlan > DateTime.Now ? TaskStatusType.Runtime : TaskStatusType.Timeout;
        }
    }
}
