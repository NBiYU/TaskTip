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

namespace TaskTip.ViewModels
{
    internal class TaskMenoViewModel : ObservableRecipient
    {
        #region 窗体属性


        public RecordPage FrameRecordPage
        {
            get => GlobalVariable.RecordPage;
        }

        private double taskMenoWidth;
        public double TaskMenoWidth
        {
            get => taskMenoWidth;
            set => SetProperty(ref taskMenoWidth, value);
        }

        private double taskMenoHeight;
        public double TaskMenoHeight
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


        /// <summary>
        /// 最小化按钮触发指令
        /// </summary>
        public RelayCommand MiniCommand { get; set; }

        /// <summary>
        /// 设置按钮触发指令
        /// </summary>
        public RelayCommand SetCommand { get; set; }



        /// <summary>
        /// 最小化指令处理函数
        /// </summary>
        private void MiniWinodow()
        {
            GlobalVariable.TaskMenoViewHide();
            if (GlobalVariable.IsFloatingImageStyle)
            {
                GlobalVariable.FloatingViewShow();
            }
            else
            {
                GlobalVariable.FloatingTitleStyleViewShow();
            }

        }

        /// <summary>
        /// 设置界面显示处理函数
        /// </summary>
        private void SetWindowShow()
        {
            GlobalVariable.CustomSetViewShow();
            GlobalVariable.TaskMenoViewHide();
        }

        private void TaskListPageModelOnTaskListChanged(ObservableCollection<TaskListItemUserControl> sender)
        {
            Title =
                $"还有 {sender.Count(x => x.IsCompleted.IsChecked == false)} 个任务未完成";
        }

        private void InitRegister()
        {
            WeakReferenceMessenger.Default.Register<ObservableCollection<TaskListItemUserControl>, string>(this, Const.CONST_TASK_LIST_CHANGED,
                (obj, msg) => { TaskListPageModelOnTaskListChanged(msg); });
        }

        /// <summary>
        /// 界面事件初始化
        /// </summary>
        public TaskMenoViewModel()
        {

            MiniCommand = new RelayCommand(MiniWinodow);
            SetCommand = new RelayCommand(SetWindowShow);

            taskMenoWidth = SystemParameters.WorkArea.Height / 3;
            taskMenoHeight = SystemParameters.WorkArea.Width / 3;

            InitRegister();

        }


    }

}
