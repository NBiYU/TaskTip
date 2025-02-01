using Hardcodet.Wpf.TaskbarNotification;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NLog;
using SQLitePCL;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;

using TaskTip.Models.Entities;
using TaskTip.Models.SettingModel;
using TaskTip.Pages;
using TaskTip.Services;
using TaskTip.Views;
using TaskTip.Views.FictionPage;
using TaskTip.Views.Pages;
using TaskTip.Views.ToolPage;
using TaskTip.Views.Windows;
using TaskTip.Views.Windows.PopWindow;

using TaskTipProject.Views.Pages;

namespace TaskTip
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon _taskbarIcon;
        public IServiceProvider ServiceProvider { get; private set; }

        public IConfiguration Configuration { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            _taskbarIcon = (TaskbarIcon)FindResource("Icon");
            NLog.LogManager.Setup().LoadConfigurationFromFile("nlog.config");

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            // 配置依赖注入
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            base.OnStartup(e);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // 注册 DbContext，并从配置文件读取连接字符串
            var dir =  AppDomain.CurrentDomain.BaseDirectory;
            var sqliteDb = $"Data Source={dir}\\Resources\\TaskTipDB.db";
            services.AddDbContext<TaskTipDbContext>(options =>
                options.UseSqlite(sqliteDb));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
