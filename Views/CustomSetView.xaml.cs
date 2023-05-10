using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TaskTip.Views
{

    /// <summary>
    /// CustomSetView.xaml 的交互逻辑
    /// </summary>
    public partial class CustomSetView : Window
    {
        public static event EventHandler CustomWindowStateChanged;
        public CustomSetView()
        {
            InitializeComponent();
        }
        private void CustomSetView_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }

        protected override void OnActivated(EventArgs e)
        {
            CustomWindowStateChanged?.Invoke(null, null);
            base.OnActivated(e);
        }

    }
}
