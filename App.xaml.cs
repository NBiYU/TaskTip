using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Hardcodet.Wpf.TaskbarNotification;
using NLog;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskTip.Common.ExecuteServices;
using System.Threading.Tasks;
using TaskTip.Common.ServiceRegister;
using Autofac;
using Newtonsoft.Json;
using TaskTip.Services;
using System.Configuration;
using TaskTip.Common;
using TaskTip.Models.SettingModel;
using TaskTip.Common.Helpers;

namespace TaskTip
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon _taskbarIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            _taskbarIcon = (TaskbarIcon)FindResource("Icon");
            NLog.LogManager.Setup().LoadConfigurationFromFile("nlog.config");

            var container = ServiceRegister.InitRegister();
            var tcpService = container.Resolve<IHostedService>();
            //tcpService.StartAsync(new System.Threading.CancellationToken());
            GlobalVariable.JsonConfiguration.TryGetValue<List<GradientColorModel>>("Themes:Default:1:CategoryThemes");

            base.OnStartup(e);
        }
    }
}
