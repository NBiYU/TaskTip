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

using TaskTip.Models.DataModel;

namespace TaskTip.Views.Windows.PopWindow
{
    /// <summary>
    /// PopInteractiveView.xaml 的交互逻辑
    /// </summary>
    public partial class PopInteractiveView : Window
    {

        private PopInteractiveModel _model;
        public PopInteractiveView()
        {
            InitializeComponent();
        }

        public PopInteractiveView(PopInteractiveModel model)
        {
            InitializeComponent();
            _model = model;
            TipMessage.Text = model.Title;
            if (model.InteractiveItemModels is { Count: > 0 })
            {
                ControlList.Visibility = Visibility.Visible;
                Layout.Visibility = Visibility.Collapsed;
                ControlList.ItemsSource = model.InteractiveItemModels;
            }
            else
            {
                Layout.Visibility = Visibility.Visible;
                ControlList.Visibility = Visibility.Collapsed;
            }

        }
        public delegate void EventHandler(object obj, EventArgs e);
        public event EventHandler PopClose;
        public event EventHandler PopConfirm;
        public event EventHandler PopOnClose;

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            if(Owner == null)
            {
                this.DialogResult = false;
            }
            else
            {
                Close();
            }
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
            if (Owner == null)
            {
                this.DialogResult = true;
            }
            else
            {
                Close();
            }
            PopConfirm?.Invoke(_model == null ? this : _model, EventArgs.Empty);

        }

        protected override void OnClosed(EventArgs e)
        {
            PopOnClose?.Invoke(this, EventArgs.Empty);
            base.OnClosed(e);
        }
    }
}
