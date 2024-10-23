using TaskTip.Enums;

namespace TaskTip.Models.DataModel
{
    internal class TaskStatusModel
    {
        public string GUID { get; set; }
        public TaskStatusType Status { get; set; }
    }
}
