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
using CommunityToolkit.Mvvm.Messaging;
using TaskTip.Models;
using TaskTip.UserControls;
using TaskTip.ViewModels;
using Control = System.Windows.Forms.Control;

namespace TaskTip.Views.Windows
{
    /// <summary>
    /// PopView.xaml µÄ½»»¥Âß¼­
    /// </summary>
    public partial class PopView : Window
    {
        public PopView()
        {
            InitializeComponent();
        }

        public delegate void EventHandler(object obj, EventArgs e);
        public event EventHandler PopClose;
        public event EventHandler PopConfirm;
        public event EventHandler PopOnClose;

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            PopClose?.Invoke(this, EventArgs.Empty);
        }

        private void PopView_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                base.DragMove();
            }
            catch
            {

            }
        }

        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            PopConfirm?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnClosed(EventArgs e)
        {
            PopOnClose?.Invoke(this, EventArgs.Empty);
            base.OnClosed(e);
        }


    }
}
