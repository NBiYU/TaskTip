using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using TaskTip.Enums;
using TaskTip.Services;

namespace TaskTip
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();
            var isFirstLoad = InitConfig();

            //if (isFirstLoad)
            //{
            //    GlobalVariable.FloatingViewClose();
            //    GlobalVariable.FloatingTitleStyleViewClose();
            //    GlobalVariable.SysRuntimeStatusViewClose();
            //    GlobalVariable.CustomSetViewShow();
            //}
            //else
            //{
            //    if (GlobalVariable.FloatingStyle == FloatingStyleEnum.Image)
            //    {
            //        GlobalVariable.FloatingViewShow();
            //    }
            //    else if (GlobalVariable.FloatingStyle == FloatingStyleEnum.Title)
            //    {
            //        GlobalVariable.FloatingTitleStyleViewShow();
            //    }
            //    else if (GlobalVariable.FloatingStyle == FloatingStyleEnum.Status)
            //    {
            //        GlobalVariable.SysRuntimeStatusViewShow();
            //    }
            //}
            //var window = new RegexHelpView();
            //window.Show();
            //this.Close();
        }

        private bool InitConfig()
        {
            var processPathInfo = Process.GetCurrentProcess().MainModule;
            var runtimePath = processPathInfo.FileName.Split(processPathInfo.ModuleName)[0];
            var configPath = runtimePath + "config.txt";
            GlobalVariable.OperationRecordPath = runtimePath + "OperationRecordList.json";
            GlobalVariable.CustomThemePath = runtimePath + "CustomTheme.json";


            if (File.Exists(configPath)) return false;
            GlobalVariable.SaveConfig("LocalKey", Guid.NewGuid());
            AddRule();

            File.WriteAllText(configPath, "Test");

            Directory.CreateDirectory(GlobalVariable.RecordFilePath);
            Directory.CreateDirectory(GlobalVariable.TaskFilePath);
            Directory.CreateDirectory(GlobalVariable.MenoFilePath);
            Directory.CreateDirectory(GlobalVariable.FictionCachePath);

            File.WriteAllText(GlobalVariable.OperationRecordPath, string.Empty);
            File.WriteAllText(GlobalVariable.CustomThemePath, string.Empty);



            return true;
        }



        private void AddRule()
        {

            string ruleName = "TaskTipTcp"; // 防火墙规则名称
            string portNumber = GlobalVariable.JsonConfiguration["TCPReceive:Port"] ?? throw new Exception("注册端口号异常"); // 要开放的端口号

            // 创建进程启动信息
            ProcessStartInfo startInfo = new();
            startInfo.FileName = "netsh";
            startInfo.Arguments =
                $"advfirewall firewall add rule name=\"{ruleName}\" dir=in action=allow protocol=TCP localport={portNumber}";
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            // 启动进程并等待执行完成
            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
            }
        }
    }
}