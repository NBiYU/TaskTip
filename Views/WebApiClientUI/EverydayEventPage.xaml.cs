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

namespace TaskTip.Pages.WebApiClientUI
{
    /// <summary>
    /// EverydayEventPage.xaml µÄ½»»¥Âß¼­
    /// </summary>
    public partial class EverydayEventPage : Page
    {
        public EverydayEventPage()
        {
            InitializeComponent();
        }

        private void UIElement_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = false;
        }
    }
}
