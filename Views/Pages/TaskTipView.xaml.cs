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
using static System.Net.Mime.MediaTypeNames;

namespace TaskTip.Views
{
    /// <summary>
    /// TaskTipView.xaml µÄ½»»¥Âß¼­
    /// </summary>
    public partial class TaskTipView : Page
    {
        public TaskTipView(string title, string text, DateTime taskTimePlan)
        {
            InitializeComponent();
            this.TitleText.Text = title;
            this.Text.Text = text;
            this.TaskTimePlan.Text = taskTimePlan.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void MenuItem1_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MenuItem2_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MenuItem3_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
