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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskTip.Views
{
    /// <summary>
    /// TaskListItemUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class TaskListItemUserControl : UserControl
    {
        public static event EventHandler IsCompleteMsg;
        public TaskListItemUserControl()
        {
            InitializeComponent();
        }
        private void IsCompleted_OnChecked(object sender, RoutedEventArgs e)
        {
            IsCompleteMsg?.Invoke(this.Guid.Text, null);
        }
    }
}
