﻿using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

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
            base.OnStartup(e);
        }

    }
}
