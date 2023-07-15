using System.Diagnostics;
using System.IO;
using System.Windows;
using TaskTip.Services;
using TaskTip.ViewModels;

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
            else
            {

                File.WriteAllText(configPath, "Test");

                GlobalVariable.FloatingViewClose();
                GlobalVariable.FloatingTitleStyleViewClose();
                GlobalVariable.CustomSetViewShow();
            }

            InitializeComponent();
            this.Close();
        }
    }
}