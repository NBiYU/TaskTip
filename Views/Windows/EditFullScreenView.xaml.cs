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
    }
}
