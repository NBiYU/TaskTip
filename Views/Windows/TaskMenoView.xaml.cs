using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;using System.Windows;using System.Windows.Controls;using System.Windows.Data;using System.Windows.Documents;using System.Windows.Input;using System.Windows.Media;using System.Windows.Media.Imaging;using System.Windows.Shapes;using TaskTip.Services;namespace TaskTip.Views{
    /// <summary>    /// TaskMenoView.xaml �Ľ����߼�    /// </summary>    public partial class TaskMenoView : Window    {        public TaskMenoView()        {            this.Width = SystemParameters.WorkArea.Height / 3;            this.Height = SystemParameters.WorkArea.Width / 3;            InitializeComponent();        }


        //private void TaskMenoView_OnLoaded(object sender, RoutedEventArgs e)
        //{
        //    this.Left = GlobalVariable.FloatingView.Left - GlobalVariable.TaskMenoView.Width + GlobalVariable.FloatingView.Width;
        //    this.Top = GlobalVariable.FloatingView.Top - GlobalVariable.TaskMenoView.Height + GlobalVariable.FloatingView.Height;
        //}

        public bool IsClosed { get; set; } = false;        protected override void OnClosed(EventArgs e)        {            base.OnClosed(e);            IsClosed = true;        }        private void TaskMenoView_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)        {            try            {                base.DragMove();            }            catch            {            }        }        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)        {            if (sender is Frame frame)            {                frame.Navigate(GlobalVariable.RecordPage);            }        }    }}