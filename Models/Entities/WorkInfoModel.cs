using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TaskTip.Models.Entities
{
    public partial class WorkInfoModel:ObservableObject
    {
        [ObservableProperty]
        private string _recordDate;
        [ObservableProperty]
        private string _workTime;
        [ObservableProperty]
        private string _startTime;
        [ObservableProperty]
        private string _endTime;
        [ObservableProperty]
        private Visibility _showVisibility;

        public override string ToString()
        {
            return RecordDate;
        }
    }
}
