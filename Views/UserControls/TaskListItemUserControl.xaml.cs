using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TaskTip.Services;

namespace TaskTip.Views
{
    /// <summary>
    /// TaskListItemUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class TaskListItemUserControl : UserControl
    {
        public TaskListItemUserControl()
        {
            InitializeComponent();
        }
        private void IsCompleted_OnChecked(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Send(this.Guid.Text, Const.CONST_COMPLETE_TASK_GUID);
        }
    }
}
