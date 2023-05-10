using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using TaskTip.Services;
using TaskTip.Views;
namespace TaskTip.ViewModels
{
    internal class FloatingTitleViewModel : ObservableObject
    {

        #region 属性
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

        private Geometry _showButtonGeometry;
        public Geometry ShowButtonGeometry
        {
            get => _showButtonGeometry;
            set => SetProperty(ref _showButtonGeometry, value);
        }

        private Geometry _settingButtonGeometry;
        public Geometry SettingButtonGeometry
        {
            get => _settingButtonGeometry;
            set => SetProperty(ref _settingButtonGeometry, value);
        }

        public Geometry CollapsedGeometry
        {
            get
            {
                PathFigure figure = new PathFigure();
                figure.StartPoint = new Point(-5, 28);
                figure.Segments.Add(new LineSegment(new Point(10, 14), true));
                figure.Segments.Add(new LineSegment(new Point(-5, 0), true));
                PathGeometry pathGeometry = new PathGeometry();
                pathGeometry.Figures.Add(figure);
                return pathGeometry;
            }
        }

        private Geometry VisibilityGeometry
        {
            get
            {
                PathFigure figure = new PathFigure();
                figure.StartPoint = new Point(0, 0);
                figure.Segments.Add(new LineSegment(new Point(15, 15), true));
                figure.Segments.Add(new LineSegment(new Point(30, 0), true));
                PathGeometry pathGeometry = new PathGeometry();
                pathGeometry.Figures.Add(figure);
                return pathGeometry;
            }
        }

        #endregion

        #region 控件指令

        public RelayCommand ButtonVisibilityChangedCommand
        {
            get;
            set;
        }

        public RelayCommand ShowSetCommand { get; set; }

        #endregion


        #region 指令处理函数

        private void ButtonVisibilityChangedHandle()
        {
            ButtonVisibility = ButtonVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            ShowButtonGeometry = ButtonVisibility == Visibility.Visible ? VisibilityGeometry : CollapsedGeometry;
            TitleBorderBrush = new SolidColorBrush(ButtonVisibility == Visibility.Visible ? Colors.White : Colors.Gray);
        }

        private void ShowSetHandle()
        {
            if (bool.Parse(ConfigurationManager.AppSettings.Get("IsFloatingImageStyle")))
            {
                GlobalVariable.FloatingViewHide();
            }
            else
            {
                GlobalVariable.FloatingTitleViewHide();
            }
            GlobalVariable.CustomSetViewShow();
        }

        #endregion

        #region  外部引用事件函数处理

        private void TaskListPageModelOnTaskListChanged(object sender, EventArgs e)
        {
            Title =
                $"还有 {((ObservableCollection<TaskListItemUserControl>)sender).Count(x => x.IsCompleted.IsChecked == false)} 个任务未完成";
        }

        #endregion

        private void InitControlImage()
        {



            //SettingButtonGeometry = pathGeometry;

        }

        public FloatingTitleViewModel()
        {
            ButtonVisibility = Visibility.Collapsed;
            ShowButtonGeometry = CollapsedGeometry;
            TitleBorderBrush = new SolidColorBrush(Colors.Gray);
            InitControlImage();

            ButtonVisibilityChangedCommand = new RelayCommand(ButtonVisibilityChangedHandle);
            ShowSetCommand = new RelayCommand(ShowSetHandle);
            TaskListPageModel.TaskListChanged += TaskListPageModelOnTaskListChanged;
        }
    }
}
