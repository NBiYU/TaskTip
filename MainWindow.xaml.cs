using NLog.LayoutRenderers.Wrappers;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using TaskTip.Common;
using TaskTip.Services;
using TaskTip.ViewModels.PageModel;
using TaskTip.Views.Windows.PopWindow;

namespace TaskTip
{
    /// <summary>
    /// MainWindow.xaml �Ľ����߼�
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();
            var isFirstLoad = InitConfig();

            if (isFirstLoad)
            {
                GlobalVariable.FloatingViewClose();
                GlobalVariable.FloatingTitleStyleViewClose();
                GlobalVariable.CustomSetViewShow();
            }
            else
            {
                if (GlobalVariable.IsFloatingImageStyle)
                {
                    GlobalVariable.FloatingViewShow();
                    GlobalVariable.FloatingTitleStyleViewClose();
                }
                else
                {
                    GlobalVariable.FloatingTitleStyleViewShow();
                    GlobalVariable.FloatingViewClose();
                }
            }
            //var window = new RegexHelpView();
            //window.Show();
            this.Close();
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

            string ruleName = "TaskTipTcp"; // ����ǽ��������
            string portNumber = GlobalVariable.JsonConfiguration["TCPReceive:Port"] ?? throw new Exception("ע��˿ں��쳣"); // Ҫ���ŵĶ˿ں�

            // ��������������Ϣ
            ProcessStartInfo startInfo = new();
            startInfo.FileName = "netsh";
            startInfo.Arguments =
                $"advfirewall firewall add rule name=\"{ruleName}\" dir=in action=allow protocol=TCP localport={portNumber}";
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            // �������̲��ȴ�ִ�����
            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
            }
        }
    }
}