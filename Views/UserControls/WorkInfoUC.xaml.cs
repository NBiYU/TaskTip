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
using TaskTip.ViewModels.UserViewModel;

namespace TaskTip.Views.UserControls
{
    /// <summary>
    /// WorkInfoUC.xaml 的交互逻辑
    /// </summary>
    public partial class WorkInfoUC : UserControl
    {
        public WorkInfoUC()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (DataContext is WorkInfoUCM vm)
                {
                    vm.Show();
                }
            }
        }
    }
}
