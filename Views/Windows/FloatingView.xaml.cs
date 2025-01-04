using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.Messaging;
using TaskTip.Common;
using TaskTip.Services;
using TaskTip.ViewModels;

namespace TaskTip.Views
{
    /// <summary>
    /// FloatingView.xaml 的交互逻辑
    /// </summary>
    public partial class FloatingView : Window
    {
        public FloatingView()
        {
            Left = WindowResource.Left;
            Top = WindowResource.Top;
            WindowStyleChanged(null);

            InitializeComponent();

            InitRegister();
        }

        private void InitRegister()
        {
            WeakReferenceMessenger.Default.Register<string, string>(this, Const.CONST_CONFIG_CHANGED, (obj, msg) => WindowStyleChanged(msg));
            WeakReferenceMessenger.Default.Register<string, string>(this, Const.CONST_FLAOTING_SIZE_CHANGED, (obj, msg) => WindowStyleChanged(msg));
        }

        /// <summary>
        /// 窗口参数变更事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowStyleChanged(string sender)
        {
            if (!string.IsNullOrEmpty(sender))
            {
                var size = sender.Split(':');
                SetWindowStyle((double.Parse(size[0]), double.Parse(size[1])), (Left, Top));
                return;

            }
            SetWindowStyle(
                (GlobalVariable.FloatingSetWidth, GlobalVariable.FloatingSetHeight),
                (this.Left, this.Top));

        }

        /// <summary>
        /// 窗口参数变更
        /// </summary>
        /// <param name="size"></param>
        /// <param name="local"></param>
        private void SetWindowStyle((double, double) size, (double, double) local)
        {
            (this.Width, this.Height) = ((double, double))size;
            (this.Left, this.Top) = ((double, double))local;
        }


        private void FloatingView_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                //任务浮窗起始位置
                var widthRatio = Width - WindowResource.TaskMenoView.Width;
                var heightRatio = Height - WindowResource.TaskMenoView.Height;
                WindowResource.TaskMenoView.Left = Left + widthRatio;
                WindowResource.TaskMenoView.Top = Top + heightRatio;

                WeakReferenceMessenger.Default.Send(new { }, Const.CONST_OPEN_APPLICATTION);
                //OpenTaskMenoUI?.Invoke(DateTime.Now, null);
                WindowResource.TaskMenoViewShow();
                this.Hide();
            }
            base.DragMove();
        }

        public bool IsClosed { get; set; } = false;
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsClosed = true;
        }
    }
}
