using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TaskTip.Views.UserControls
{
    /// <summary>
    /// LodingUC.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingUC : UserControl
    {
        private CancellationTokenSource _cts;
        private bool _isSuccess;
        private int _totalLength;
        public LoadingUC()
        {
            InitializeComponent();
        }

        #region WorkFuncProperty
        public static readonly DependencyProperty WorkFuncProperty =
            DependencyProperty.Register(nameof(WorkFunc), typeof(Func<IProgress<int>, IProgress<int>, CancellationToken, Task<bool>>), typeof(LoadingUC),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 第一个是总数，第二个是当前进度，第三个是任务取消Token
        /// </summary>
        public Func<IProgress<int>, IProgress<int>, CancellationToken, Task<bool>> WorkFunc
        {
            get => (Func<IProgress<int>, IProgress<int>, CancellationToken, Task<bool>>)GetValue(WorkFuncProperty);
            set => SetValue(WorkFuncProperty, value);
        }
        #endregion

        #region WorkStateProperty

        public static readonly DependencyProperty WorkStateProperty =
            DependencyProperty.Register(nameof(WorkState), typeof(bool), typeof(LoadingUC),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnWorkStateChanged));

        private static void OnWorkStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is LoadingUC uc)
            {
                if ((bool)e.NewValue == true)
                {
                    uc.Start();
                }
                else
                {
                    uc.Cancel();
                }
            }

        }

        public bool WorkState
        {
            get => (bool)GetValue(WorkStateProperty);
            set => SetValue(WorkStateProperty, value);
        }

        #endregion

        private void Start()
        {
            //_cts = new CancellationTokenSource();
            //var progress = new Progress<int>(value =>
            //{
            //    ProgressValue.Value = ((double)value+1)/ (double)_totalLength * 100;
            //});
            //var total = new Progress<int>(value =>
            //{
            //    _totalLength = value;
            //});
            //Dispatcher.BeginInvoke(async () =>
            //    {
            //        var result = await WorkFunc(total, progress, _cts.Token);
            //        if(result) WorkState = false;
            //    }
            //);
        }
        private void Cancel()
        {
            _cts?.Cancel();
        }

    }
}
