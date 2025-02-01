using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TaskTip.Common.Extends;
using TaskTip.Services;
using TaskTip.ViewModels.WindowModel;
using TaskTip.Views.UserControls;

namespace TaskTip.Views.Windows
{
    /// <summary>
    /// ResoureceLoading.xaml 的交互逻辑
    /// </summary>
    public partial class ResoureceLoading : Window
    {
        private CancellationTokenSource _cts;
        private bool _isSuccess;
        private int _totalLength;
        public ResoureceLoading()
        {
            InitializeComponent();
        }


        private void Start()
        {
            _cts = new CancellationTokenSource();
            var progress = new Progress<int>(value =>
            {
                ProgressValue.Value = ((double)value + 1) / (double)_totalLength * 100;
            });
            var total = new Progress<int>(value =>
            {
                _totalLength = value;
            });
            Dispatcher.BeginInvoke(async () =>
            {
                //var result = await WorkFunc(total, progress, _cts.Token);
                //if (result) WorkState = false;
            }
            );
        }
        private void Cancel()
        {
            _cts?.Cancel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(DataContext is ResourceLoadingVM vm)
            {
                vm.LoadedStart();
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            if (DataContext is ResourceLoadingVM vm)
            {
                vm.Cancel(this);
            }
            base.OnClosed(e);
        }
    }
}
