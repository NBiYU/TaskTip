using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaskTip.Models.Entities;
using TaskTip.Models.Enums;

namespace TaskTip.Common.Converter.Map
{
    public static class MapConverter
    {
        public static TaskFileModel Entity2TaskModel(this BizTask bizTask)
        {
            return new TaskFileModel
            {
                GUID = bizTask.Guid,
                TaskTimePlan = bizTask.TaskTimePlan ?? DateTime.MinValue,
                CompletedDateTime = bizTask.CompletedDateTime ?? DateTime.MinValue,
                IsCompleted = bizTask.IsCompleted == 1,
                EditTextTitle = bizTask.EditTextTitle ?? string.Empty,
                EditTextText = bizTask.EditTextText ?? string.Empty,
                CurrentTaskStatus = bizTask.BizTask2TaskStatusType()

            };
        } 
        public static WorkInfoModel Entity2WorkModel(this BizRecordWork bizRecordWork)
        {
            return new WorkInfoModel
            {
                RecordDate = bizRecordWork.RecordDate ?? string.Empty,
                WorkTime = bizRecordWork.WorkTime ?? string.Empty,
                StartTime = bizRecordWork.StartTime ?? string.Empty,
                EndTime = bizRecordWork.EndTime ?? string.Empty
            };
        }
    }
}
