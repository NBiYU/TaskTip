using CommunityToolkit.Mvvm.ComponentModel;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaskTip.Models.Enums;

namespace TaskTip.Models.Entities
{
    public partial class TaskFileModel:ObservableObject
    {
        [ObservableProperty]
        private string _gUID;
        [ObservableProperty]
        private bool _isCompleted;
        [ObservableProperty]
        private DateTime _taskTimePlan;
        [ObservableProperty]
        private DateTime _completedDateTime;
        [ObservableProperty]
        private string _editTextTitle;
        [ObservableProperty]
        private string _editTextText;
        [ObservableProperty]
        private bool _isFocus= false;
        [ObservableProperty]
        private TaskStatusType _currentTaskStatus = TaskStatusType.Undefined;

        public override string ToString()
        {
            return EditTextTitle;
        }
    }
}
