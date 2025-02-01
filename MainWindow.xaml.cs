using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

using TaskTip.Common;
using TaskTip.Common.Extends;
using TaskTip.Models.Enums;
using TaskTip.Services;
using TaskTip.Views.Windows;
using TaskTip.Views.Windows.PopWindow;

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
            #region 初始化

            var isFirstLoad = InitConfig();


            var window = new ResourceUpdateWindow();
            window.ShowDialog();
            if (isFirstLoad)
            {
                WindowResource.FloatingCloseAll();
                WindowResource.CustomSetViewShow();
            }
            else
            {

                if (GlobalVariable.FloatingStyle != FloatingStyleEnum.Image)
                {
                    WindowResource.FloatingCloseAll();
                }
                WindowResource.SwitchFloatingShow(GlobalVariable.FloatingStyle);
            }


            #endregion
            this.Close();
        }
        private bool InitConfig()
        {
            if (GlobalVariable.Version.IsNullOrEmpty())
            {
                return true;
            }

            //var configPath = runtimePath + "config.txt";
            //GlobalVariable.OperationRecordPath = runtimePath + "OperationRecordList.json";



            //if (File.Exists(configPath)) return false;
            //GlobalVariable.SaveConfig("LocalKey", Guid.NewGuid());

            //File.WriteAllText(configPath, "Test");

            //Directory.CreateDirectory(GlobalVariable.RecordFilePath);
            //Directory.CreateDirectory(GlobalVariable.TaskFilePath);
            //Directory.CreateDirectory(GlobalVariable.MenoFilePath);
            //Directory.CreateDirectory(GlobalVariable.FictionCachePath);

            //File.WriteAllText(GlobalVariable.OperationRecordPath, string.Empty);
            //File.WriteAllText(GlobalVariable.CustomThemePath, string.Empty);

            return true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var a = new ClockSelectorPop();
            a.Confirmed += (o,e) => {
                MessageBox.Show($"选择时间：{o}");
            };
            a.ShowDialog();
        }
    }

    
}