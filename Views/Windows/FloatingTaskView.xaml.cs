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
            Left = GlobalVariable.Left;
            Top = GlobalVariable.Top;
            InitializeComponent();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ClickCount > 1)
                {
                    GlobalVariable.TaskMenoViewShow();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloatingTaskVM vm && (sender as Button).DataContext is TaskFileModel fvm)
            {
                if (vm.DtlContent == fvm.EditTextText)
                {
                    vm.DtlContent = string.Empty;
                    vm.DtlVisibility = Visibility.Collapsed;
                }
                else
                {
                    vm.DtlContent = fvm.EditTextText;
                    vm.DtlVisibility = Visibility.Visible;
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if(sender is CheckBox box && box.DataContext is TaskFileModel model)
            {
                model.IsCompleted = true;
                model.CompletedDateTime = DateTime.Now;
                WriteModel(model);
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox box && box.DataContext is TaskFileModel model)
            {
                model.IsCompleted = false;
                model.CompletedDateTime = DateTime.MinValue;
                WriteModel(model);
            }
        }
        private void WriteModel(TaskFileModel model)
        {
            var path = System.IO.Path.Combine(GlobalVariable.TaskFilePath, $"{model.GUID}{GlobalVariable.EndFileFormat}");
            File.WriteAllText(path,JsonConvert.SerializeObject(model));
            WeakReferenceMessenger.Default.Send(GlobalVariable.TaskFilePath, Const.CONST_TASK_RELOAD);
        }
    }
}
