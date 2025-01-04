using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.Messaging;
using TaskTip.Pages;
using TaskTip.Services;
using TaskTip.Views;
using TaskTip.ViewModels.UserViewModel;
using TaskTip.Common;
using TaskTip.Models.Enums;
using TaskTip.Models.DataModel;
using TaskTip.Models.Entities;
using System.Collections.Generic;

namespace TaskTip.ViewModels.WindowModel
{
    internal partial class TaskMenoViewModel : ObservableRecipient
    {
        #region 窗体属性

        private string _timeoutTitle;
        public string TimeoutTitle
        {
            get => _timeoutTitle;
            set => SetProperty(ref _timeoutTitle, value);
        }

        private string _timeoutListString;
        public string TimeoutListString
        {
            get => _timeoutListString;
            set => SetProperty(ref _timeoutListString, value);
        }


        public Page FrameRecordPage
        {
            get => WindowResource.RecordPage;
        }

        private object taskMenoWidth;
        public object TaskMenoWidth
        {
            get => taskMenoWidth;
            set => SetProperty(ref taskMenoWidth, value);
        }

        private object taskMenoHeight;
        public object TaskMenoHeight
        {
            get => taskMenoHeight;
            set => SetProperty(ref taskMenoHeight, value);
        }

        private string minimizeImagePath;
        /// <summary>
        /// 最小化图片路径
        /// </summary>
        public string MinimizeImagePath
        {
            get => minimizeImagePath;
            set
            {
                SetProperty(ref minimizeImagePath, value);
                MinImageSource = new BitmapImage(new Uri(minimizeImagePath));
            }
        }

        private ImageSource minImageSource;
        /// <summary>
        /// 最小化图片
        /// </summary>
        public ImageSource MinImageSource
        {
            get => minImageSource;
            set => SetProperty(ref minImageSource, value);
        }

        private string settingImagePath;
        /// <summary>
        /// Setting图片路径
        /// </summary>
        public string SettingImagePath
        {
            get => settingImagePath;
            set => SetProperty(ref settingImagePath, value);
        }

        private ImageSource settImageSource;
        /// <summary>
        /// Setting图片
        /// </summary>
        public ImageSource SetImageSource
        {
            get => settImageSource;
            set => SetProperty(ref settImageSource, value);
        }


        private string _title;
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }



        #endregion

        #region 指令处理函数

        /// <summary>
        /// 最小化指令处理函数
        /// </summary>
        [RelayCommand]
        public void Mini()
        {
            WindowResource.TaskMenoViewHide();
            WindowResource.SwitchFloatingShow(GlobalVariable.FloatingStyle);

        }

        /// <summary>
        /// 设置界面显示处理函数
        /// </summary>
        [RelayCommand]
        public void Set()
        {
            WindowResource.CustomSetViewShow();
            WindowResource.TaskMenoViewHide();
        }

        /// <summary>
        /// 选项卡切换事件
        /// </summary>
        /// <param name="sender"></param>
        [RelayCommand]
        public void TabControlSelect(object sender)
        {
            if (sender is not TabItem item) return;
            if (item.Header.ToString() != "记录") return;

            WindowResource.EditFullScreenViewClose();
        }

        #endregion

        #region 功能函数

        private void TaskListPageModelOnTaskListChanged(CorrespondenceModel corr)
        {
            if (corr.Message is ObservableCollection<TaskFileModel> taskList)
            {
                Title = $"还有 {taskList.Count(x => x.IsCompleted == false)} 个任务未完成";
                var timeoutVms = taskList.Where(x => x.TaskTimePlan != DateTime.MinValue && x.TaskTimePlan < DateTime.Now && x.IsCompleted == false).ToList();
                TimeoutTitle = $"存在 {timeoutVms.Count} 个超时任务";
                TimeoutListString = timeoutVms.Count != 0 ? string.Join("\n", timeoutVms.Select(x => x.EditTextTitle)) : "当前无超时任务";
            }

        }

        #endregion

        #region 初始化
        private void InitRegister()
        {
            WeakReferenceMessenger.Default.Register<CorrespondenceModel, string>(this, Const.CONST_TASK_LIST_CHANGED,
                (obj, msg) =>
                {
                    TaskListPageModelOnTaskListChanged(msg);
                });
        }

        /// <summary>
        /// 界面事件初始化
        /// </summary>
        public TaskMenoViewModel()
        {
            taskMenoWidth = SystemParameters.WorkArea.Height / 3;
            taskMenoHeight = SystemParameters.WorkArea.Width / 3;

            InitRegister();

        }

        #endregion

    }

}
