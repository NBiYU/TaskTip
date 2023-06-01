using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TaskTip.Services;
using TaskTip.Views;
using File = System.IO.File;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace TaskTip.ViewModels
{
    internal partial class CustomSetViewModel : ObservableObject
    {

        private bool isClose = false;
        private bool isError = false;

        #region 属性
        
        private bool _autoStartUp;
        /// <summary>
        /// 是否开机自启
        /// </summary>
        public bool AutoStartUp
        {
            get=>_autoStartUp;
            set
            {
                SetProperty(ref _autoStartUp, value);
                ChangedValueSave(nameof(AutoStartUp), AutoStartUp);
            }
        }


        private bool _isFloatingImageStyle;
        public bool IsFloatingImageStyle
        {
            get => _isFloatingImageStyle;
            set
            {
                SetProperty(ref _isFloatingImageStyle, value);
                ChangedValueSave(nameof(IsFloatingImageStyle), IsFloatingImageStyle);
                AutoSizeImage = true;
            }
        }


        #region 悬浮窗设置属性

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
                FloatingSetVisibility = _autoSizeImage ? Visibility.Collapsed : Visibility.Visible;
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
                FloatingSizeEvent?.Invoke($"{FloatingSetWidth}:{FloatingSetHeight}", null);
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
                FloatingSizeEvent?.Invoke($"{FloatingSetWidth}:{FloatingSetHeight}", null);
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

        private string _menoFilePath;
        public string MenoFilePath
        {
            get
            {
                if (Directory.Exists(_menoFilePath))
                    return _menoFilePath;
                else
                    return "";
            }
            set
            {
                SetProperty(ref _menoFilePath, value);
                ChangedValueSave(nameof(MenoFilePath), MenoFilePath);
            }
        }

        #endregion


        #region 任务属性

        private string _taskFilePath;
        /// <summary>
        /// 任务保存路径
        /// </summary>
        public string TaskFilePath
        {
            get
            {
                if (Directory.Exists(_taskFilePath))
                    return _taskFilePath;
                else
                    return "";
            } 
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                SetProperty(ref _taskFilePath, value);
                ChangedValueSave(nameof(TaskFilePath), TaskFilePath);
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

        #endregion



        #endregion

        /// <summary>
        /// 已更改的参数缓存
        /// </summary>
        private Dictionary<string, string> ChangedValue = new Dictionary<string, string>();

        #region 控件指令


        public RelayCommand CloseViewCommand { get; set; }
        public RelayCommand SaveDataCommand { get; set; }
        public RelayCommand MiniCommand { get; set; }
        public RelayCommand GetImageFilePathCommand { get; set; }
        public RelayCommand GetTaskFilePathCommand { get; set; }
        public RelayCommand GetMenoFilePathCommand { get; set; }

        #endregion

        #region 控件指令处理函数

        /// <summary>
        /// 关闭界面指令处理函数，如果有已更改参数未保存，则回弹
        /// </summary>
        private void CloseView()
        {
            var isEmptyMsg = string.Empty;
            isEmptyMsg += string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[nameof(FloatingBgPath)]) ? "悬浮窗背景不能为空\n" : "";
            isEmptyMsg += string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[nameof(TaskFilePath)]) ? "保存路径不能为空\n" : "";
            isEmptyMsg += string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[nameof(MenoFilePath)]) ? "记事路径不能为空\n" : "";
            isEmptyMsg += !Regex.IsMatch(DeleteTimes, @"^[0-9]+$") ? "计划删除天数格式异常" : "";
            isEmptyMsg += !Regex.IsMatch(DailyTaskEndTime, @"^\d{1,2}:\d{1,2}?$") ? "每日截时间格式异常" : "";

            if (!string.IsNullOrWhiteSpace(isEmptyMsg))
            {
                MessageBox.Show(isEmptyMsg.TrimEnd('\n'));
                return;
            }


            if (ChangedValue.Count != 0)
            {
                isClose = true;
                if (MessageBox.Show($"您有未保存的内容：\n{JsonConvert.SerializeObject(ChangedValue)}", "是否不保存更改直接退出",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;
                foreach (var val in ChangedValue)
                {
                    this.GetType().GetProperty(val.Key)?.SetValue(this, GlobalVariable.ValueToType(ConfigurationManager.AppSettings.Get(val.Key)));
                }
            }

            ChangedValue.Clear();

            GlobalVariable.CustomSetViewHide();

            if (GlobalVariable.IsFloatingImageStyle)
            {
                GlobalVariable.FloatingViewShow();
            }
            else
            {
                GlobalVariable.FloatingTitleStyleViewShow();
            }

            isClose = false;
        }

        /// <summary>
        /// 设置界面最小化处理函数
        /// </summary>
        private void MiniView()
        {
            GlobalVariable.CustomSetView.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 参数保存处理函数
        /// </summary>
        private void SaveData()
        {
            try
            {
                if (ChangedValue.Count != 0)
                {
                    if (ChangedValue.ContainsKey(nameof(TaskFilePath)))
                    {
                        DirMove(GlobalVariable.TaskFilePath, TaskFilePath);
                    }


                    if (ChangedValue.ContainsKey(nameof(MenoFilePath)))
                    {
                        DirMove(GlobalVariable.MenoFilePath, MenoFilePath);
                    }

                    GlobalVariable.SaveConfig(ChangedValue);
                    MessageBox.Show("设置已保存");
                    ChangedValue.Clear();
                    AutoStart(AutoStartUp);
                    TaskLoadChanged?.Invoke(TaskFilePath, EventArgs.Empty);
                    MenoLoadChanged?.Invoke(MenoFilePath, EventArgs.Empty);
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

        /// <summary>
        /// 任务路径更改处理函数
        /// </summary>
        private void GetTaskDirPath()
        {
            var dirPath = GetDirPath();
            if (string.IsNullOrWhiteSpace(dirPath))
                return;

            TaskFilePath = dirPath;
        }

        private void GetMenoDirPath()
        {
            var dirPath = GetDirPath();
            if (string.IsNullOrWhiteSpace(dirPath))
                return;

            MenoFilePath = dirPath;
        }


        /// <summary>
        /// 图片获取处理函数
        /// </summary>
        /// <returns></returns>
        private string GetImageFilePath()
        {
            try
            {
                var fileName = new Microsoft.Win32.OpenFileDialog()
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowCustom(object sender, EventArgs e)
        {
            FloatingViewState();
        }

        //private void DailyTaskEndTimeHandler(object sender, EventArgs e)
        //{
        //    ChangedValueSave(nameof(DailyTaskEndTime), DailyTaskEndTime);
        //}


        #endregion

        #region 功能函数

        private void ConfigChangedHandler(object sender,EventArgs e)
        {
            TaskFilePath = GlobalVariable.TaskFilePath;
            MenoFilePath = GlobalVariable.MenoFilePath;
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
            AllFileMove(sPath,dirPath);

        }
        private void AllFileMove(string sPath, string dirPath)
        {
            var allFile = Directory.GetFiles(sPath, "*.task");
            foreach (var file in allFile)
            {
                var sfileName = Path.GetFileName(file);
                var dfilePath = Path.Combine(dirPath, sfileName);
                File.Move(file,dfilePath);
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




        #endregion

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

        /// <summary>
        /// 图片更改处理函数
        /// </summary>
        private void FloatingImageChanged()
        {
            var path = GetImageFilePath();

            if (string.IsNullOrEmpty(path)) return;

            FloatingBgPath = path;

            ImageSource igs = new BitmapImage(new Uri(path));
            GlobalVariable.FloatingView.FloatingBgImage.Source = igs;
        }

        /// <summary>
        /// 获取文件夹路径函数
        /// </summary>
        /// <returns></returns>
        private string GetDirPath()
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


        /// <summary>
        /// 通知外部图片路径更换
        /// </summary>
#pragma warning disable CS0067 // 从不使用事件“CustomSetViewModel.ImagePathCahnged”
        public static event EventHandler ImagePathChanged;
#pragma warning restore CS0067 // 从不使用事件“CustomSetViewModel.ImagePathCahnged”
        public static event EventHandler TaskLoadChanged;
        public static event EventHandler MenoLoadChanged;


        /// <summary>
        /// 预览自定义尺寸悬浮窗
        /// </summary>
        private void FloatingViewState()
        {
            if (AutoSizeImage)
                GlobalVariable.FloatingViewHide();
            else
                GlobalVariable.FloatingViewShow();
        }

        /// <summary>
        /// 改变悬浮窗尺寸委托
        /// </summary>
        public static EventHandler FloatingSizeEvent;

        public CustomSetViewModel()
        {
            CloseViewCommand = new RelayCommand(CloseView);
            MiniCommand = new RelayCommand(MiniView);
            SaveDataCommand = new RelayCommand(SaveData);
            GetImageFilePathCommand = new RelayCommand(FloatingImageChanged);
            GetTaskFilePathCommand = new RelayCommand(GetTaskDirPath);
            GetMenoFilePathCommand = new RelayCommand(GetMenoDirPath);

            FloatingBgPath = GlobalVariable.FloatingBgPath;
            TaskFilePath = GlobalVariable.TaskFilePath;
            MenoFilePath = GlobalVariable.MenoFilePath;
            DeleteTimes = GlobalVariable.DeleteTimes.ToString();
            IsFloatingImageStyle = GlobalVariable.IsFloatingImageStyle;

            AutoSizeImage = GlobalVariable.AutoSizeImage;
            FloatingSetHeight = GlobalVariable.FloatingSetHeight;
            FloatingSetWidth = GlobalVariable.FloatingSetHeight;
            DailyTaskEndTime = GlobalVariable.DailyTaskEndTime;

            FloatingMaxHeight = SystemParameters.WorkArea.Height;
            floatingMaxWidth = SystemParameters.WorkArea.Width;

            CustomSetView.CustomWindowStateChanged += ShowCustom;
            GlobalVariable.ConfigChanged += ConfigChangedHandler;
        }
    }
}
