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
using TaskTip.Services;
using TaskTip.ViewModels;

namespace TaskTip.Views
{
    /// <summary>
    /// FloatingView.xaml 的交互逻辑
    /// </summary>
    public partial class FloatingView : Window
    {
        public static event EventHandler OpenTaskMenoUI;
        public FloatingView()
        {


            var screenHeight = SystemParameters.WorkArea.Height;
            var screenWidth = SystemParameters.WorkArea.Width;
            this.Left = screenWidth * 0.5;
            this.Top = screenHeight * 0.5;

            WindowStyleChanged(null, null);

            InitializeComponent();
            GlobalVariable.ConfigChanged += WindowStyleChanged;
            CustomSetViewModel.FloatingSizeEvent += WindowStyleChanged;
        }

        /// <summary>
        /// 窗口参数变更事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowStyleChanged(object sender, EventArgs e)
        {
            var size = sender?.ToString().Split(':');

            if (sender != null)
            {

                SetWindowStyle((double.Parse(size[0]), double.Parse(size[1])), (Left, Top));
                return;

            }
            SetWindowStyle(
                ((double.Parse(ConfigurationManager.AppSettings["FloatingSetWidth"])), (double.Parse(ConfigurationManager.AppSettings["FloatingSetHeight"]))),
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
                var widthRatio = Width - GlobalVariable.TaskMenoView.Width;
                var heightRatio = Height - GlobalVariable.TaskMenoView.Height;
                GlobalVariable.TaskMenoView.Left = Left + widthRatio;
                GlobalVariable.TaskMenoView.Top = Top + heightRatio;

                OpenTaskMenoUI?.Invoke(DateTime.Now, null);

                GlobalVariable.TaskMenoViewShow();
                this.Hide();
            }
            base.DragMove();
        }
    }
}
