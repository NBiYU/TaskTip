using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTip.Enums;

namespace TaskTip.Models.DataModel
{
    public class TaskFileModel
    {
        public string GUID;
        public bool IsCompleted;
        public DateTime TaskTimePlan;
        public DateTime CompletedDateTime;
        public string EditTextTitle;
        public string EditTextText;

        [JsonIgnore]
        public string GUIDProperty { get => GUID; set => GUID = value; }
        [JsonIgnore]
        public bool IsCompletedProperty { get => IsCompleted; set => IsCompleted = value; }
        [JsonIgnore]
        public DateTime TaskTimePlanProperty { get => TaskTimePlan; set => TaskTimePlan = value; }
        [JsonIgnore]
        public DateTime CompletedDateTimeProperty { get => CompletedDateTime; set => CompletedDateTime = value; }
        [JsonIgnore]
        public string EditTextTitleProperty { get => EditTextTitle; set => EditTextTitle = value; }
        [JsonIgnore]
        public string EditTextTextProperty { get => EditTextText; set => EditTextText = value; }
        public TaskStatusType CurrentTaskStatusProperty => TaskTimePlan == DateTime.MinValue ? TaskStatusType.Undefined :
                    IsCompleted ? TaskStatusType.Complete :
                    TaskTimePlan > DateTime.Now ? TaskStatusType.Runtime : TaskStatusType.Timeout;

        public override string ToString()
        {
            return EditTextTitle;
        }
    }
}
