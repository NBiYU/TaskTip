using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;
using NLog;
using TaskTip.Common;
using TaskTip.Pages;
using TaskTip.ViewModels.WindowModel;
using TaskTip.Views;
using TaskTip.Views.Windows;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using MessageBox = System.Windows.MessageBox;

namespace TaskTip.Services
{
    public static class GlobalVariable
    {

        #region 共享属性

        public static string LocalKey => ConfigurationManager.AppSettings[nameof(LocalKey)] ?? "";

        /// <summary>
        /// 悬浮窗图片路径
        /// </summary>
        public static string FloatingBgPath => ConfigurationManager.AppSettings[nameof(FloatingBgPath)] ?? "";

        /// <summary>
        /// 悬浮窗图片自动大小
        /// </summary>
        public static bool AutoSizeImage => ValueToType(ConfigurationManager.AppSettings[nameof(AutoSizeImage)] ?? "false");
        /// <summary>
        /// WCF数据同步服务是否启用
        /// </summary>
        public static bool CanSyncEnable => ValueToType(ConfigurationManager.AppSettings[nameof(CanSyncEnable)] ?? "false");

        /// <summary>
        /// 软件文件夹保存路径
        /// </summary>
        public static string TaskTipPath
        {
            get
            {
                var processPathInfo = Process.GetCurrentProcess().MainModule;
                var runtimePath = processPathInfo.FileName.Split(processPathInfo.ModuleName)[0];
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings[nameof(TaskTipPath)]) ? runtimePath : ConfigurationManager.AppSettings[nameof(TaskTipPath)]!;
            }
        }

        /// <summary>
        /// 任务文件保存路径
        /// </summary>
        public static string TaskFilePath => Path.Combine(TaskTipPath ,nameof(TaskFilePath));

        /// <summary>
        /// 记事文件保存路径
        /// </summary>
        public static string MenoFilePath => Path.Combine(TaskTipPath, nameof(MenoFilePath));

        /// <summary>
        /// 记录文件路径
        /// </summary>
        public static string RecordFilePath => Path.Combine(TaskTipPath ,nameof(RecordFilePath));

        /// <summary>
        /// 目录树配置文件
        /// </summary>
        public static string MenuTreeConfigPath => Path.Combine(TaskTipPath , "MenuTreeConfig.json");

        public static string FictionProgressPath => Path.Combine(TaskTipPath  ,"Fictions" , "FictionProgress.json");

        public static string FictionCachePath => Path.Combine(TaskTipPath, "Fictions");
        public static string? WorkTimeRecordPath = Path.Combine(TaskTipPath, "WorkTime.json");

        public static string CustomThemePath { get; set; }

        public static string OperationRecordPath { get; set; }

        /// <summary>
        /// 悬浮窗宽度
        /// </summary>
        public static double FloatingSetWidth => ValueToType(ConfigurationManager.AppSettings[nameof(FloatingSetWidth)] ?? "0");

        /// <summary>
        /// 悬浮窗高度
        /// </summary>
        public static double FloatingSetHeight => ValueToType(ConfigurationManager.AppSettings[nameof(FloatingSetHeight)] ?? "0");

        /// <summary>
        /// 文件后缀名
        /// </summary>
        public static string EndFileFormat => ConfigurationManager.AppSettings[nameof(EndFileFormat)] ?? "";

        /// <summary>
        /// 第二天日报生成结束时间
        /// </summary>
        public static string DailyTaskEndTime => ConfigurationManager.AppSettings[nameof(DailyTaskEndTime)] ?? "11:30";

        /// <summary>
        /// 是否自动创建明日计划
        /// </summary>
        public static bool IsCreateTomorrowPlan => ValueToType(ConfigurationManager.AppSettings[nameof(IsCreateTomorrowPlan)] ?? "true");

        /// <summary>
        /// 是否启用悬浮窗样式
        /// </summary>
        public static bool IsFloatingImageStyle => ValueToType(ConfigurationManager.AppSettings[nameof(IsFloatingImageStyle)] ?? "true");

        /// <summary>
        /// 自动删除时间
        /// </summary>
        public static int DeleteTimes => ValueToType(ConfigurationManager.AppSettings[nameof(DeleteTimes)] ?? "99");

        public static bool IsEnableAutoDelete => ValueToType(ConfigurationManager.AppSettings[nameof(IsEnableAutoDelete)] ?? "false");

        public static Configuration Configurations =
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public static ConfigurationHelper JsonConfiguration = new();

        public static Logger LogHelper = NLog.LogManager.GetCurrentClassLogger();

        public static string[] ThemeStyleKeys => (string[])Application.Current.Resources["ThemeColorArray"];

        public static string WorkFinishTime => ConfigurationManager.AppSettings[nameof(WorkFinishTime)] ?? "";
        public static double SiestaTime => double.Parse(ConfigurationManager.AppSettings[nameof(SiestaTime)] ?? "0.0");
        public static double AgainWorkGapTime => double.Parse(ConfigurationManager.AppSettings[nameof(AgainWorkGapTime)] ?? "0");
        internal static string WorkStartTime => ConfigurationManager.AppSettings[nameof(WorkStartTime)] ?? "";




        #endregion

        #region 静态页面

        public static RecordPage RecordPage = new();

        #endregion

        #region 配置操作


        /// <summary>
        /// 刷入配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="variableName">键</param>
        /// <param name="variable">值</param>
        public static void SaveConfig<T>(string variableName, T variable)
        {
            try
            {
                Configurations.AppSettings.Settings[variableName].Value = variable?.ToString();
                Configurations.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch
            {
                MessageBox.Show(variableName + "\t与值" + variable?.ToString() + "不匹配");
            }
        }


        /// <summary>
        /// 刷入配置
        /// </summary>
        /// <param name="val"></param>
        public static void SaveConfig(Dictionary<string, string> val)
        {
            try
            {
                foreach (var v in val)
                {
                    Configurations.AppSettings.Settings[v.Key].Value = v.Value;
                }

                Configurations.Save();
                ConfigurationManager.RefreshSection("appSettings");

                WeakReferenceMessenger.Default.Send<string, string>(string.Empty, Const.CONST_CONFIG_CHANGED);
            }
            catch (Exception e)
            {
                MessageBox.Show($"数据刷入配置错误:{e.Message}");
            }
        }


        /// <summary>
        /// 把string类型转换成字符串对应的类型
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static dynamic ValueToType(string val)
        {
            Regex floatNumRegex = new Regex(@"^(-?\d+)\.(\d+)?$"); //匹配浮点数
            Regex intNumRegex = new Regex(@"^(-?\d+)?$");    //匹配整数
            Regex dateTimeRegex = new Regex(@"^(?:\d{4}(-|/)\d{1,2}(-|/)\d{1,2})?(?:(\s|T))?(?:\d{1,2}:\d{1,2}(?::\d{1,2})?)?$");

            try
            {
                if (string.IsNullOrEmpty(val)) return string.Empty;
                if (floatNumRegex.IsMatch(val)) return double.Parse(val);
                if (intNumRegex.IsMatch(val)) return int.Parse(val);
                if (dateTimeRegex.IsMatch(val)) return DateTime.Parse(val);
                if (val.ToLower() == "false" ||
                    val.ToLower() == "true") return bool.Parse(val);

            }
            catch (SystemException ex)
            {
                MessageBox.Show("检测到值转换错误:" + ex.Message);
            }
            return val;
        }
 

        #endregion

        #region 窗口初始化

        public static FloatingView FloatingView { get; set; } = new();
        public static TaskMenoView TaskMenoView { get; set; } = new();
        public static FloatingTitleStyleView FloatingTitleStyleView { get; set; } = new();
        public static CustomSetView CustomSetView { get; set; } = new();
        public static EditFullScreenView EditFullScreenView { get; set; } = new();


        #endregion

        #region 窗口显示状态管理


        #region Floating

        public static void FloatingViewShow(string imagePath = "")
        {

            if (FloatingView.IsClosed)
            {
                FloatingView = new FloatingView();
            }
            if (!string.IsNullOrEmpty(imagePath))
            {
                FloatingView.FloatingBgImage.Source = new BitmapImage(new Uri(imagePath));
            }
            FloatingView.Show();
        }

        public static void FloatingViewHide()
        {
            if (FloatingView.IsClosed)
            {
                FloatingView = new FloatingView();
            }
            FloatingView.Hide();
        }

        public static void FloatingViewClose()
        {
            if (!FloatingView.IsClosed)
            {
                FloatingView.Close();
            }
        }

        #endregion

        #region FloatingTitleStyle

        public static void FloatingTitleStyleViewShow()
        {
            if (FloatingTitleStyleView.IsClosed)
            {
                FloatingTitleStyleView = new FloatingTitleStyleView();
            }
            FloatingTitleStyleView.Show();
        }

        public static void FloatingTitleStyleViewHide()
        {
            if (FloatingTitleStyleView.IsClosed)
            {
                FloatingTitleStyleView = new FloatingTitleStyleView();
            }
            FloatingTitleStyleView.Hide();
        }

        public static void FloatingTitleStyleViewClose()
        {
            if (!FloatingTitleStyleView.IsClosed)
            {
                FloatingTitleStyleView.Close();
            }
        }
        #endregion
        #region TaskMeno

        public static void TaskMenoViewShow()
        {
            if (TaskMenoView.IsClosed)
            {
                TaskMenoView = new TaskMenoView();
            }
            TaskMenoView.Show();
        }
        public static void TaskMenoViewHide()
        {
            if (TaskMenoView.IsClosed)
            {
                TaskMenoView = new TaskMenoView();
            }
            TaskMenoView.Hide();
        }

        public static void TaskMenoViewClose()
        {
            if (!TaskMenoView.IsClosed)
            {
                TaskMenoView.Close();
            }
        }

        #endregion

        #region CustomSet

        public static void CustomSetViewShow()
        {
            if (CustomSetView.IsClosed)
            {
                CustomSetView = new CustomSetView();
            }
            CustomSetView.Show();
        }

        public static void CustomSetViewHide()
        {
            if (CustomSetView.IsClosed)
            {
                CustomSetView = new CustomSetView();
            }
            CustomSetView.Hide();
        }

        public static void CustomSetViewClose()
        {
            if (!CustomSetView.IsClosed)
            {
                CustomSetView.Close();
            }
        }

        #endregion

        #region EditFullScreenView

        public static void EditFullScreenViewShow()
        {
            if (EditFullScreenView.IsClosed)
            {
                EditFullScreenView = new EditFullScreenView();
            }
            EditFullScreenView.Show();
        }

        public static void EditFullScreenViewHide()
        {
            if (EditFullScreenView.IsClosed)
            {
                EditFullScreenView = new EditFullScreenView();
            }
            EditFullScreenView.Hide();
        }

        public static void EditFullScreenViewClose()
        {
            if (!EditFullScreenView.IsClosed)
            {
                EditFullScreenView.Close();
            }
        }

        #endregion

        #endregion

    }
}
