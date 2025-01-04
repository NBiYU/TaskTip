using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Messaging;
using TaskTip.Services;
using TaskTip.Views;
using TaskTip.Pages;
using TaskTip.Common;
using TaskTip.Models.Enums;
using TaskTip.Models.DataModel;
using TaskTip.Models.Entities;
using System.Collections.Generic;

namespace TaskTip.ViewModels.WindowModel
{
    public partial class FloatingTitleViewModel : ObservableRecipient
    {
        #region ����

        public Page FrameRecordPage
        {
            get => WindowResource.RecordPage;
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private Visibility _buttonVisibility;
        public Visibility ButtonVisibility
        {
            get => _buttonVisibility;
            set => SetProperty(ref _buttonVisibility, value);
        }

        private Brush titleBorderBrush;
        public Brush TitleBorderBrush { get => titleBorderBrush; set => SetProperty(ref titleBorderBrush, value); }

        #endregion

        #region ָ�����
        [RelayCommand]
        public void ButtonVisibilityChangedHandle()
        {
            ButtonVisibility = ButtonVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            
            TitleBorderBrush = new SolidColorBrush(ButtonVisibility == Visibility.Visible ? Colors.White : Colors.Gray);
        }
        [RelayCommand]
        public void ShowSetHandle()
        {
            WindowResource.FlotingHidden(GlobalVariable.FloatingStyle);
            WindowResource.CustomSetViewShow();
        }

        #endregion

        #region  �ⲿ�����¼���������

        private void TaskListPageModelOnTaskListChanged(CorrespondenceModel corr)
        {
            if (corr.Message is ObservableCollection<TaskFileModel> taskList)
                Title = $"���� {taskList.Count(x => x.IsCompleted == false)} ������δ���";
        }



        #endregion

        private void InitRegister()
        {
            WeakReferenceMessenger.Default.Register<CorrespondenceModel, string>(this, Const.CONST_TASK_LIST_CHANGED,
                (obj, msg) => { TaskListPageModelOnTaskListChanged(msg); });
        }
        private void UnRegister()
        {
            WeakReferenceMessenger.Default.Unregister<CorrespondenceModel, string>(this, Const.CONST_TASK_LIST_CHANGED);
        }

        public FloatingTitleViewModel()
        {
            ButtonVisibility = Visibility.Collapsed;
            //ShowButtonGeometry = GlobalVariable.CollapsedGeometry;
            TitleBorderBrush = new SolidColorBrush(Colors.Gray);
            InitRegister();
        }

        ~FloatingTitleViewModel()
        {
            UnRegister();
        }
    }
}
