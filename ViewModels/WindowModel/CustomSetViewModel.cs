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

        #region ����

        private bool _autoStartUp;
        /// <summary>
        /// �Ƿ񿪻�����
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
        #region ��������������
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
        /// �Ƿ��Զ���С
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
        /// ������������
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
        /// ������������
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
        /// ��������÷�Χ
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
        /// ��������÷�Χ
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
        /// �������Ƿ���ʾ
        /// </summary>
        public Visibility FloatingSetVisibility
        {
            get => _floatingSetVisibility;
            set => SetProperty(ref _floatingSetVisibility, value);
        }


        private string _floatingBgPath;
        /// <summary>
        /// ����������ͼƬ
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


        #region ��������

        private string _taskTipPath;
        /// <summary>
        /// ���񱣴�·��
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
        /// �Ѹ��ĵĲ�������
        /// </summary>
        private Dictionary<string, string> ChangedValue = new Dictionary<string, string>();

        #region �ؼ�ָ�����

        /// <summary>
        /// �رս���ָ�������������Ѹ��Ĳ���δ���棬��ص�
        /// </summary>
        [RelayCommand]
        private void CloseView()
        {
            var errMsg = CheckConfig();
            if (!string.IsNullOrEmpty(errMsg))
            {
                MessageBox.Show(errMsg, "�ر�ʧ��");
                return;
            }

            if (ChangedValue.Count != 0)
            {
                isClose = true;
                if (MessageBox.Show($"����δ��������ݣ�\n{JsonConvert.SerializeObject(ChangedValue,Formatting.Indented)}", "�Ƿ񲻱������ֱ���˳�",
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
        /// ���ý�����С��������
        /// </summary>
        [RelayCommand]
        private void Mini()
        {
            GlobalVariable.CustomSetView.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// �������洦����
        /// </summary>
        [RelayCommand]
        private void SaveData()
        {
            try
            {
                var errMsg = CheckConfig();
                if (!string.IsNullOrEmpty(errMsg))
                {
                    MessageBox.Show(errMsg, "����ʧ��");
                    return;
                }

                if (ChangedValue.Count != 0)
                {
                    if (ChangedValue.ContainsKey(nameof(TaskTipPath)) && GlobalVariable.TaskTipPath != ChangedValue[nameof(TaskTipPath)])
                    {
                        PathChanged();
                    }

                    GlobalVariable.SaveConfig(ChangedValue);
                    MessageBox.Show("�����ѱ���");

                    ChangedValue.Clear();
                    AutoStart(AutoStartUp);

                    WeakReferenceMessenger.Default.Send(GlobalVariable.TaskFilePath, Const.CONST_TASK_RELOAD);
                    WeakReferenceMessenger.Default.Send(GlobalVariable.MenoFilePath, Const.CONST_MENO_RELOAD);
                    WeakReferenceMessenger.Default.Send(GlobalVariable.MenuTreeConfigPath, Const.CONST_RECORD_RELOAD);
                }
                else
                {
                    MessageBox.Show("����δ���ģ����豣��");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"����ʧ�ܣ��쳣��ʾ��{e}");
            }
        }

        #endregion

        #region ��ͼ����

        #region ����



        /// <summary>
        /// ����·�����Ĵ�����
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
        /// ��ȡ�ļ���·������
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
                MessageBox.Show("�ļ��л�ȡʧ��");
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

        #region ������

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
        /// ͼƬ��ȡ������
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
                MessageBox.Show("�ļ�δ����ȷ����");
                return null;
            }
        }

        /// <summary>
        /// �������¼�
        /// </summary>
        private void ShowCustom()
        {
            FloatingViewState();
        }
        /// <summary>
        /// ͼƬ���Ĵ�����
        /// </summary>

        /// <summary>
        /// Ԥ���Զ���ߴ�������
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

        #region �Ķ�С˵�ȼ�

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

                        // �����Ƿ��ͻ
                        var checkKeyExisted = hotKeys.FirstOrDefault(x => x.KeyASCII == saveKey);
                        if (checkKeyExisted != null && checkKeyExisted.HotKeyName.ToString() != _hotKeyRelevancyName[uc.HotKeyName.Text])
                        {
                            MessageBox.Show($"��ǰ���ÿ�ݼ��͡�{_hotKeyRelevancyName.First(x=>x.Value == checkKeyExisted.HotKeyName.ToString()).Key}����ݼ���ͻ");
                            uc.InputKeys = new int[2];
                            return;
                        }

                        // ��������
                        var idx = hotKeys.FindIndex(x =>
                            x.HotKeyName == _hotKeyRelevancyName[uc.HotKeyName.Text]);
                        if (idx == -1)
                        {
                            MessageBox.Show($"δ�ҵ���{_hotKeyRelevancyName[uc.HotKeyName.Text]}����Ӧ��");
                            return;
                        }
                        GlobalVariable.JsonConfiguration[$"HotKeys:ReadUIHotKeys:{idx}:KeyASCII", GetType()] = saveKey;
                        
                        // ��������Ѵ򿪣�֪ͨ����ע��
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

        #region ���ܺ���

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
                if (MessageBox.Show("MenuTreeConfig.json�ļ��Ѵ��ڣ��Ƿ񸲸�", "�ļ��ƶ�", MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    File.Delete(jsonPath);
                    File.Move(GlobalVariable.MenuTreeConfigPath, jsonPath);
                }
            }
            if (File.Exists(fictionSetPath))
            {
                if (MessageBox.Show("FictionProgress.json�ļ��Ѵ��ڣ��Ƿ񸲸�", "�ļ��ƶ�", MessageBoxButton.YesNo,
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
            isEmptyMsg += string.IsNullOrWhiteSpace(FloatingBgPath) ? "��������������Ϊ��\n" : "";
            isEmptyMsg += string.IsNullOrWhiteSpace(TaskTipPath) ? "����·������Ϊ��\n" : "";
            isEmptyMsg += !Regex.IsMatch(DeleteTimes, @"^[0-9]+$") ? "�ƻ�ɾ��������ʽ�쳣" : "";
            isEmptyMsg += !Regex.IsMatch(DailyTaskEndTime, @"^\d{1,2}:\d{1,2}?$") ? "ÿ�ս�ʱ���ʽ�쳣" : "";


            return isEmptyMsg;
        }

        private void ConfigChangedHandler()
        {
            TaskTipPath = GlobalVariable.TaskTipPath;
        }
        /// <summary>
        /// �����ļ��ƶ�
        /// </summary>
        /// <param name="dirPath"></param>
        private void DirMove(string sPath, string dirPath)
        {
            if (!Directory.Exists(sPath))
            {
                throw new Exception($"Դ�ļ��У�{sPath}������");
            }
            if (!Directory.Exists(dirPath))
            {
                throw new Exception($"Ŀ���ļ��У�{dirPath}������");
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
        /// �޸ĳ�����ע����еļ�ֵ  
        /// </summary>  
        /// <param name="isAuto">true:��������,false:����������</param> 
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
                MessageBox.Show($"����Ҫ����ԱȨ���޸ģ�{e}", "��ʾ");
            }
        }

        /// <summary>
        /// ���û���
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

        #region ��ʼ��

        private void InitRegister()
        {
            WeakReferenceMessenger.Default.Register<string, string>(this, Const.CONST_SHOW_CUSTOM, (obj, msg) => ShowCustom());
            WeakReferenceMessenger.Default.Register<string, string>(this, Const.CONST_CONFIG_CHANGED, (obj, msg) => ConfigChangedHandler());
        }

        private void InitHotKeyKeyValue()
        {
            _hotKeyRelevancyName.Add("��С��",Const.CONST_HOT_KEY_HIDE);
            _hotKeyRelevancyName.Add("��ʾ", Const.CONST_HOT_KEY_SHOW);
            _hotKeyRelevancyName.Add("�ر�", Const.CONST_HOT_KEY_CLOSE);
            _hotKeyRelevancyName.Add("��һҳ", Const.CONST_HOT_KEY_PREVIEW);
            _hotKeyRelevancyName.Add("��һҳ", Const.CONST_HOT_KEY_NEXT);
            _hotKeyRelevancyName.Add("�Զ��Ķ�", Const.CONST_HOT_KEY_AUTO_READ);
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
