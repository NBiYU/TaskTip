using CommunityToolkit.Mvvm.ComponentModel;using CommunityToolkit.Mvvm.Input;using System;using System.Collections.ObjectModel;using System.Configuration;using System.Linq;using System.Windows;using System.Windows.Controls;using System.Windows.Documents;using System.Windows.Media;using CommunityToolkit.Mvvm.Messaging;using TaskTip.Services;using TaskTip.Views;using TaskTip.Pages;using TaskTip.Common;using TaskTip.Enums;using TaskTip.Models.DataModel;

namespace TaskTip.ViewModels.WindowModel{    internal class FloatingTitleViewModel : ObservableObject    {



        #region 属性
        public Page FrameRecordPage        {            get => GlobalVariable.RecordPage;        }        private string _title;        public string Title        {            get => _title;            set => SetProperty(ref _title, value);        }        private Visibility _buttonVisibility;        public Visibility ButtonVisibility        {            get => _buttonVisibility;            set => SetProperty(ref _buttonVisibility, value);        }        private Brush titleBorderBrush;        public Brush TitleBorderBrush { get => titleBorderBrush; set => SetProperty(ref titleBorderBrush, value); }        private Geometry _showButtonGeometry;        public Geometry ShowButtonGeometry        {            get => _showButtonGeometry;            set => SetProperty(ref _showButtonGeometry, value);        }        private Geometry _settingButtonGeometry;        public Geometry SettingButtonGeometry        {            get => _settingButtonGeometry;            set => SetProperty(ref _settingButtonGeometry, value);        }





        #endregion
        #region 控件指令
        public RelayCommand ButtonVisibilityChangedCommand        {            get;            set;        }        public RelayCommand ShowSetCommand { get; set; }





        #endregion

        #region 指令处理函数
        private void ButtonVisibilityChangedHandle()        {            ButtonVisibility = ButtonVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            //ShowButtonGeometry = ButtonVisibility == Visibility.Visible ? GlobalVariable.VisibilityGeometry : GlobalVariable.CollapsedGeometry;
            TitleBorderBrush = new SolidColorBrush(ButtonVisibility == Visibility.Visible ? Colors.White : Colors.Gray);        }        private void ShowSetHandle()        {            if (GlobalVariable.FloatingStyle == FloatingStyleEnum.Image)            {                GlobalVariable.FloatingViewHide();            }            else if (GlobalVariable.FloatingStyle == FloatingStyleEnum.Title)            {                GlobalVariable.FloatingTitleStyleViewHide();            }
            else if (GlobalVariable.FloatingStyle == FloatingStyleEnum.Status)
            {
                GlobalVariable.SysRuntimeStatusViewHide();
            }            GlobalVariable.CustomSetViewShow();        }





        #endregion
        #region  外部引用事件函数处理
        private void TaskListPageModelOnTaskListChanged(CorrespondenceModel corr)        {            if (corr.Message is ObservableCollection<TaskListItemUserControl> taskList)                Title = $"还有 {taskList.Count(x => x.IsCompleted.IsChecked == false)} 个任务未完成";        }



        #endregion
        private void InitControlImage()        {



            //SettingButtonGeometry = pathGeometry;

        }        private void InitRegister()        {            WeakReferenceMessenger.Default.Register<CorrespondenceModel, string>(this, Const.CONST_TASK_LIST_CHANGED,                (obj, msg) => { TaskListPageModelOnTaskListChanged(msg); });        }        public FloatingTitleViewModel()        {            ButtonVisibility = Visibility.Collapsed;
            //ShowButtonGeometry = GlobalVariable.CollapsedGeometry;
            TitleBorderBrush = new SolidColorBrush(Colors.Gray);            InitControlImage();            ButtonVisibilityChangedCommand = new RelayCommand(ButtonVisibilityChangedHandle);            ShowSetCommand = new RelayCommand(ShowSetHandle);            InitRegister();        }    }}