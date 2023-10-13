using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TaskTip.Services;
using TaskTip.Views.UserControls;
using TaskTip.Views.Windows.PopWindow;
using Application = System.Windows.Application;
using File = System.IO.File;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace TaskTip.ViewModels.WindowModel
{
    internal partial class CustomSetViewModel : ObservableObject
    {

        private bool isClose = false;

        private Dictionary<string, string> _hotKeyRelevancyName = new();

        #region 属性

        private bool _autoStartUp;
        /// <summary>
        /// 是否开机自启
        /// </summary>
        public bool AutoStartUp
        {
            get => _autoStartUp;
            set
            {
                SetProperty(ref _autoStartUp, value);
                ChangedValueSave(nameof(AutoStartUp), AutoStartUp);
            }
        }
        private bool _isFloatingImageStyle;
        #region 悬浮窗设置属性
        public bool IsFloatingImageStyle
        {
            get => _isFloatingImageStyle;
            set
            {
                SetProperty(ref _isFloatingImageStyle, value);
                ChangedValueSave(nameof(IsFloatingImageStyle), IsFloatingImageStyle);
                IsFloatingStyleChanged();
                AutoSizeImage = true;
            }
        }

        private bool _autoSizeImage;
        /// <summary>
        /// 是否自动大小
        /// </summary>
        public bool AutoSizeImage
        {
            get => _autoSizeImage;
            set
            {
                SetProperty(ref _autoSizeImage, value);
                ChangedValueSave(nameof(AutoSizeImage), _autoSizeImage);
                FloatingViewState();
            }
        }

        private double _floatingSetWidth;
        /// <summary>
        /// 设置悬浮窗宽
        /// </summary>
        public double FloatingSetWidth
        {
            get => _floatingSetWidth;
            set
            {
                SetProperty(ref _floatingSetWidth, value);
                WeakReferenceMessenger.Default.Send($"{FloatingSetWidth}:{FloatingSetHeight}", Const.CONST_FLAOTING_SIZE_CHANGED);
                ChangedValueSave(nameof(FloatingSetWidth), _floatingSetWidth);
            }
        }


        private double _floatingSetHeight;
        /// <summary>
        /// 设置悬浮窗高
        /// </summary>
        public double FloatingSetHeight
        {
            get => _floatingSetHeight;
            set
            {
                SetProperty(ref _floatingSetHeight, value);
                WeakReferenceMessenger.Default.Send($"{FloatingSetWidth}:{FloatingSetHeight}", Const.CONST_FLAOTING_SIZE_CHANGED);
                ChangedValueSave(nameof(FloatingSetHeight), _floatingSetHeight);
            }
        }


        private double floatingMaxWidth;
        /// <summary>
        /// 宽最大设置范围
        /// </summary>
        public double FloatingMaxWidth
        {
            get => floatingMaxWidth;
            set
            {
                SetProperty(ref floatingMaxWidth, value);
            }
        }

        private double _floatingMaxHeight;
        /// <summary>
        /// 高最大设置范围
        /// </summary>
        public double FloatingMaxHeight
        {
            get => _floatingMaxHeight;
            set
            {
                SetProperty(ref _floatingMaxHeight, value);
            }
        }


        private Visibility _floatingSetVisibility;
        /// <summary>
        /// 悬浮窗是否显示
        /// </summary>
        public Visibility FloatingSetVisibility
        {
            get => _floatingSetVisibility;
            set => SetProperty(ref _floatingSetVisibility, value);
        }


        private string _floatingBgPath;
        /// <summary>
        /// 更改悬浮窗图片
        /// </summary>
        public string FloatingBgPath
        {
            get
            {
                if (File.Exists(_floatingBgPath))
                    return _floatingBgPath;
                else
                    return "";
            }
            set
            {
                if (value == null) return;
                SetProperty(ref _floatingBgPath, value);
                ChangedValueSave(nameof(FloatingBgPath), FloatingBgPath);
            }
        }

        #endregion


        #region 任务属性

        private string _taskTipPath;
        /// <summary>
        /// 任务保存路径
        /// </summary>
        public string TaskTipPath
        {
            get
            {
                if (Directory.Exists(_taskTipPath))
                    return _taskTipPath;
                else
                    return "";
            }
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                SetProperty(ref _taskTipPath, value);
                ChangedValueSave(nameof(TaskTipPath), TaskTipPath);
            }
        }

        private string _timeText;
        public string TimeText
        {
            get => _timeText;
            set
            {
                SetProperty(ref _timeText, value);
            }
        }


        private string _dailyTaskEndTime;
        public string DailyTaskEndTime
        {
            get => _dailyTaskEndTime;
            set
            {
                SetProperty(ref _dailyTaskEndTime, value);
                ChangedValueSave(nameof(DailyTaskEndTime), DailyTaskEndTime);
            }
        }


        private string _deleteTimes;
        public string DeleteTimes
        {
            get => _deleteTimes;
            set
            {
                SetProperty(ref _deleteTimes, value);
                ChangedValueSave(nameof(DeleteTimes), DeleteTimes);
            }
        }

        private bool _isCreateTomorrowPlan;
        public bool IsCreateTomorrowPlan
        {
            get => _isCreateTomorrowPlan;
            set
            {
                SetProperty(ref _isCreateTomorrowPlan, value);
                ChangedValueSave(nameof(IsCreateTomorrowPlan), IsCreateTomorrowPlan);
            }
        }

        private bool _isEnableAutoDelete;

        public bool IsEnableAutoDelete
        {
            get => _isEnableAutoDelete;
            set
            {
                SetProperty(ref _isEnableAutoDelete, value);
                ChangedValueSave(nameof(IsEnableAutoDelete), IsEnableAutoDelete);
            }
        }

        #endregion



        #endregion

        /// <summary>
        /// 已更改的参数缓存
        /// </summary>
        private Dictionary<string, string> ChangedValue = new Dictionary<string, string>();

        #region 控件指令处理函数

        /// <summary>
        /// 关闭界面指令处理函数，如果有已更改参数未保存，则回弹
        /// </summary>
        [RelayCommand]
        private void CloseView()
        {
            var errMsg = CheckConfig();
            if (!string.IsNullOrEmpty(errMsg))
            {
                MessageBox.Show(errMsg, "关闭失败");
                return;
            }

            if (ChangedValue.Count != 0)
            {
                isClose = true;
                if (MessageBox.Show($"您有未保存的内容：\n{JsonConvert.SerializeObject(ChangedValue,Formatting.Indented)}", "是否不保存更改直接退出",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;
                foreach (var val in ChangedValue)
                {
                    GetType().GetProperty(val.Key)?.SetValue(this, GlobalVariable.ValueToType(ConfigurationManager.AppSettings.Get(val.Key)));
                }
            }

            ChangedValue.Clear();


            if (GlobalVariable.IsFloatingImageStyle)
            {
                GlobalVariable.FloatingViewShow();
            }
            else
            {
                GlobalVariable.FloatingTitleStyleViewShow();
            }

            //GlobalVariable.CustomSetViewHide();
            GlobalVariable.CustomSetViewHide();


            isClose = false;
        }

        /// <summary>
        /// 设置界面最小化处理函数
        /// </summary>
        [RelayCommand]
        private void Mini()
        {
            GlobalVariable.CustomSetView.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 参数保存处理函数
        /// </summary>
        [RelayCommand]
        private void SaveData()
        {
            try
            {
                var errMsg = CheckConfig();
                if (!string.IsNullOrEmpty(errMsg))
                {
                    MessageBox.Show(errMsg, "保存失败");
                    return;
                }

                if (ChangedValue.Count != 0)
                {
                    if (ChangedValue.ContainsKey(nameof(TaskTipPath)) && GlobalVariable.TaskTipPath != ChangedValue[nameof(TaskTipPath)])
                    {
                        PathChanged();
                    }

                    GlobalVariable.SaveConfig(ChangedValue);
                    MessageBox.Show("设置已保存");

                    ChangedValue.Clear();
                    AutoStart(AutoStartUp);

                    WeakReferenceMessenger.Default.Send(GlobalVariable.TaskFilePath, Const.CONST_TASK_RELOAD);
                    WeakReferenceMessenger.Default.Send(GlobalVariable.MenoFilePath, Const.CONST_MENO_RELOAD);
                    WeakReferenceMessenger.Default.Send(GlobalVariable.MenuTreeConfigPath, Const.CONST_RECORD_RELOAD);
                }
                else
                {
                    MessageBox.Show("设置未更改，无需保存");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"保存失败，异常提示：{e}");
            }
        }

        #endregion

        #region 视图分类

        #region 任务



        /// <summary>
        /// 任务路径更改处理函数
        /// </summary>
        [RelayCommand]
        private void GetTaskTipPath()
        {
            var dirPath = GetDirPath();
            if (string.IsNullOrWhiteSpace(dirPath))
                return;

            TaskTipPath = dirPath;
        }


        /// <summary>
        /// 获取文件夹路径函数
        /// </summary>
        /// <returns></returns>
        private string? GetDirPath()
        {
            try
            {
                var dirName = new System.Windows.Forms.FolderBrowserDialog();
                dirName.ShowDialog();

                if (string.IsNullOrEmpty(dirName.SelectedPath))
                    return null;

                return dirName.SelectedPath;
            }
            catch
            {
                MessageBox.Show("文件夹获取失败");
            }
            return null;
        }


        private void IsFloatingStyleChanged()
        {
            if (IsFloatingImageStyle)
            {
                GlobalVariable.FloatingTitleStyleViewClose();
            }
            else
            {
                GlobalVariable.FloatingViewClose();
                GlobalVariable.TaskMenoViewClose();
            }

        }

        #endregion

        #region 悬浮窗

        [RelayCommand]
        private void GetImageFilePath()
        {
            var path = GetImagePathFormDir();

            if (string.IsNullOrEmpty(path)) return;

            FloatingBgPath = path;

            ImageSource igs = new BitmapImage(new Uri(path));
            GlobalVariable.FloatingView.FloatingBgImage.Source = igs;
        }

        /// <summary>
        /// 图片获取处理函数
        /// </summary>
        /// <returns></returns>
        private string? GetImagePathFormDir()
        {
            try
            {
                var fileName = new OpenFileDialog()
                {
                    Filter = "All Files|*.*"
                };
                fileName.ShowDialog();

                if (string.IsNullOrEmpty(fileName.FileName))
                    return null;

                return fileName.FileName;
            }
            catch
            {
                MessageBox.Show("文件未能正确加载");
                return null;
            }
        }

        /// <summary>
        /// 悬浮窗事件
        /// </summary>
        private void ShowCustom()
        {
            FloatingViewState();
        }
        /// <summary>
        /// 图片更改处理函数
        /// </summary>

        /// <summary>
        /// 预览自定义尺寸悬浮窗
        /// </summary>
        private void FloatingViewState()
        {
            if (AutoSizeImage)
            {
                FloatingSetVisibility = Visibility.Collapsed;
                GlobalVariable.FloatingViewHide();
            }
            else
            {
                GlobalVariable.FloatingViewShow(FloatingBgPath);
                FloatingSetVisibility = Visibility.Visible;
            }
           
        }


        #endregion



        #endregion

        #region 阅读小说热键

        private ObservableCollection<KeyInputUC> _keyInputUcs=new();

        public ObservableCollection<KeyInputUC> KeyInputUcs 
        {
            get => _keyInputUcs;
            set=> SetProperty(ref _keyInputUcs,value);
        }

        
        private List<KeyInputUC> AddKeyInputUc(params string[] ucNames)
        {
            var collection = new List<KeyInputUC>();
            var relevancyName = GlobalVariable.JsonConfiguration.TryGetValue<List<dynamic>>("HotKeys:ReadUIHotKeys");
            foreach (var name in ucNames)
            {
                var control = new KeyInputUC(name);
                control.InputKeysChanged += (o, e) =>
                {
                    if (o is KeyInputUC uc)
                    {
                        var hotKeys =
                            GlobalVariable.JsonConfiguration.TryGetValue<List<dynamic>>("HotKeys:ReadUIHotKeys");

                        var saveKey = string.Join("+", uc.InputKeys);

                        // 检查键是否冲突
                        var checkKeyExisted = hotKeys.FirstOrDefault(x => x.KeyASCII == saveKey);
                        if (checkKeyExisted != null && checkKeyExisted.HotKeyName.ToString() != _hotKeyRelevancyName[uc.HotKeyName.Text])
                        {
                            MessageBox.Show($"当前设置快捷键和”{_hotKeyRelevancyName.First(x=>x.Value == checkKeyExisted.HotKeyName.ToString()).Key}“快捷键冲突");
                            uc.InputKeys = new int[2];
                            return;
                        }

                        // 保存设置
                        var idx = hotKeys.FindIndex(x =>
                            x.HotKeyName == _hotKeyRelevancyName[uc.HotKeyName.Text]);
                        if (idx == -1)
                        {
                            MessageBox.Show($"未找到“{_hotKeyRelevancyName[uc.HotKeyName.Text]}”对应项");
                            return;
                        }
                        GlobalVariable.JsonConfiguration[$"HotKeys:ReadUIHotKeys:{idx}:KeyASCII", GetType()] = saveKey;
                        
                        // 如果界面已打开，通知重新注册
                        if (FictionReadView.HotKeyManagers.Count != 0)
                            WeakReferenceMessenger.Default.Send(string.Empty, Const.CONST_HOT_KEY_RE_REGISTER);
                    }
                };
                string hotKeys = relevancyName.First(x => x.HotKeyName == _hotKeyRelevancyName[name]).KeyASCII;
                control.InputKeys = Array.ConvertAll(hotKeys.ToString().Split("+"),int.Parse);
                collection.Add(control);
            }

            return collection;
        }

        #endregion

        #region 功能函数

        private void PathChanged()
        {
            var taskPath = Path.Combine(TaskTipPath , nameof(GlobalVariable.TaskFilePath));
            var menoPath = Path.Combine(TaskTipPath , nameof(GlobalVariable.MenoFilePath));
            var recordPath = Path.Combine(TaskTipPath, nameof(GlobalVariable.RecordFilePath));
            var fictionCachePath = Path.Combine(TaskTipPath ,"Fictions");
            var fictionSetPath = Path.Combine(fictionCachePath , "FictionProgress.json");
            var jsonPath = Path.Combine(TaskTipPath ,"MenuTreeConfig.json");

            if (!Directory.Exists(taskPath)) Directory.CreateDirectory(taskPath);
            if (!Directory.Exists(menoPath)) Directory.CreateDirectory(menoPath);
            if (!Directory.Exists(recordPath)) Directory.CreateDirectory(recordPath);
            if (!Directory.Exists(fictionCachePath)) Directory.CreateDirectory(fictionCachePath);

            DirMove(GlobalVariable.TaskFilePath, taskPath);
            DirMove(GlobalVariable.MenoFilePath, menoPath);
            DirMove(GlobalVariable.RecordFilePath, recordPath);
            DirMove(GlobalVariable.RecordFilePath, fictionCachePath);



            if (File.Exists(jsonPath))
            {
                if (MessageBox.Show("MenuTreeConfig.json文件已存在，是否覆盖", "文件移动", MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    File.Delete(jsonPath);
                    File.Move(GlobalVariable.MenuTreeConfigPath, jsonPath);
                }
            }
            if (File.Exists(fictionSetPath))
            {
                if (MessageBox.Show("FictionProgress.json文件已存在，是否覆盖", "文件移动", MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    File.Delete(jsonPath);
                    File.Move(GlobalVariable.MenuTreeConfigPath, jsonPath);
                }
            }
        }

        public string CheckConfig()
        {
            var isEmptyMsg = string.Empty;
            isEmptyMsg += string.IsNullOrWhiteSpace(FloatingBgPath) ? "悬浮窗背景不能为空\n" : "";
            isEmptyMsg += string.IsNullOrWhiteSpace(TaskTipPath) ? "保存路径不能为空\n" : "";
            isEmptyMsg += !Regex.IsMatch(DeleteTimes, @"^[0-9]+$") ? "计划删除天数格式异常" : "";
            isEmptyMsg += !Regex.IsMatch(DailyTaskEndTime, @"^\d{1,2}:\d{1,2}?$") ? "每日截时间格式异常" : "";


            return isEmptyMsg;
        }

        private void ConfigChangedHandler()
        {
            TaskTipPath = GlobalVariable.TaskTipPath;
        }
        /// <summary>
        /// 所有文件移动
        /// </summary>
        /// <param name="dirPath"></param>
        private void DirMove(string sPath, string dirPath)
        {
            if (!Directory.Exists(sPath))
            {
                throw new Exception($"源文件夹：{sPath}不存在");
            }
            if (!Directory.Exists(dirPath))
            {
                throw new Exception($"目标文件夹：{dirPath}不存在");
            }
            AllFileMove(sPath, dirPath);

        }
        private void AllFileMove(string sPath, string dirPath)
        {
            var allFile = Directory.GetFiles(sPath, "*.task");
            foreach (var file in allFile)
            {
                var sfileName = Path.GetFileName(file);
                var dfilePath = Path.Combine(dirPath, sfileName);
                File.Move(file, dfilePath);
            }
        }
        /// <summary>  
        /// 修改程序在注册表中的键值  
        /// </summary>  
        /// <param name="isAuto">true:开机启动,false:不开机自启</param> 
        public static void AutoStart(bool isAuto)
        {
            try
            {
                RegistryKey R_local = Registry.CurrentUser;
                RegistryKey R_run = R_local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");

                if (isAuto) R_run.SetValue("TaskTip", Application.Current);
                else R_run.DeleteValue("TaskTip", false);

                R_run.Close();
                R_local.Close();

                //GlobalVariant.Instance.UserConfig.AutoStart = isAuto;
            }
            catch (Exception e)
            {
                MessageBox.Show($"您需要管理员权限修改：{e}", "提示");
            }
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="varName"></param>
        /// <param name="value"></param>
        private void ChangedValueSave<T>(string varName, T val)
        {
            if (isClose)
                return;

            if (ConfigurationManager.AppSettings.Get(varName) == val.ToString())
                return;

            if (ChangedValue.ContainsKey(varName))
            {
                ChangedValue[varName] = val.ToString();
            }
            else
            {
                ChangedValue.Add(key: varName, value: val.ToString());
            }
        }


        #endregion

        #region 初始化

        private void InitRegister()
        {
            WeakReferenceMessenger.Default.Register<string, string>(this, Const.CONST_SHOW_CUSTOM, (obj, msg) => ShowCustom());
            WeakReferenceMessenger.Default.Register<string, string>(this, Const.CONST_CONFIG_CHANGED, (obj, msg) => ConfigChangedHandler());
        }

        private void InitHotKeyKeyValue()
        {
            _hotKeyRelevancyName.Add("最小化",Const.CONST_HOT_KEY_HIDE);
            _hotKeyRelevancyName.Add("显示", Const.CONST_HOT_KEY_SHOW);
            _hotKeyRelevancyName.Add("关闭", Const.CONST_HOT_KEY_CLOSE);
            _hotKeyRelevancyName.Add("上一页", Const.CONST_HOT_KEY_PREVIEW);
            _hotKeyRelevancyName.Add("下一页", Const.CONST_HOT_KEY_NEXT);
            _hotKeyRelevancyName.Add("自动阅读", Const.CONST_HOT_KEY_AUTO_READ);
        }

        private void InitProperty()
        {
            FloatingBgPath = GlobalVariable.FloatingBgPath;
            TaskTipPath = GlobalVariable.TaskTipPath;
            DeleteTimes = GlobalVariable.DeleteTimes.ToString();
            IsFloatingImageStyle = GlobalVariable.IsFloatingImageStyle;

            AutoSizeImage = GlobalVariable.AutoSizeImage;
            FloatingSetHeight = GlobalVariable.FloatingSetHeight;
            FloatingSetWidth = GlobalVariable.FloatingSetHeight;
            DailyTaskEndTime = GlobalVariable.DailyTaskEndTime;

            FloatingMaxHeight = SystemParameters.WorkArea.Height;
            FloatingMaxWidth = SystemParameters.WorkArea.Width;

            IsEnableAutoDelete = GlobalVariable.IsEnableAutoDelete;

            KeyInputUcs = new ObservableCollection<KeyInputUC>(AddKeyInputUc(_hotKeyRelevancyName.Keys.Select(x=>x).ToArray()));
            
        }

        public CustomSetViewModel()
        {
            InitHotKeyKeyValue();
            InitProperty();
            InitRegister();
        }

        #endregion


    }
}
