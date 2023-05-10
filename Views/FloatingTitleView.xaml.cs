using System;
using System.Collections.Generic;
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

namespace TaskTip.Views
{
    /// <summary>
    /// FloatingTitleView.xaml 的交互逻辑
    /// </summary>
    public  class FloatingTitleView : Window
    {
        //public FloatingTitleView()
        //{
        //    InitializeComponent();
        //}
        private void FloatingTitleView_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }

        private void FloatingTitleView_OnStateChanged(object sender, EventArgs e)
        {
            var thisWindow = (Window)sender;

            //if (thisWindow.WindowState == WindowState.Minimized)
            //{
            //    SettingAnimationPath.IsPlaying = false;
            //    ShowTaskAnimationPath.IsPlaying = false;
            //}
            //else
            //{
            //    SettingAnimationPath.IsPlaying = true;
            //    ShowTaskAnimationPath.IsPlaying = true;
            //}
        }

    }
}
