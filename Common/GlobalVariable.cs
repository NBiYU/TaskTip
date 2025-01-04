using CommunityToolkit.Mvvm.Messaging;

using NLog;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

using TaskTip.Common;
using TaskTip.Common.Helpers;
using TaskTip.Models.Enums;

using ConfigurationManager = System.Configuration.ConfigurationManager;
using MessageBox = System.Windows.MessageBox;

namespace TaskTip.Services
{
    public static class GlobalVariable
    {
        #region 从配置中获取
        public static string LocalKey => GetConfigValue(nameof(LocalKey)) ?? "";

        /// <summary>
        /// 悬浮窗图片路径
        /// </summary>
        public static string FloatingBgPath => GetConfigValue(nameof(FloatingBgPath)) ?? "";

        /// <summary>
        /// 悬浮窗图片自动大小
        /// </summary>
        public static bool AutoSizeImage => ValueToType(GetConfigValue(nameof(AutoSizeImage)) ?? "false");
        /// <summary>
        /// WCF数据同步服务是否启用
        /// </summary>
        public static bool CanSyncEnable => ValueToType(GetConfigValue(nameof(CanSyncEnable)) ?? "false");

        public static string RecordVersion => GetConfigValue(nameof(RecordVersion)) ?? "";

        /// <summary>
        /// 软件文件夹保存路径
        /// </summary>
        public static string TaskTipPath
        {
            get
            {
                var processPathInfo = Process.GetCurrentProcess().MainModule;
                var runtimePath = processPathInfo.FileName.Split(processPathInfo.ModuleName)[0];
                var cachePath = GetConfigValue(nameof(TaskTipPath));
                return string.IsNullOrEmpty(cachePath) ? runtimePath : cachePath!;
            }
        }
        public static string WorkFinishTime => GetConfigValue(nameof(WorkFinishTime)) ?? "";
        public static double SiestaTime => double.Parse(GetConfigValue(nameof(SiestaTime)) ?? "0.0");
        public static double AgainWorkGapTime => double.Parse(GetConfigValue(nameof(AgainWorkGapTime)) ?? "0");
        internal static string WorkStartTime => GetConfigValue(nameof(WorkStartTime)) ?? "";
        /// <summary>
        /// 悬浮窗宽度
        /// </summary>
        public static double FloatingSetWidth => ValueToType(GetConfigValue(nameof(FloatingSetWidth)) ?? "0");

        /// <summary>
        /// 悬浮窗高度
        /// </summary>
        public static double FloatingSetHeight => ValueToType(GetConfigValue(nameof(FloatingSetHeight)) ?? "0");
        /// <summary>
        /// 文件后缀名
        /// </summary>
        public static string EndFileFormat => GetConfigValue(nameof(EndFileFormat)) ?? "";

        /// <summary>
        /// 第二天日报生成结束时间
        /// </summary>
        public static string DailyTaskEndTime => GetConfigValue(nameof(DailyTaskEndTime)) ?? "11:30";

        /// <summary>
        /// 是否自动创建明日计划
        /// </summary>
        public static bool IsCreateTomorrowPlan => ValueToType(GetConfigValue(nameof(IsCreateTomorrowPlan)) ?? "true");

        /// <summary>
        /// 是否启用悬浮窗样式
        /// </summary>
        public static FloatingStyleEnum FloatingStyle => (FloatingStyleEnum)ValueToType(GetConfigValue(nameof(FloatingStyle)) ?? "1");

        /// <summary>
        /// 自动删除时间
        /// </summary>
        public static int DeleteTimes => ValueToType(GetConfigValue(nameof(DeleteTimes)) ?? "99");
        public static bool FloatingStatusIsFixed => ValueToType(GetConfigValue(nameof(FloatingStatusIsFixed)) ?? "false");

        public static bool IsEnableAutoDelete => ValueToType(GetConfigValue(nameof(IsEnableAutoDelete)) ?? "false");

        public static int LoadTaskFilePageSize => ValueToType(GetConfigValue(nameof(LoadTaskFilePageSize)) ?? "20");

        #endregion

        #region 间接处理属性
        /// <summary>
        /// 任务文件保存路径
        /// </summary>
        public static string TaskFilePath => Path.Combine(TaskTipPath, nameof(TaskFilePath));

        /// <summary>
        /// 记事文件保存路径
        /// </summary>
        public static string MenoFilePath => Path.Combine(TaskTipPath, nameof(MenoFilePath));

        /// <summary>
        /// 记录文件路径
        /// </summary>
        public static string RecordFilePath => Path.Combine(TaskTipPath, nameof(RecordFilePath));

        /// <summary>
        /// 目录树配置文件
        /// </summary>
        public static string MenuTreeConfigPath => Path.Combine(TaskTipPath, "MenuTreeConfig.json");

        public static string FictionProgressPath => Path.Combine(TaskTipPath, "Fictions", "FictionProgress.json");

        public static string FictionCachePath => Path.Combine(TaskTipPath, "Fictions");
        public static string? WorkTimeRecordPath = Path.Combine(TaskTipPath, "WorkTime.json");
        public static string[] ThemeStyleKeys => (string[])Application.Current.Resources["ThemeColorArray"];
        public static string RecordMaxVersion => "HTML";

        public static string CustomThemePath { get; set; }

        public static string OperationRecordPath { get; set; }


        #endregion

        #region 工具
        public static Configuration Configurations =
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public static ConfigurationHelper JsonConfiguration = new();

        public static Logger LogHelper = NLog.LogManager.GetCurrentClassLogger();



        #endregion

        #region 配置操作

        public static string? GetConfigValue(string key)
        {
            //return ConfigurationManager.AppSettings[key];
            var db = new SQLiteDB();
            return db.GetSysParamByKey(key);
        }


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
                var db = new SQLiteDB();
                db.UpdateSysParam(variableName, variable);
                //Configurations.AppSettings.Settings[variableName].Value = variable?.ToString();
                //Configurations.Save();
                //ConfigurationManager.RefreshSection("appSettings");
            }
            catch(Exception ex)
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
                //foreach (var v in val)
                //{
                //    Configurations.AppSettings.Settings[v.Key].Value = v.Value;
                //}

                //Configurations.Save();
                //ConfigurationManager.RefreshSection("appSettings");
                var db = new SQLiteDB();
                db.UpdateSysParams(val);

                WeakReferenceMessenger.Default.Send(string.Empty, Const.CONST_CONFIG_CHANGED);
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
    }
}
