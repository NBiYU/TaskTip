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
        #region ��������

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
        /// ��С��ͼƬ·��
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
        /// ��С��ͼƬ
        /// </summary>
        public ImageSource MinImageSource
        {
            get => minImageSource;
            set => SetProperty(ref minImageSource, value);
        }

        private string settingImagePath;
        /// <summary>
        /// SettingͼƬ·��
        /// </summary>
        public string SettingImagePath
        {
            get => settingImagePath;
            set => SetProperty(ref settingImagePath, value);
        }

        private ImageSource settImageSource;
        /// <summary>
        /// SettingͼƬ
        /// </summary>
        public ImageSource SetImageSource
        {
            get => settImageSource;
            set => SetProperty(ref settImageSource, value);
        }


        private string _title;
        /// <summary>
        /// ����
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }



        #endregion

        #region ָ�����

        /// <summary>
        /// ��С��ָ�����
        /// </summary>
        [RelayCommand]
        public void Mini()
        {
            WindowResource.TaskMenoViewHide();
            WindowResource.SwitchFloatingShow(GlobalVariable.FloatingStyle);

        }

        /// <summary>
        /// ���ý�����ʾ������
        /// </summary>
        [RelayCommand]
        public void Set()
        {
            WindowResource.CustomSetViewShow();
            WindowResource.TaskMenoViewHide();
        }

        /// <summary>
        /// ѡ��л��¼�
        /// </summary>
        /// <param name="sender"></param>
        [RelayCommand]
        public void TabControlSelect(object sender)
        {
            if (sender is not TabItem item) return;
            if (item.Header.ToString() != "��¼") return;

            WindowResource.EditFullScreenViewClose();
        }

        #endregion

        #region ���ܺ���

        private void TaskListPageModelOnTaskListChanged(CorrespondenceModel corr)
        {
            if (corr.Message is ObservableCollection<TaskFileModel> taskList)
            {
                Title = $"���� {taskList.Count(x => x.IsCompleted == false)} ������δ���";
                var timeoutVms = taskList.Where(x => x.TaskTimePlan != DateTime.MinValue && x.TaskTimePlan < DateTime.Now && x.IsCompleted == false).ToList();
                TimeoutTitle = $"���� {timeoutVms.Count} ����ʱ����";
                TimeoutListString = timeoutVms.Count != 0 ? string.Join("\n", timeoutVms.Select(x => x.EditTextTitle)) : "��ǰ�޳�ʱ����";
            }

        }

        #endregion

        #region ��ʼ��
        private void InitRegister()
        {
            WeakReferenceMessenger.Default.Register<CorrespondenceModel, string>(this, Const.CONST_TASK_LIST_CHANGED,
                (obj, msg) =>
                {
                    TaskListPageModelOnTaskListChanged(msg);
                });
        }

        /// <summary>
        /// �����¼���ʼ��
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
