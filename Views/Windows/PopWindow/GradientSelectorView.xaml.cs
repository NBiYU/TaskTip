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

namespace TaskTip.Views.Windows.PopWindow
{
    /// <summary>
    /// GradientSelectorView.xaml µÄ½»»¥Âß¼­
    /// </summary>
    public partial class GradientSelectorView : Window
    {
        public GradientSelectorView()
        {
            InitializeComponent();
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true; ;
        }
    }
}
