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
using TaskTip.Services;

namespace TaskTip.Views.Windows
{
    /// <summary>
    /// EditFullScreenView.xaml µÄ½»»¥Âß¼­
    /// </summary>
    public partial class EditFullScreenView : Window
    {
        public bool IsClosed { get; set; } = false;
        public EditFullScreenView()
        {
            InitializeComponent();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsClosed = true;
        }

        private void EditFullScreenView_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = !this.Topmost;
            this.FixedImage.Source = this.Topmost ? (BitmapImage)Application.Current.Resources["Unpin"] : (BitmapImage)Application.Current.Resources["Fixed"];
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            this.FixedBorder.Margin = new Thickness(0);
        }

        private void FixedBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            this.FixedBorder.Margin = new Thickness(-10);
        }
    }
}
