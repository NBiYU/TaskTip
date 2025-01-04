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

            //tcpService.StartAsync(new System.Threading.CancellationToken());
            GlobalVariable.JsonConfiguration.TryGetValue<List<GradientColorModel>>("Themes:Default:1:CategoryThemes");

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

        private void AddWindow(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<FloatingView>();
            services.AddSingleton<FloatingTitleStyleView>();
            services.AddSingleton<FloatingTaskView>();
            services.AddSingleton<CustomSetView>();
            services.AddSingleton<EditFullScreenView>();
            services.AddSingleton<ResoureceLoading>();
            services.AddSingleton<SysRuntimeStatusView>();
            services.AddSingleton<TaskMenoView>();
        }

        private void AddPage(IServiceCollection services)
        {
            services.AddSingleton<CustomThemePage>();
            services.AddSingleton<ComplateWorkTimePage>();
            services.AddSingleton<RecordPage>();
            services.AddSingleton<SysRuntimeStatusSetPage>();
            services.AddSingleton<TaskListPage>();
            services.AddSingleton<TaskTipView>();
        }

        private void AddToolPage(IServiceCollection services)
        {
            services.AddSingleton<JsonPage>();
            services.AddSingleton<RegexPage>();
            services.AddSingleton<ToolMainPage>();
        }

        private void AddFiction(IServiceCollection services)
        {
            services.AddSingleton<FictionAccountPage>();
            services.AddSingleton<FictionCategoryPage>();
            services.AddSingleton<FictionSearchPage>();
            services.AddSingleton<FictionPage>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
