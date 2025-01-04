using CommunityToolkit.Mvvm.Messaging;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using TaskTip.Common;
using TaskTip.Common.Extends;
using TaskTip.Common.Helpers;
using TaskTip.Models.ConfigModel;
using TaskTip.Models.DataModel;
using TaskTip.Services;
using TaskTip.Views.Windows.PopWindow;

namespace TaskTip.Views.Windows
{
    /// <summary>
    /// SysRuntimeStatusView.xaml 的交互逻辑
    /// </summary>
    public partial class SysRuntimeStatusView : Window
    {
        public bool IsClosed { get; set; } = false;
        public bool IsFixed { get; set; } = false;

        private SysRuntimeConfigModel _config { get; set; }
        private CancellationTokenSource _toeknSource;
        private CancellationToken _cancelToken;
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsClosed = true;
        }

        #region WindowsAPI 
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        // 导入 GetWindowRect 函数
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        // SetWindowPos 常量
        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOZORDER = 0x0004;
        public const uint SWP_NOACTIVATE = 0x0010;
        public const int SWP_SHOWWINDOW = 0x0040;
        public static readonly IntPtr HWND_TOP = new IntPtr(0);

        #endregion

        public SysRuntimeStatusView()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _toeknSource = new CancellationTokenSource();
            _cancelToken = _toeknSource.Token;
            _config = JsonConfigHelper.ReadConfig<SysRuntimeConfigModel>(Const.RuntimeStatusConfig);

            InitControl(_config);

            if (GlobalVariable.FloatingStatusIsFixed)
            {
                var createResult = InitWindowInTaskbar();
                if (!createResult) MessageBox.Show("创建任务栏窗口失败。");
                if (_config == null)
                {
                    Background = new SolidColorBrush(ThemeHelper.GetSystemThemeColor());
                }
                IsFixed = true;
            }
            else
            {
                IsFixed = false;
            }

            #region 刷新当前状态
            if (_config != null)
            {
                Application.Current.Dispatcher.BeginInvoke(async () =>
                {
                    while (true)
                    {
                        if (_cancelToken.IsCancellationRequested)
                        {
                            break;
                        }
                        CPU.Text = await RunTimeStatusHelper.GetCPUStatusAsync();
                        //await Task.Delay(1000);
                    }
                });
                Application.Current.Dispatcher.BeginInvoke(async () =>
                {
                    while (true)
                    {
                        if (_cancelToken.IsCancellationRequested)
                        {
                            break;
                        }
                        CurrentMemory.Text = await RunTimeStatusHelper.GetMemoryStatusAsync();
                        await Task.Delay(1000);
                    }
                });
                Application.Current.Dispatcher.BeginInvoke(async () =>
                {
                    while (true)
                    {
                        if (_cancelToken.IsCancellationRequested)
                        {
                            break;
                        }

                        if ((_config?.NetworkCardName).IsNullOrEmpty())
                            break;
                        NetDownloadSpeed.Text = await RunTimeStatusHelper.GetNetDownloadSpeedStatusAsync(_config?.NetworkCardName);
                    }

                });
                Application.Current.Dispatcher.BeginInvoke(async () =>
                {
                    while (true)
                    {
                        if (_cancelToken.IsCancellationRequested)
                        {
                            break;
                        }

                        if ((_config?.NetworkCardName).IsNullOrEmpty())
                            break;
                        NetUploadSpeed.Text = await RunTimeStatusHelper.GetNetUploadSpeedStatusAsync(_config.NetworkCardName);
                    }
                });
            }

            #endregion
        }

        private void InitControl(SysRuntimeConfigModel config)
        {
            if (config != null)
            {
                var cpuBrush = config.FontTheme.CategoryThemes[0].GetGradientBrush();
                CPU.Foreground = cpuBrush;
                CPUText.Foreground = cpuBrush;
                var memoryBrush = config.FontTheme.CategoryThemes[1].GetGradientBrush();
                CurrentMemory.Foreground = memoryBrush;
                CurrentMemoryText.Foreground = memoryBrush;
                var upBrush = config.FontTheme.CategoryThemes[2].GetGradientBrush();
                NetUploadSpeed.Foreground = upBrush;
                NetUploadSpeedText.Foreground = upBrush;
                var downBrush = config.FontTheme.CategoryThemes[3].GetGradientBrush();
                NetDownloadSpeed.Foreground = downBrush;
                NetDownloadSpeedText.Foreground = downBrush;
            }
            else
            {
                MessageBox.Show("当前配置不正确，请前往设置重新保存");
            }
        }

        private bool InitWindowInTaskbar()
        {

            IntPtr taskbarHandle = FindWindow("Shell_TrayWnd", null);

            if (taskbarHandle == IntPtr.Zero) return false;

            IntPtr trayNotifyWnd = FindWindowEx(taskbarHandle, IntPtr.Zero, "TrayNotifyWnd", null);

            if (trayNotifyWnd == IntPtr.Zero) return false;

            var trayNotifyWndButton = FindWindowEx(trayNotifyWnd, IntPtr.Zero, "Button", null);

            if (trayNotifyWndButton == IntPtr.Zero) return false;

            IntPtr windowHandle = new WindowInteropHelper(this).Handle;

            if (!GetWindowRect(trayNotifyWndButton, out var rect)) return false;

            SetParent(windowHandle, taskbarHandle);

            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;

            SetWindowPos(windowHandle, HWND_TOP, rect.Left - 230, 0, 230, 40, SWP_NOZORDER | SWP_SHOWWINDOW);
            return true;
        }

        private void SwitchNetworkCard(object obj, EventArgs e)
        {
            if (obj is MenuItem menu)
            {
                _toeknSource?.Cancel();
                _config.NetworkCardName = menu.Header.ToString();
                JsonConfigHelper.SaveConfig(_config, Const.RuntimeStatusConfig);
                Window_Loaded(null, null);
            }
        }
        private void ModifyNetworkCardIPv4(object obj, EventArgs e)
        {
            if (obj is MenuItem menu)
            {
                var mo = NetworkAdapterHelper.GetNetworkAdapterInstance(menu.Header.ToString());
                var data = mo.GetNetworkAdapterInfo();
                var model = new PopInteractiveModel
                {
                    Title = menu.Header.ToString(),
                    InteractiveItemModels = new List<PopInteractiveItemModel>()
                    {
                        new(){ Tip = "自动获取",ControlType=ControlTypeEnum.CheckBox,Data=data.DHCPEnabled},
                        new(){ Tip = "IP",ControlType=ControlTypeEnum.Input,Data=data.IPAddress.FirstOrDefault()},
                        new(){ Tip = "掩码",ControlType=ControlTypeEnum.Input,Data=data.IPSubnet.FirstOrDefault()},
                        new(){ Tip = "网关",ControlType=ControlTypeEnum.Input,Data=data.DefaultIPGateway.FirstOrDefault()},
                        new(){ Tip = "DNS",ControlType=ControlTypeEnum.Input,Data=data.DNSServerSearchOrder.FirstOrDefault()}
                    }
                };
                var dialog = new PopInteractiveView(model);
                var result = dialog.ShowDialog();
                if (result == true)
                {
                    NetworkAdapterHelper.SetIPWithDnsDHCP(_config.NetworkCardName,
                        (bool)(model.InteractiveItemModels[0]?.Data ?? false));
                    NetworkAdapterHelper.SetIPWithDnsAddress(_config.NetworkCardName,
                        model.InteractiveItemModels[1]?.Data?.ToString() ?? "",
                        model.InteractiveItemModels[2]?.Data?.ToString() ?? "",
                        model.InteractiveItemModels[3]?.Data?.ToString() ?? "",
                        model.InteractiveItemModels[4]?.Data?.ToString() ?? "");
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                //任务浮窗起始位置
                var widthRatio = Width - WindowResource.TaskMenoView.Width;
                var heightRatio = Height - WindowResource.TaskMenoView.Height;
                WindowResource.TaskMenoView.Left = Left + widthRatio;
                WindowResource.TaskMenoView.Top = Top + heightRatio;

                WeakReferenceMessenger.Default.Send(new { }, Const.CONST_OPEN_APPLICATTION);
                //OpenTaskMenoUI?.Invoke(DateTime.Now, null);
                WindowResource.TaskMenoViewShow();
                if (!IsFixed) this.Hide();

            }
            if (!IsFixed)
            {
                try
                {
                    base.DragMove();
                }
                catch { }
            }

        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_config == null) return;
            ContextMenu contextMenu = new ContextMenu() { };

            MenuItem networkAdaptersSetting = new MenuItem { Header = "网口设置" };
            MenuItem switchAdapter = new MenuItem { Header = "选择网卡" };
            var adapters = NetworkAdapterHelper.GetNetworkInterfaces();
            foreach (var adapter in adapters)
            {
                var menuItem = new MenuItem
                {
                    Header = adapter,
                    IsCheckable = true,
                    IsChecked = adapter == _config?.NetworkCardName,
                };
                menuItem.Click += SwitchNetworkCard;
                switchAdapter.Items.Add(menuItem);
            }
            MenuItem ipSetting = new MenuItem() { Header = "网络设置" };
            foreach (var adapter in adapters)
            {
                var menuItem = new MenuItem
                {
                    Header = adapter
                };

                menuItem.Click += ModifyNetworkCardIPv4;
                ipSetting.Items.Add(menuItem);
            }
            networkAdaptersSetting.Items.Add(switchAdapter);
            networkAdaptersSetting.Items.Add(ipSetting);


            contextMenu.Items.Add(networkAdaptersSetting);

            contextMenu.PlacementTarget = this;
            contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            contextMenu.IsOpen = true;

            e.Handled = true;

        }
    }
}
