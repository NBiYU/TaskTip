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
using CommunityToolkit.Mvvm.Messaging;
using TaskTip.Services;

namespace TaskTip.Views
{

    /// <summary>
    /// CustomSetView.xaml µÄ½»»¥Âß¼­
    /// </summary>
    public partial class CustomSetView : Window
    {
        public bool IsClosed { get; set; } = false;
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
            //WeakReferenceMessenger.Default.Send<string,string>(string.Empty,Const.CONST_SHOW_CUSTOM);
            base.OnActivated(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsClosed = true;
        }

    }
}
