using System.Diagnostics;
using System.IO;
using System.Windows;
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
            //this.WindowState = WindowState.Minimized;
            var processPath = Process.GetCurrentProcess().MainModule?.FileName;
            var configPath = processPath!.Replace("TaskTip.exe", "config.txt");

            if (File.Exists(configPath))
            {
                GlobalVariable.TaskMenoViewHide();

                if (GlobalVariable.IsFloatingImageStyle) GlobalVariable.FloatingViewShow();
                else GlobalVariable.FloatingTitleStyleViewShow();
            }
            else
            {
                var taskDir = processPath.Replace("TaskTip.exe", "TaskFile");
                var menoDir = processPath.Replace("TaskTip.exe", "MenoFile");

                if (!Directory.Exists(taskDir)) Directory.CreateDirectory(taskDir);
                if (!Directory.Exists(menoDir)) Directory.CreateDirectory(menoDir);

                File.WriteAllText(configPath, "Test");
                GlobalVariable.CustomSetViewShow();
            }

            InitializeComponent();
            this.Close();
        }
    }
}