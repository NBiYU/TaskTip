using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using CommunityToolkit.Mvvm.Messaging;
using TaskTip.Services;

namespace TaskTip.ViewModels.WindowModel
{
    internal class DateTimeGetViewModel : ObservableObject
    {
        
        #region 属性

        private DateTime selectTaskPlanTime;
        public DateTime SelectTaskPlanTime
        {
            get => selectTaskPlanTime;
            set => SetProperty(ref selectTaskPlanTime, value);
        }

        private DateTime _displayTaskDateTime;

        public DateTime DisplayTaskDateTime
        {
            get => _displayTaskDateTime;
            set => SetProperty(ref _displayTaskDateTime, value);
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
            //WeakReferenceMessenger.Default.Send(SourceGuid + ";" + SelectTaskPlanTime, Const.CONST_DATETIME_RETURN);
        }

        /// <summary>
        /// 取消指令处理函数
        /// </summary>
        private void Cancel()
        {
            //WeakReferenceMessenger.Default.Send("" + ";" + SelectTaskPlanTime, Const.CONST_DATETIME_RETURN);
        }

        #endregion

        public DateTimeGetViewModel()
        {
            ConfirmCommand = new RelayCommand(Confirm);
            CancelCommand = new RelayCommand(Cancel);
        }

    }

}