using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using TaskTip.Views;
using MessageBox = System.Windows.MessageBox;

namespace TaskTip.Services
{
    public static class GlobalVariable
    {

        #region 共享属性
        /// <summary>
        /// 悬浮窗图片路径
        /// </summary>
        public static string FloatingBgPath
        {
            get => ConfigurationManager.AppSettings[nameof(FloatingBgPath)] ?? "";
        }
        /// <summary>
        /// 悬浮窗图片自动大小
        /// </summary>
        public static bool AutoSizeImage
        {
            get => ValueToType(ConfigurationManager.AppSettings[nameof(AutoSizeImage)] ?? "false");
        }
        /// <summary>
        /// 任务文件保存路径
        /// </summary>
        public static string TaskFilePath
        {
            get => string.IsNullOrEmpty(ConfigurationManager.AppSettings[nameof(TaskFilePath)])
                ? Process.GetCurrentProcess().MainModule?.FileName.Replace("TaskTip.exe", "TaskFile")!
                : ConfigurationManager.AppSettings[nameof(TaskFilePath)]!;
        }
        /// <summary>
        /// 记事文件保存路径
        /// </summary>
        public static string MenoFilePath
        {
            get => string.IsNullOrEmpty(ConfigurationManager.AppSettings[nameof(MenoFilePath)])
                ? Process.GetCurrentProcess().MainModule?.FileName.Replace("TaskTip.exe", "MenoFile")!
                : ConfigurationManager.AppSettings[nameof(MenoFilePath)]!;
        }
        /// <summary>
        /// 悬浮窗宽度
        /// </summary>
        public static double FloatingSetWidth
        {
            get =>  ValueToType(ConfigurationManager.AppSettings[nameof(FloatingSetWidth)] ?? "0");
        }
        /// <summary>
        /// 悬浮窗高度
        /// </summary>
        public static double FloatingSetHeight
        {
            get => ValueToType(ConfigurationManager.AppSettings[nameof(FloatingSetHeight)] ?? "0");
        }
        /// <summary>
        /// 文件后缀名
        /// </summary>
        public static string EndFileFormat
        {
            get => ConfigurationManager.AppSettings[nameof(EndFileFormat)] ?? "";
        }
        /// <summary>
        /// 第二天日报生成结束时间
        /// </summary>
        public static string DailyTaskEndTime
        {
            get => ConfigurationManager.AppSettings[nameof(DailyTaskEndTime)] ?? "11:30";
        }
        /// <summary>
        /// 是否自动创建明日计划
        /// </summary>
        public static bool IsCreateTomorrowPlan
        {
            get => ValueToType(ConfigurationManager.AppSettings[nameof(IsCreateTomorrowPlan)] ?? "true");
        }
        /// <summary>
        /// 是否启用悬浮窗样式
        /// </summary>
        public static bool IsFloatingImageStyle
        {
            get => ValueToType(ConfigurationManager.AppSettings[nameof(IsFloatingImageStyle)] ?? "true");
        }
        /// <summary>
        /// 自动删除时间
        /// </summary>
        public static int DeleteTimes
        {
            get => ValueToType(ConfigurationManager.AppSettings[nameof(DeleteTimes)] ?? "99");
        }

        public static Configuration Configurations =
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        
        #endregion

        /// <summary>
        /// 通知外部配置更改
        /// </summary>
        public static event EventHandler ConfigChanged;

        /// <summary>
        /// 通知外部矩形形状更改
        /// </summary>
        public static event EventHandler RectangleChanged;

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

                ConfigChanged?.Invoke(null, null);
            }
            catch
            {
                MessageBox.Show("数据刷入配置错误");
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

        #region 窗口初始化
        public static FloatingView FloatingView { get; set; } = new FloatingView();
        public static FloatingTitleView FloatingTitleView { get; set; } = new FloatingTitleView();
        public static TaskMenoView TaskMenoView { get; set; } = new TaskMenoView();
        public static CustomSetView CustomSetView { get; set; } = new CustomSetView();
        #endregion

        #region 窗口显示状态管理

        public static void FloatingViewShow()
        {
            if (FloatingView == null)
            {
                FloatingView = new FloatingView();
            }
            FloatingView.Show();
        }
        public static void FloatingTitleViewShow()
        {
            if (FloatingTitleView == null)
            {
                FloatingTitleView = new FloatingTitleView();
            }
            FloatingTitleView.Show();
        }
        public static void TaskMenoViewShow()
        {
            if (TaskMenoView == null)
            {
                TaskMenoView = new TaskMenoView();
            }
            TaskMenoView.Show();
        }
        public static void CustomSetViewShow()
        {
            if (CustomSetView == null)
            {
                CustomSetView = new CustomSetView();
            }
            CustomSetView.Show();
        }

        public static void FloatingViewHide()
        {
            if (FloatingView == null)
            {
                FloatingView = new FloatingView();
            }
            FloatingView.Hide();
        }
        public static void FloatingTitleViewHide()
        {
            if (FloatingTitleView == null)
            {
                FloatingTitleView = new FloatingTitleView();
            }
            FloatingTitleView.Hide();
        }
        public static void TaskMenoViewHide()
        {
            if (TaskMenoView == null)
            {
                TaskMenoView = new TaskMenoView();
            }
            TaskMenoView.Hide();
        }
        public static void CustomSetViewHide()
        {
            if (CustomSetView == null)
            {
                CustomSetView = new CustomSetView();
            }
            CustomSetView.Hide();
        }


        #endregion
    }
}
