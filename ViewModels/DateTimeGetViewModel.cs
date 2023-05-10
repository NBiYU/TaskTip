using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace TaskTip.ViewModels
{
    internal class DateTimeGetViewModel : ObservableObject
    {
        /// <summary>
        /// 通知外部时间选择结果
        /// </summary>
        public static event EventHandler DateTimeCalendarEvent;


        #region 属性

        private DateTime selectTaskPlanTime;
        public DateTime SelectTaskPlanTime
        {
            get => selectTaskPlanTime;
            set => SetProperty(ref selectTaskPlanTime, value);
        }

        private string sourceGuid;
        public string SourceGuid
        {
            get => sourceGuid;
            set => SetProperty(ref sourceGuid, value);
        }

        #endregion

        #region 控件指令

        /// <summary>
        /// 确认指令
        /// </summary>
        public RelayCommand ConfirmCommand { get; set; }

        /// <summary>
        /// 取消指令
        /// </summary>
        public RelayCommand CancelCommand { get; set; }

        #endregion

        #region 控件指令处理函数

        /// <summary>
        /// 确认指令处理函数
        /// </summary>
        private void Confirm()
        {
            DateTimeCalendarEvent?.Invoke(SourceGuid + ";" + SelectTaskPlanTime, null);
        }

        /// <summary>
        /// 取消指令处理函数
        /// </summary>
        private void Cancel()
        {
            DateTimeCalendarEvent?.Invoke("" + ";" + SelectTaskPlanTime, null);
        }

        #endregion

        public DateTimeGetViewModel()
        {
            ConfirmCommand = new RelayCommand(Confirm);
            CancelCommand = new RelayCommand(Cancel);
        }

    }

}