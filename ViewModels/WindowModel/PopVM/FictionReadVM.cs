using CommunityToolkit.Mvvm.ComponentModel;using CommunityToolkit.Mvvm.Input;using Newtonsoft.Json;using System;using System.Collections.Generic;using System.Collections.ObjectModel;using System.IO;using System.Linq;using System.Text.RegularExpressions;using System.Threading;using System.Threading.Tasks;using System.Windows;using System.Windows.Media;using TaskTip.Enums;using TaskTip.Extends.FictionAPI.LRY_API;using TaskTip.Models;using TaskTip.Models.DataModel;using TaskTip.Services;namespace TaskTip.ViewModels.WindowModel.PopVM{    public partial class FictionReadVM : ObservableObject    {


        #region ������Ա
        private FictionProgressModel _model;        private LRY_APIRequest _request;        private ChapterListItem _chapterInfo;
        //private List<Chapter> _chapterList;
        private List<string> _currentChapterContent;        private string _regexRule = "^#[0-f]{6}(?:[0-f]{2})?$";





        #endregion
        #region ����
        private string _downloadTip;        public string DownloadTip        {            get => _downloadTip;            set => SetProperty(ref _downloadTip, value);        }        private bool _canDownload;        public bool CanDownload        {            get => _canDownload;            set => SetProperty(ref _canDownload, value);        }



        private string _currentChapter;        public string CurrentChapter        {            get => _currentChapter;            set => SetProperty(ref _currentChapter, value);        }        private string _currentProgress;        public string CurrentProgress
        {
            get => _currentProgress;
            set => SetProperty(ref _currentProgress, value);
        }        private string _currentContent;
        public string CurrentContent        {            get => _currentContent;            set => SetProperty(ref _currentContent, value);        }        private Visibility _exitAutoBtnVisibility;        public Visibility ExitAutoBtnVisibility        {            get => _exitAutoBtnVisibility;            set => SetProperty(ref _exitAutoBtnVisibility, value);        }        private Visibility _directoryVisibility;        public Visibility DirectoryVisibility        {            get => _directoryVisibility;            set => SetProperty(ref _directoryVisibility, value);        }        private Visibility _settingVisibility;        public Visibility SettingVisibility        {            get => _settingVisibility;            set => SetProperty(ref _settingVisibility, value);        }        private Visibility _loadingVisibility;        public Visibility LoadingVisibility
        {
            get => _loadingVisibility;
            set => SetProperty(ref _loadingVisibility, value);
        }        private bool _isDirectoryShow;        public bool IsDirectoryShow        {            get => _isDirectoryShow;            set => SetProperty(ref _isDirectoryShow, value);        }        private double _directoryHeight;        public double DirectoryHeight        {            get => _directoryHeight;            set => SetProperty(ref _directoryHeight, value);        }        private double _directoryWidth;        public double DirectoryWidth        {            get => _directoryWidth;            set => SetProperty(ref _directoryWidth, value);        }        private double _settingHeight;        public double SettingHeight        {            get => _settingHeight;            set => SetProperty(ref _settingHeight, value);        }        private double _settingWidth;        public double SettingWidth        {            get => _settingWidth;            set => SetProperty(ref _settingWidth, value);        }        private SolidColorBrush _bottomToolBrush;        public SolidColorBrush BottomToolBrush        {            get => _bottomToolBrush;            set => SetProperty(ref _bottomToolBrush, value);        }


        private ObservableCollection<string> _chapterListObservable;        public ObservableCollection<string> ChapterListObservable        {            get => _chapterListObservable;            set => SetProperty(ref _chapterListObservable, value);        }







        #endregion
        //�ĳ�ֱ��ת���ַ�����ÿ�ο�����趨ֵ����
        //private ObservableCollection<string> _contentCollection;

        //public ObservableCollection<string> ContentCollection
        //{
        //    get => _contentCollection;
        //    set => SetProperty(ref _contentCollection, value);
        //}

        #region �趨ֵ
        private int _contentGap;        public int ContentGap        {            get => _contentGap;            set            {                SetProperty(ref _contentGap, value);                LoadedContent(_model.LastContentReadIndex, value).Wait(1000);                SaveConfig<int>(value, nameof(ContentGap));            }        }        private string _backgroundColorHex;        public string BackgroundColorHex        {            get => _backgroundColorHex;            set            {                SetProperty(ref _backgroundColorHex, value);                if (Regex.IsMatch(value, _regexRule))                {                    BackgroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));                    SaveConfig<string>(value, nameof(BackgroundColorHex));                }            }        }        private string _fontColorHex;        public string FontColorHex        {            get => _fontColorHex;            set            {                SetProperty(ref _fontColorHex, value);                if (Regex.IsMatch(value, _regexRule))                {                    FontBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));                    SaveConfig<string>(value, nameof(FontColorHex));                }            }        }        private SolidColorBrush _backgroundBrush;        public SolidColorBrush BackgroundBrush        {            get => _backgroundBrush;            set => SetProperty(ref _backgroundBrush, value);        }        private SolidColorBrush _fontBrush;        public SolidColorBrush FontBrush        {            get => _fontBrush;            set => SetProperty(ref _fontBrush, value);        }        private double _windowWidth;        public double WindowWidth        {            get => _windowWidth;            set            {
                // ������С��ʱ���߶Ȼ��Ϊ160
                if (value > 160)
                {
                    if (value < 200)
                    {
                        MessageBox.Show("��ȹ�С");
                        return;
                    }
                    SetProperty(ref _windowWidth, value);
                    SaveConfig<double>(value, nameof(WindowWidth));
                    DirectoryWidth = value * 0.4;
                    SettingWidth = value;
                }
            }        }        private double _windowHeight;        public double WindowHeight        {            get => _windowHeight;            set            {
                // ������С��ʱ���߶Ȼ��Ϊ28
                if (value > 28)
                {
                    if (value < 100)
                    {
                        MessageBox.Show("�߶ȹ�С");
                        return;
                    }
                    SetProperty(ref _windowHeight, value);
                    SaveConfig<double>(value, nameof(WindowHeight));
                    DirectoryHeight = value * 0.8;
                    SettingHeight = value * 0.4;
                }
            }        }        private double _fictionFontSize;        public double FictionFontSize        {            get => _fictionFontSize;            set            {                SetProperty(ref _fictionFontSize, value);                SaveConfig<double>(value, nameof(FictionFontSize));            }        }        private bool _readMode;        public bool ReadMode        {            get => _readMode;            set            {                SetProperty(ref _readMode, value);                ExitAutoBtnVisibility = value ? Visibility.Visible : Visibility.Collapsed;            }        }        private int _readSpeed;        public int ReadSpeed        {            get => _readSpeed;            set            {                SetProperty(ref _readSpeed, value);                SaveConfig<int>(value, nameof(ReadSpeed));            }        }        private bool _isTopmost;        public bool IsTopmost        {            get => _isTopmost;            set            {                SetProperty(ref _isTopmost, value);                SaveConfig<bool>(value, nameof(IsTopmost));            }        }





        #endregion
        #region ָ���
        [RelayCommand]        public Task OpenDirectory()        {            DirectoryVisibility = DirectoryVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;            SettingVisibility = Visibility.Collapsed;            return Task.CompletedTask;        }        [RelayCommand]        public async Task CurrentChapterChanged(object sender)        {            if (sender is string chapterName)            {                var idx = _chapterInfo.chapterList.FindIndex(x => x.title == chapterName);                if (idx < 0)                {                    MessageBox.Show("�½���ת�쳣��δ�ҵ���ӦĿ��");                    return;                }                _model.LastChapterReadIndex = idx;                _model.LastContentReadIndex = 0;                DirectoryVisibility = Visibility.Collapsed;                await GetChapterContent();
            }
        }        [RelayCommand]        public void AutoReadMode()        {            ReadMode = !ReadMode;            if (ReadMode)            {                Task.Run(async () =>                {                    await Task.Delay(ReadSpeed * 1000);                    while (ReadMode)                    {                        Application.Current.Dispatcher.Invoke(() =>                        {                            RightTransform().Wait(1000);                        });                        Thread.Sleep(ReadSpeed * 1000);                    }                });            }        }        [RelayCommand]        public async Task OpenSetting()        {            SettingVisibility = SettingVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;            DirectoryVisibility = Visibility.Collapsed;        }        [RelayCommand]        public async Task LeftTransform()        {            var subNum = _model.LastContentReadIndex;            if (subNum == 0)            {                if (_model.LastChapterReadIndex != 0)                {                    _model.LastChapterReadIndex--;                    await GetChapterContent();                    _model.LastContentReadIndex = _currentChapterContent.Count - ContentGap;                }            }            else if (subNum < ContentGap)            {                _model.LastContentReadIndex = 0;            }            else            {                _model.LastContentReadIndex -= ContentGap;            }
            await LoadedContent(_model.LastContentReadIndex, ContentGap);        }        [RelayCommand]        public async Task RightTransform()        {            _model.LastContentReadIndex += ContentGap;            var subNum = _currentChapterContent.Count - _model.LastContentReadIndex;            if (subNum <= 0)            {                if (_model.LastChapterReadIndex < _chapterInfo.chapterList.Count)                {                    _model.LastChapterReadIndex++;                    _model.LastContentReadIndex = 0;                    await GetChapterContent();                }            }            else if (subNum < ContentGap)            {                await LoadedContent(_model.LastContentReadIndex, subNum);            }            await LoadedContent(_model.LastContentReadIndex, ContentGap);        }        [RelayCommand]        public async Task SaveProgress()        {            var progress = string.Empty;            var saveProgress = new List<FictionProgressModel>();            if (!File.Exists(Path.Combine(GlobalVariable.FictionCachePath, _model.Id, "Chapter.json"))) return;            if (File.Exists(GlobalVariable.FictionProgressPath))            {                progress = await File.ReadAllTextAsync(GlobalVariable.FictionProgressPath);            }            if (!string.IsNullOrEmpty(progress))            {                var objList = JsonConvert.DeserializeObject<List<FictionProgressModel>>(progress);                if (objList is { Count: > 0 })                    saveProgress.AddRange(objList);            }            var localProgress = saveProgress.FindIndex(x => x.Title == _model.Title);            if (localProgress < 0) saveProgress.Add(_model);            else saveProgress[localProgress] = _model;            await File.WriteAllTextAsync(GlobalVariable.FictionProgressPath, JsonConvert.SerializeObject(saveProgress, Formatting.Indented));        }        [RelayCommand]        public Task DownloadAllChapter()        {            var bookCacheDirPath = Path.Combine(GlobalVariable.FictionCachePath, _model.Id, "ChapterCache");            var files = Directory.GetFiles(bookCacheDirPath).Select(Path.GetFileNameWithoutExtension);            var needDownload = _chapterInfo.chapterList.Select(x => x.chapterId).Except(files).ToList();            DownloadStatusChanged(DownloadStatusType.Runtime);            Task.Run(async () =>            {                try                {                    foreach (var chapterId in needDownload)                    {                        var bookChapterCachePath = Path.Combine(bookCacheDirPath, chapterId) + ".txt";                        var content = await _request.SearchContent(chapterId);                        if (!Directory.Exists(bookCacheDirPath)) Directory.CreateDirectory(bookCacheDirPath);                        await File.WriteAllTextAsync(bookChapterCachePath, string.Join("\n", content.data)); // �����鼮�½���Ϣ
                        await Task.Delay(100);                    }                    Application.Current.Dispatcher.Invoke(() => DownloadStatusChanged(DownloadStatusType.Complete));                }                catch (Exception e)                {                    Application.Current.Dispatcher.Invoke(() => DownloadStatusChanged(DownloadStatusType.Failure));                }            });            return Task.CompletedTask;        }





        #endregion
        #region ���ܺ���
        private void UpdateChapterContentProgess()
        {
            //int total = _currentChapterContent.Count/ContentGap ;
            //int idx = _model.LastContentReadIndex / ContentGap + (_model.LastContentReadIndex % ContentGap > 0 ? 1 : 0);
            var remainderNum = _currentChapterContent.Count - _model.LastContentReadIndex;
            var currentShowNum = remainderNum > ContentGap ? ContentGap : remainderNum;
            var progress = (int)(((_model.LastContentReadIndex + currentShowNum) * 1.0 / _currentChapterContent.Count) * 100);
            CurrentProgress = $"{progress}%";
        }
        private void DownloadStatusChanged(DownloadStatusType state)        {            switch (state)            {                case DownloadStatusType.CanUpdate:                    DownloadTip = "Download All";                    CanDownload = true;                    break;                case DownloadStatusType.Complete:                    DownloadTip = "Download Complete";                    CanDownload = true;                    break;                case DownloadStatusType.Failure:                    DownloadTip = "Exception occurred during download,Please try again";                    CanDownload = true;                    break;                case DownloadStatusType.Runtime:                    DownloadTip = "Downloading,please wait some time";                    CanDownload = false;                    break;                case DownloadStatusType.Default:                    DownloadTip = "Unknown Processing";                    CanDownload = true;                    break;            }        }        private void SaveConfig<T>(T val, string varName)        {            if (GlobalVariable.JsonConfiguration[$"ReadViewSet:{varName}"] != val.ToString())                GlobalVariable.JsonConfiguration[$"ReadViewSet:{varName}", val.GetType()] = val;        }        private async Task LoadedContent(int startIndex, int gap)        {            if (_chapterInfo == null || _currentChapterContent is not { Count: > 0 }) return;            CurrentChapter = _chapterInfo.chapterList[_model.LastChapterReadIndex].title;

            CurrentContent = string.Join("\n", _currentChapterContent.Skip(startIndex).Take(gap).ToList());            UpdateChapterContentProgess();        }
        //��ȡ���½�����
        private async Task GetChapterContent()        {
            if (LoadingVisibility == Visibility.Visible) return;            LoadingVisibility = Visibility.Visible;            try
            {
                var selectChapter = _chapterInfo.chapterList[_model.LastChapterReadIndex];

                var bookCacheDirPath = Path.Combine(GlobalVariable.FictionCachePath, _model.Id, "ChapterCache");//��Ӧ�鼮����·��
                var bookChapterCachePath = Path.Combine(bookCacheDirPath, selectChapter.chapterId) + ".txt";

                // ���ػ���
                if (File.Exists(bookChapterCachePath))
                {
                    var cacheContent = await File.ReadAllTextAsync(bookChapterCachePath);
                    _currentChapterContent = cacheContent.Split("\n&&\n").ToList();
                }
                else
                {
                    var content = await _request.SearchContent(selectChapter.chapterId);
                    _currentChapterContent = new List<string>(content.data);
                    if (!Directory.Exists(bookCacheDirPath)) Directory.CreateDirectory(bookCacheDirPath);
                    await File.WriteAllTextAsync(bookChapterCachePath, string.Join("\n&&\n", _currentChapterContent)); // �����鼮�½���Ϣ
                }
                _currentChapterContent.RemoveAll(x => string.IsNullOrEmpty(x) || string.IsNullOrWhiteSpace(x));
                await LoadedContent(_model.LastContentReadIndex, ContentGap);
            }
            catch (Exception e)
            {
                MessageBox.Show($"�ڼ��ع�������������{e}");
            }
            finally
            {
                LoadingVisibility = Visibility.Collapsed;
            }
        }

        // ��ȡ�鼮�½���Ϣ
        private async Task LoadedChapter()        {            var bookCacheDirPath = Path.Combine(GlobalVariable.FictionCachePath, _model.Id);//��Ӧ�鼮����·��
            var bookChapterInfoPath = Path.Combine(bookCacheDirPath, "Chapter") + ".json";

            // ���ػ���
            if (File.Exists(bookChapterInfoPath))            {                var cacheContent = await File.ReadAllTextAsync(bookChapterInfoPath);                _chapterInfo = JsonConvert.DeserializeObject<ChapterListItem?>(cacheContent) ?? new ChapterListItem();            }            else            {                var chapters = await _request.SearchChapter(_model.Id);                _chapterInfo = chapters.data;                if (!Directory.Exists(bookCacheDirPath)) Directory.CreateDirectory(bookCacheDirPath);                await File.WriteAllTextAsync(bookChapterInfoPath, JsonConvert.SerializeObject(_chapterInfo, Formatting.Indented)); // �����鼮�½���Ϣ

            }            await HasUpdateContent(bookCacheDirPath, _chapterInfo.chapterList);            ChapterListObservable = new ObservableCollection<string>(_chapterInfo.chapterList.Select(x => x.title));            await GetChapterContent();        }        private async Task HasUpdateContent(string baseCachePath, List<Chapter> chapters)        {            var path = Path.Combine(baseCachePath, "ChapterCache");            if (!Directory.Exists(path)) Directory.CreateDirectory(path);            var files = Directory.GetFiles(path);

            if (files.Length == chapters.Count)            {                DownloadStatusChanged(DownloadStatusType.Complete);            }
            else            {                DownloadStatusChanged(DownloadStatusType.CanUpdate);            }        }




        #endregion
        #region ��ʼ��
        private void SettingValue()        {            var readViewSet = GlobalVariable.JsonConfiguration.TryGetValue<ReadViewSetModel>("ReadViewSet");            ContentGap = readViewSet.ContentGap;            ReadSpeed = readViewSet.ReadSpeed;            WindowHeight = readViewSet.WindowHeight;            WindowWidth = readViewSet.WindowWidth;            FictionFontSize = readViewSet.FictionFontSize;            IsTopmost = readViewSet.IsTopmost;            BackgroundColorHex = readViewSet.BackgroundColorHex;            FontColorHex = readViewSet.FontColorHex;            IsDirectoryShow = false;            LoadingVisibility = Visibility.Collapsed;            DirectoryVisibility = Visibility.Collapsed;            SettingVisibility = Visibility.Collapsed;            ReadMode = false;        }        public FictionReadVM()        {        }        public FictionReadVM(FictionProgressModel model)        {            _request = new();            _model = model;            SettingValue();            LoadedChapter().Wait(1000);        }    }



    #endregion               }