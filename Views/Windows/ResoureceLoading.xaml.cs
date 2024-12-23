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
using TaskTip.Common.Extends;
using TaskTip.Services;
using TaskTip.ViewModels.WindowModel;

namespace TaskTip.Views.Windows
{
    /// <summary>
    /// ResoureceLoading.xaml 的交互逻辑
    /// </summary>
    public partial class ResoureceLoading : Window
    {
        public ResoureceLoading()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(DataContext is ResourceLoadingVM vm)
            {
                vm.LoadedStart();
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            if (DataContext is ResourceLoadingVM vm)
            {
                vm.Cancel(this);
            }
            base.OnClosed(e);
        }
    }
}
