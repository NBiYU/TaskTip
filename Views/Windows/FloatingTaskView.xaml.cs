using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
using TaskTip.Common;
using TaskTip.Models.DataModel;
using TaskTip.Models.Entities;
using TaskTip.Services;
using TaskTip.ViewModels.WindowModel;

namespace TaskTip.Views.Windows
{
    /// <summary>
    /// FloatingTaskView.xaml 的交互逻辑
    /// </summary>
    public partial class FloatingTaskView : Window
    {
        public bool IsClosed { get; set; } = false;

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsClosed = true;
        }
        public FloatingTaskView()
        {
            Left = WindowResource.Left;
            Top = WindowResource.Top;
            InitializeComponent();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ClickCount > 1)
                {
                    WindowResource.TaskMenoViewShow();
                    this.Close();
                }
                else
                {
                    if(DataContext is FloatingTaskVM vm)
                    {
                        vm.TopListVisibility = vm.TopListVisibility == Visibility.Visible ? Visibility.Collapsed:Visibility.Visible;
                    }
                }
                DragMove();
            }
            catch { }

        }
    }
}
