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
        #region �������л�ȡ
        public static string LocalKey => GetConfigValue(nameof(LocalKey)) ?? "";

        /// <summary>
        /// ������ͼƬ·��
        /// </summary>
        public static string FloatingBgPath => GetConfigValue(nameof(FloatingBgPath)) ?? "";

        /// <summary>
        /// ������ͼƬ�Զ���С
        /// </summary>
        public static bool AutoSizeImage => ValueToType(GetConfigValue(nameof(AutoSizeImage)) ?? "false");
        /// <summary>
        /// WCF����ͬ�������Ƿ�����
        /// </summary>
        public static bool CanSyncEnable => ValueToType(GetConfigValue(nameof(CanSyncEnable)) ?? "false");

        public static string RecordVersion => GetConfigValue(nameof(RecordVersion)) ?? "";

        public static string Version => GetConfigValue(nameof(Version)) ?? "";
        /// <summary>
        /// ����ļ��б���·��
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
        /// ���������
        /// </summary>
        public static double FloatingSetWidth => ValueToType(GetConfigValue(nameof(FloatingSetWidth)) ?? "0");

        /// <summary>
        /// �������߶�
        /// </summary>
        public static double FloatingSetHeight => ValueToType(GetConfigValue(nameof(FloatingSetHeight)) ?? "0");
        /// <summary>
        /// �ļ���׺��
        /// </summary>
        public static string EndFileFormat => GetConfigValue(nameof(EndFileFormat)) ?? "";

        /// <summary>
        /// �ڶ����ձ����ɽ���ʱ��
        /// </summary>
        public static string DailyTaskEndTime => GetConfigValue(nameof(DailyTaskEndTime)) ?? "11:30";

        /// <summary>
        /// �Ƿ��Զ��������ռƻ�
        /// </summary>
        public static bool IsCreateTomorrowPlan => ValueToType(GetConfigValue(nameof(IsCreateTomorrowPlan)) ?? "true");

        /// <summary>
        /// �Ƿ�������������ʽ
        /// </summary>
        public static FloatingStyleEnum FloatingStyle => (FloatingStyleEnum)ValueToType(GetConfigValue(nameof(FloatingStyle)) ?? "1");

        /// <summary>
        /// �Զ�ɾ��ʱ��
        /// </summary>
        public static int DeleteTimes => ValueToType(GetConfigValue(nameof(DeleteTimes)) ?? "99");
        public static bool FloatingStatusIsFixed => ValueToType(GetConfigValue(nameof(FloatingStatusIsFixed)) ?? "false");

        public static bool IsEnableAutoDelete => ValueToType(GetConfigValue(nameof(IsEnableAutoDelete)) ?? "false");

        public static int LoadTaskFilePageSize => ValueToType(GetConfigValue(nameof(LoadTaskFilePageSize)) ?? "20");

        #endregion

        #region ��Ӵ�������
        /// <summary>
        /// �����ļ�����·��
        /// </summary>
        public static string TaskFilePath => Path.Combine(TaskTipPath, nameof(TaskFilePath));

        /// <summary>
        /// �����ļ�����·��
        /// </summary>
        public static string MenoFilePath => Path.Combine(TaskTipPath, nameof(MenoFilePath));

        /// <summary>
        /// ��¼�ļ�·��
        /// </summary>
        public static string RecordFilePath => Path.Combine(TaskTipPath, nameof(RecordFilePath));

        /// <summary>
        /// Ŀ¼�������ļ�
        /// </summary>
        public static string MenuTreeConfigPath => Path.Combine(TaskTipPath, "MenuTreeConfig.json");

        public static string FictionProgressPath => Path.Combine(TaskTipPath, "Fictions", "FictionProgress.json");

        public static string FictionCachePath => Path.Combine(TaskTipPath, "Fictions");
        public static string? WorkTimeRecordPath = Path.Combine(TaskTipPath, "WorkTime.json");
        public static string[] ThemeStyleKeys => (string[])Application.Current.Resources["ThemeColorArray"];
        public static string RecordMaxVersion => "HTML";
        public static string NewVersion => "1.0.20250105";
        public static string CustomThemePath { get; set; }

        public static string OperationRecordPath { get; set; }


        #endregion

        #region ����
        public static Configuration Configurations =
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public static ConfigurationHelper JsonConfiguration = new();

        public static Logger LogHelper = NLog.LogManager.GetCurrentClassLogger();



        #endregion

        #region ���ò���

        public static string? GetConfigValue(string key)
        {
            //return ConfigurationManager.AppSettings[key];
            var db = new SQLiteDB();
            return db.GetSysParamByKey(key);
        }


        /// <summary>
        /// ˢ������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="variableName">��</param>
        /// <param name="variable">ֵ</param>
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
                MessageBox.Show(variableName + "\t��ֵ" + variable?.ToString() + "��ƥ��");
            }
        }


        /// <summary>
        /// ˢ������
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
                MessageBox.Show($"����ˢ�����ô���:{e.Message}");
            }
        }
        

        /// <summary>
        /// ��string����ת�����ַ�����Ӧ������
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static dynamic ValueToType(string val)
        {
            Regex floatNumRegex = new Regex(@"^(-?\d+)\.(\d+)?$"); //ƥ�両����
            Regex intNumRegex = new Regex(@"^(-?\d+)?$");    //ƥ������
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
                MessageBox.Show("��⵽ֵת������:" + ex.Message);
            }
            return val;
        }


        #endregion
    }
}
