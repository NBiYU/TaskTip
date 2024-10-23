using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TaskTip.ViewModels.WindowModel;

namespace TaskTip.Views.Windows
{
    /// <summary>
    /// SyncMainView.xaml 的交互逻辑
    /// </summary>
    public partial class SyncMainView : Window
    {
        public SyncMainView()
        {
            InitializeComponent();
        }


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch { }
        }

        private void Mini_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void WindowSize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }else
            {
                this.WindowState= WindowState.Normal;
            }
        }

        private void Closse_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private async void ConfrimSync_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SyncMainVM vm)
            {
                //await vm.ConfirmSyncCategory(((Button)sender).Content.ToString());
            }
        }
    }
}
