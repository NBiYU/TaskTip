using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Newtonsoft.Json;
using SyncDataService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using TaskTip.Common.Extends;
using TaskTip.Models;
using TaskTip.Services;

namespace TaskTip.ViewModels.WindowModel
{
    public partial class SyncMainVM:ObservableRecipient
    {
        #region 字段

        private readonly Dictionary<string, string> _categoryDic = new Dictionary<string, string>() 
        {
            {"Task","日常任务" },
            {"Record","笔记记录" },
            {"WorkTime" ,"工作时间" },
            {"FictionProcess","小说进度" },
        };

        #endregion

        #region 属性

        #region 窗体属性
        private double _windowHeight;
        public double WindowHeight 
        { 
            get => _windowHeight;
            set => SetProperty(ref _windowHeight, value);
        }

        private double _windowWidth;
        public double WindowWidth
        {
            get => _windowWidth;
            set => SetProperty(ref _windowWidth, value);
        }

        #endregion

        #region 数据对比统计

        private string syncDataCagetoryName;

        public string SyncDataCagetoryName { get => syncDataCagetoryName; set => SetProperty(ref syncDataCagetoryName, value); }

        private int fileCount;

        public int FileCount { get => fileCount; set => SetProperty(ref fileCount, value); }
        private int mergerFileCount;

        public int MergerFileCount { get => mergerFileCount; set => SetProperty(ref mergerFileCount, value); }
        private int conflictFileCount;

        public int ConflictFileCount { get => conflictFileCount; set => SetProperty(ref conflictFileCount, value); }

        #endregion

        #region 差异同步
        private bool? isShowConflict;

        public bool? IsShowConflict { get => isShowConflict; set => SetProperty(ref isShowConflict, value); }

        private bool? isShowMerger;

        public bool? IsShowMerger { get => isShowMerger; set => SetProperty(ref isShowMerger, value); }

        private int mergerCount;

        public int MergerCount { get => mergerCount; set => SetProperty(ref mergerCount, value); }

        private int conflictProcessed;

        public int ConflictProcessed { get => conflictProcessed; set => SetProperty(ref conflictProcessed, value); }

        private int conflictCount;

        public int ConflictCount { get => conflictCount; set => SetProperty(ref conflictCount, value); }
        #endregion

        #region 同步页面


        private bool remoteIsAllCheck;

        public bool RemoteIsAllCheck { get => remoteIsAllCheck; set => SetProperty(ref remoteIsAllCheck, value); }
        private bool localIsAllCheck;

        public bool LocalIsAllCheck { get => localIsAllCheck; set => SetProperty(ref localIsAllCheck, value); }

        private double maxOneFourthWidth;

        public double MaxOneFourthWidth { get => maxOneFourthWidth; set => SetProperty(ref maxOneFourthWidth, value); }
        private double maxListHeight;

        public double MaxListHeight { get => maxListHeight; set => SetProperty(ref maxListHeight, value); }

        private ObservableCollection<FileListItem> fileList;

        public ObservableCollection<FileListItem> FileList { get => fileList; set => SetProperty(ref fileList, value); }

        private ObservableCollection<CompareContent> remoteContent;

        public ObservableCollection<CompareContent> RemoteContent { get => remoteContent; set => SetProperty(ref remoteContent, value); }

        private ObservableCollection<CompareContent> localContent;

        public ObservableCollection<CompareContent> LocalContent { get => localContent; set => SetProperty(ref localContent, value); }
        #endregion

        #region 同步类型选择页面

        private System.Windows.Visibility mainVisibility;

        public System.Windows.Visibility MainVisibility { get => mainVisibility; set => SetProperty(ref mainVisibility, value); }

        private List<string> syncCategoryList;

        public List<string> SyncCategoryList { get => syncCategoryList; set => SetProperty(ref syncCategoryList, value); }

        #endregion

        #endregion

        #region 命令

        [RelayCommand]
        public void ToMain()
        {
            MainVisibility = MainVisibility == System.Windows.Visibility.Visible ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }

        [RelayCommand]
        public async Task Preview()
        {

        }
        [RelayCommand]
        public async Task Next()
        {

        }
        [RelayCommand]
        public async Task FileSelectChanged(object sender)
        {
            if (sender is FileListItem item)
            {
                await LoadContent(item.Guid);
            }
        }

        [RelayCommand]
        public async Task Finish(object sender)
        {
            if (sender is FileListItem item)
            {
                if (!(MessageBox.Show("是否同步", "是否完成同步，并将文件合并", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
                    return;
                try
                {
                    var dic = _categoryDic.FirstOrDefault(x => x.Value == SyncDataCagetoryName);
                    switch (dic.Key)
                    {
                        case "Task":
                            var newContent = await GetSyncResultList();

                            var remotePath = System.IO.Path.Combine(GlobalVariable.TaskFilePath, "Temp", $"{item.Guid}{GlobalVariable.EndFileFormat}");
                            var localPath = System.IO.Path.Combine(GlobalVariable.TaskFilePath, $"{item.Guid}{GlobalVariable.EndFileFormat}");

                            if (newContent.Count == 0) {
                                MessageBox.Show("同步失败，选择内容为空");
                                return;
                            }

                            if (newContent.Count > (LocalContent.Count > RemoteContent.Count ? LocalContent.Count : RemoteContent.Count))
                            {
                                MessageBox.Show($"同步结果数据异常：{string.Join("\r\n", newContent)}\r\n清重新检查操作");
                                return;
                            }

                            await File.WriteAllTextAsync(remotePath, string.Join("\r\n", newContent));
                            if (File.Exists(localPath))
                            {
                                File.Delete(localPath);
                            }
                            File.Move(remotePath, localPath);


                            break;
                        case "Record":

                            break;
                        case "WorkTime":

                            break;
                        case "FictionProcess":

                            break;
                        default:
                            break;
                    }


                    FileList.Remove(item);
                    RemoteContent.Clear();
                    LocalContent.Clear();
                    if (item.CompareStatus == CompareStatus.Merger)
                    {
                        MergerFileCount--;
                    }
                    else
                    {
                        ConflictFileCount--;
                    }
                    FileCount--;


                    MergerCount = 0;
                    ConflictCount = 0;
                    ConflictProcessed = 0;
                }
                catch (Exception e)
                {
                    MessageBox.Show($"合并异常 ：{e.Message}");
                }

            }

        }
        [RelayCommand]
        public async Task ConfirmSyncCategory(string category)
        {

            FileCount = 0;
            MergerFileCount = 0;
            ConflictFileCount = 0;
            RemoteContent.Clear();
            LocalContent.Clear();

            //SyncDataServiceClient.Address = "http://localhost:8085/SyncDataService.svc";
            
            var dic = _categoryDic.FirstOrDefault(x => x.Value == category);

            var (message, fileList) = await GetFileList(dic);
            
            FileList = new ObservableCollection<FileListItem>(fileList);

            FileCount = FileList.Count;
            SyncDataCagetoryName = dic.Value;
            MainVisibility = System.Windows.Visibility.Collapsed;

            if (dic.Value != message)
                MessageBox.Show(message);
        }

        [RelayCommand]
        public async Task AllCheck(string sender)
        {
            var length = MiniNum(RemoteContent.Count, LocalContent.Count);
            switch (sender) {
                case "Remote":
                    for (int i = 0; i < RemoteContent.Count; i++)
                    {
                        RemoteContent[i].IsCheck = true;
                    }

                    for (int i = 0; i < length; i++)
                    {
                        LocalContent[i].IsCheck = false;
                    }
                    break;
                case "Local":
                    for (int i = 0; i < LocalContent.Count; i++)
                    {
                        LocalContent[i].IsCheck = true;
                    }
                    for (int i = 0; i < length; i++)
                    {
                        RemoteContent[i].IsCheck = false;
                    }
                    break;
            }

            ConflictCountProcessedChange();
        }
        [RelayCommand]
        public async Task UnAllCheck(string sender)
        {
            var length = MiniNum(RemoteContent.Count, LocalContent.Count);
            switch (sender)
            {
                case "Remote":
                    for (int i = 0; i < RemoteContent.Count; i++)
                    {
                        RemoteContent[i].IsCheck = false;
                    }
                    for (int i = 0; i < length; i++)
                    {
                        LocalContent[i].IsCheck = true;
                    }
                    break;
                case "Local":
                    for (int i = 0; i < LocalContent.Count; i++)
                    {
                        LocalContent[i].IsCheck = false;
                    }
                    for (int i = 0; i < length; i++)
                    {
                        RemoteContent[i].IsCheck = true;
                    }
                    break;
            }
            ConflictCountProcessedChange();
        }

        [RelayCommand]
        public void CheckChanged(object sender)
        {
            var values = (object[])sender;
            if (values[0] is string type && values[1] is CompareContent content)
            {
                var rowId = content.LineNumber - 1;
                switch (type)
                {
                    case "Local":
                        if (content.LineNumber < RemoteContent.Count && content.IsCheck == true && RemoteContent[rowId].IsCheck == true)
                        {
                            RemoteContent[rowId].IsCheck = false;
                        }
                        break;
                    case "Remote":
                        if (content.LineNumber < LocalContent.Count && content.IsCheck == true && LocalContent[rowId].IsCheck == true)
                        {
                            LocalContent[rowId].IsCheck = false;
                        }
                        break;
                    default: break;
                }
                ConflictCountProcessedChange();
            }
        }


        #endregion

        #region 函数

        private async Task<(string,List<FileListItem>)> GetFileList(KeyValuePair<string,string> category)
        {
            var res = new ResModel();
            var files = new List<FileListItem>();
            var remotePath = System.IO.Path.Combine(GlobalVariable.TaskTipPath, "Temp", category.Key);

            try
            {
 
                if (!Directory.Exists(remotePath))
                    Directory.CreateDirectory(remotePath);

                using (var client = new SyncDataServiceClient())
                {
                    switch (category.Key)
                    {
                        case "Task":
                            res = await client.GetTaskInfoAsync();

                            if (res.Result)
                            {
                                var remoteTaskList = JsonConvert.DeserializeObject<List<TaskFileModel>>(res.Content);

                                foreach (var item in remoteTaskList)
                                {
                                    files.Add(await CompareWithSave(item,
                                        System.IO.Path.Combine(GlobalVariable.TaskFilePath, $"{item.GUID}{GlobalVariable.EndFileFormat}"),
                                        System.IO.Path.Combine(remotePath, $"{item.GUID}{GlobalVariable.EndFileFormat}")));
                                }

                                files.RemoveAll(x => x == new FileListItem());

                                return (category.Value, files);
                            }
                            break;
                        case "Record":
                            res = await client.GetMenuInfoAsync();
                            if (res.Result)
                            {
                                var menuObj = JsonConvert.DeserializeObject<TreeInfo>(res.Content);
                                var menufile = await CompareWithSave(menuObj, GlobalVariable.MenuTreeConfigPath, System.IO.Path.Combine(remotePath, System.IO.Path.GetFileName(GlobalVariable.MenuTreeConfigPath)));
                                menufile.Title = "目录配置文件";
                                files.Add(menufile);
                                var remoteMenuList = menuObj.GetFiles();
                                foreach (var tree in remoteMenuList)
                                {
                                    var t = await client.GetRecordInfoAsync();
                                    if (t.Result && !string.IsNullOrEmpty(t.Content))
                                    {
                                        files.Add(await CompareWithSave(JsonConvert.DeserializeObject<RecordFileModel>(t.Content),
                                                System.IO.Path.Combine(GlobalVariable.MenoFilePath, $"{tree.GUID}{GlobalVariable.EndFileFormat}"),
                                                System.IO.Path.Combine(remotePath, $"{tree.GUID}{GlobalVariable.EndFileFormat}")));
                                    }
                                    return (category.Value, files);
                                }
                            }
                            break;
                        case "WorkTime":
                            res = await client.GetWorkTimeInfoAsync();
                            break;
                        case "FictionProcess":
                            res = await client.GetFictionInfoAsync();
                            break;
                        default:
                            break;
                    }
                }

                

            }
            catch (Exception e)
            {
                  res.Message = e.ToString();
            }
            return ($"{category.Value} 数据获取失败,返回信息：{res.Message}", new List<FileListItem>());
        }

        private async Task<FileListItem> CompareWithSave(dynamic item,string path,string tempPath)
        {
            var compareStatus = CompareStatus.Default;
            if (File.Exists(path))
            {
                var content = await File.ReadAllTextAsync(path);
                if (content == JsonConvert.SerializeObject(item, Formatting.Indented))
                {
                    return new FileListItem();
                }
                ConflictFileCount++;
                compareStatus = CompareStatus.Conflict;
            }
            else
            {
                MergerFileCount++;
                compareStatus = CompareStatus.Merger;
            }
            await File.WriteAllTextAsync(System.IO.Path.Combine(tempPath), JsonConvert.SerializeObject(item, Formatting.Indented));
            return new FileListItem() { Title = item.ToString(), Guid = item.GUID, CompareStatus = compareStatus };
        }

        public void ConflictCountProcessedChange()
        {
            if (LocalContent.Count == 0) return;
            var localContent = new List<CompareContent>(LocalContent);
            var conflicts = localContent.FindAll(x => x.HasVarying == Visibility.Visible);

            var conflictProcessed = 0;

            foreach (var conflict in conflicts)
            {
                if (conflict.LineNumber > RemoteContent.Count && conflictProcessed < ConflictCount)
                {
                    conflictProcessed++;
                    break;
                }

                if (RemoteContent[conflict.LineNumber - 1].IsCheck == true || LocalContent[conflict.LineNumber - 1].IsCheck == true)
                    conflictProcessed++;

            }

            ConflictProcessed = conflictProcessed;
        }

        private async Task<List<string>> GetSyncResultList()
        {
            var newContent = new List<CompareContent>();
            var localContent = new List<CompareContent>(LocalContent);
            var remoteContent = new List<CompareContent>(RemoteContent);
            var contentList = new List<string>();
            newContent.AddRange(localContent.FindAll(x => x.IsCheck == true));
            newContent.AddRange(remoteContent.FindAll(x => x.IsCheck == true));

            foreach (var CompareContent in newContent.OrderBy(x => x.LineNumber))
            {
                var p = ((Paragraph)CompareContent.FlowContent.Blocks.First());
                var rowContent = new List<string>();
                foreach (Run run in p.Inlines)
                {
                    rowContent.Add(run.Text);
                }
                contentList.Add(string.Join("", rowContent));
            }
            return contentList.Distinct().ToList();
        }

        private async Task LoadContent(string guid) 
        {
            RemoteContent.Clear();
            LocalContent.Clear();

            var needOp = false;
            (needOp,var local) = await GetFileContentAsync(System.IO.Path.Combine(GlobalVariable.TaskFilePath, $"{guid}{GlobalVariable.EndFileFormat}"));
            (needOp, var remote) = await GetFileContentAsync(System.IO.Path.Combine(GlobalVariable.TaskFilePath,"Temp", $"{guid}{GlobalVariable.EndFileFormat}"));


            var differ = new Differ();
            var inlineDiffBuilder = new SideBySideDiffBuilder(differ);
            var diff = inlineDiffBuilder.BuildDiffModel(local,remote);

            LocalContent =  GetContent(diff.OldText);
            RemoteContent =  GetContent(diff.NewText);

            Func<ChangeType, bool> isConflictFunc = new Func<ChangeType, bool>((type) => type != ChangeType.Inserted && type != ChangeType.Unchanged);

            ConflictCount = RemoteContent.Count(x => isConflictFunc(x.BurshToTypeConvert(x.MergerOrConfictBrush)) );
            MergerCount = RemoteContent.Count(x => x.BurshToTypeConvert(x.MergerOrConfictBrush) != ChangeType.Unchanged && !isConflictFunc(x.BurshToTypeConvert(x.MergerOrConfictBrush)));

            LocalIsAllCheck = false;
            RemoteIsAllCheck = false;
        }

        private ObservableCollection<CompareContent> GetContent(DiffPaneModel model)
        {
            var content = new ObservableCollection<CompareContent>();
            if (model.HasDifferences)
            {
                foreach (var item in model.Lines)
                {
                    if (item.Position == null) continue;
                    content.Add(new CompareContent(item));
                }
            }
            return content;
        }

        private async Task<(bool,string)> GetFileContentAsync(string path)
        {
            if (File.Exists(path))
            {
                return (true,await File.ReadAllTextAsync(path));
            }
            return (false,string.Empty);
        }


        private int MiniNum(int a, int b)
        {
            return a < b ? a : b;
        }

        #endregion

        #region 初始化

        private List<string> GetCategories()
        {
            var categories = new List<string>();
            foreach (var category in _categoryDic)
            {
                categories.Add(category.Value);
            }
            return categories;
        }

        private void InitProperies()
        {
            WindowHeight = 800;
            WindowWidth = 1500;
            SyncDataCagetoryName = "当前未选择";
            FileCount = 0;
            IsShowConflict = true;
            IsShowMerger = true;
            MergerCount = 0;
            ConflictProcessed = 0;
            ConflictCount = 0;
            MaxOneFourthWidth = WindowWidth / 5;
            MaxListHeight = (WindowHeight / 4) * 3.5;
            FileList = new ObservableCollection<FileListItem>();
            RemoteContent = new ObservableCollection<CompareContent>();
            localContent = new ObservableCollection<CompareContent>();
            MainVisibility = Visibility.Collapsed;
            SyncCategoryList = GetCategories();
        }

        public SyncMainVM()
        {
           InitProperies();
        }


        #endregion
    }

    public class FileListItem
    {
        public string Title { get; set; }
        public string Guid { get; set; }
        public CompareStatus CompareStatus { get; set; }

    }

    public class CompareContent:ObservableRecipient {

        #region 颜色定义

        private Color unChanged = Colors.Black;
        private Color inserted = Colors.Green;
        private Color imaginary = Colors.AliceBlue;
        private Color deleted = Colors.OrangeRed;
        private Color modified = Colors.Orange;


        #endregion

        public int LineNumber { get; set; }
        public string LineContent { get; set; }
        public FlowDocument FlowContent { get; set; }
        private bool? isCheck = false;
        public bool? IsCheck { get => isCheck; set => SetProperty(ref isCheck, value); } 
        public Visibility HasVarying { get; set; }
        public SolidColorBrush MergerOrConfictBrush { get; set; }

        public CompareContent(int lineNumber, string lineContent, Visibility hasVarying, SolidColorBrush mergerOrConfictBrush)
        {
            LineNumber = lineNumber;
            LineContent = lineContent;
            HasVarying = hasVarying;
            MergerOrConfictBrush = mergerOrConfictBrush;
        }

        public CompareContent(int lineNumber, DiffPiece diffPiece, ChangeType changeType)
        {
            LineNumber = lineNumber;
            FlowContent = DiffPaneModelToFlowDocument(diffPiece);
            HasVarying = changeType == ChangeType.Unchanged ? Visibility.Hidden : Visibility.Visible;
            MergerOrConfictBrush = TypeToBrushConvert(changeType);
        }

        public CompareContent(DiffPiece diffPiece)
        {
            LineNumber = diffPiece.Position ?? 0;
            FlowContent = DiffPaneModelToFlowDocument(diffPiece);
            HasVarying = diffPiece.Type == ChangeType.Unchanged ? Visibility.Hidden : Visibility.Visible;
            IsCheck = diffPiece.Type == ChangeType.Unchanged;
            MergerOrConfictBrush = TypeToBrushConvert(diffPiece.Type);
        }

        private FlowDocument DiffPaneModelToFlowDocument(DiffPiece model)
        {
            var document = new FlowDocument();
            var p = new Paragraph();
            if (model.SubPieces.Count == 0)
            {
                var brush = TypeToBrushConvert(model.Type);
                p.Inlines.Add(new Run(model.Text) { Foreground = brush });

            }else
            {
                foreach (var subPiece in model.SubPieces)
                {
                    p.Inlines.Add(new Run(subPiece.Text) { Foreground = TypeToBrushConvert(subPiece.Type) });
                }
            }
            document.Blocks.Add(p);
            return document;
        }

        public SolidColorBrush TypeToBrushConvert(ChangeType type)
        {
            return type switch
            {
                ChangeType.Unchanged => new SolidColorBrush(unChanged),
                ChangeType.Inserted => new SolidColorBrush(inserted),
                ChangeType.Imaginary => new SolidColorBrush(imaginary),
                ChangeType.Deleted => new SolidColorBrush(deleted),
                ChangeType.Modified => new SolidColorBrush(modified),
            };
        }

        public ChangeType BurshToTypeConvert(SolidColorBrush brush)
        {
            if(brush.Color.Equals(unChanged))
                return ChangeType.Unchanged;
            else if(brush.Color.Equals(inserted))
                return ChangeType.Inserted;
            else if(brush.Color.Equals(imaginary))
                return ChangeType.Imaginary;
            else if(brush.Color.Equals(deleted))
                return ChangeType.Deleted;
            else 
                return ChangeType.Modified;
        }
    }

    public enum CompareStatus
    {
        [Description("无变化")]
        Default,
        [Description("合并")]
        Merger,
        [Description("冲突")]
        Conflict,
        [Description("完成")]
        Finish
    }
}
