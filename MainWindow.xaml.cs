using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using TaskTip.Services;
using ConfigurationManager = System.Configuration.ConfigurationManager;

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
            var configPath = Process.GetCurrentProcess().MainModule?.FileName.Replace("TaskTip.exe", "config.txt");

            if (File.Exists(configPath))
            {
                GlobalVariable.TaskMenoViewHide();

                if (GlobalVariable.IsFloatingImageStyle) GlobalVariable.FloatingViewShow();
                else GlobalVariable.FloatingTitleViewShow();
            }
            else
            {
                File.WriteAllText(configPath, "Test");
                GlobalVariable.CustomSetViewShow();
            }
            InitializeComponent();
            this.Close();
        }
    }
}