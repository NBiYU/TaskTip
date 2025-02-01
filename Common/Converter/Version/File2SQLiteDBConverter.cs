using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using TaskTip.Common.Extends;
using TaskTip.Common.Helpers;
using TaskTip.Models.DataModel;
using TaskTip.Models.Entities;
using TaskTip.Models.ViewDataModels;
using TaskTip.Services;

namespace TaskTip.Common.Converter.Version
{
    public class File2SQLiteDBConverter: BaseConverter
    {
        public File2SQLiteDBConverter(IProgress<int> total,IProgress<int> progress, CancellationToken token):base(total,progress,token)
        {
        }

        public void WorkInfoConvert(string path)
        {
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                if (json.IsNullOrEmpty()) return;
                var workInfoLst = JsonConvert.DeserializeObject<List<WorkInfoModel>>(json);
                if(workInfoLst is { Count : > 0 })
                {
                    StopToken.ThrowIfCancellationRequested();
                    TotalProgres.Report(workInfoLst.Count);
                    var db = new SQLiteDB();
                    db.InsertWorkRecord(workInfoLst);
                    File.Delete(path);
                    CurrentProgress.Report(workInfoLst.Count);
                }
            }
        }
        public void TaskFileConvert(string path)
        {
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path);
                if (files.Length == 0) return;
                TotalProgres.Report(files.Length);
                var taskLst = new List<TaskFileModel>();
                for (var i=0; i<files.Length; i++)
                {
                    StopToken.ThrowIfCancellationRequested();
                    var json = File.ReadAllText(files[i]);                  
                    if (json.IsNullOrEmpty()) continue;
                    var taskItem = JsonConvert.DeserializeObject<TaskFileModel>(json);
                    if (taskItem == null) continue;
                    taskLst.Add(taskItem);
                    CurrentProgress.Report(i);
                }
                var db = new SQLiteDB();
                db.InsertTaskList(taskLst);
                Directory.Delete(path, true);
            }
        }
        public void MenusConvert(string path)
        {
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                if (json.IsNullOrEmpty()) return;
                var tempTreeInfo = JsonConvert.DeserializeObject<TempTreeInfo>(json);
                if (tempTreeInfo != null)
                {
                    StopToken.ThrowIfCancellationRequested();
                    TotalProgres.Report(1);
                    var treeInfo = ConvertToTreeInfo(tempTreeInfo);                   
                    var bizRecordMenus = ConvertTreeInfoToBizRecordMenus(treeInfo, null);

                    var db = new SQLiteDB();
                    bizRecordMenus.RemoveAll(x => x.Guid == "RootDocument");
                    db.InsertBizRecordMenus(bizRecordMenus);
                    CurrentProgress.Report(1);
                }
                File.Delete(path);       
            }
        }
        public void RecordContentConvert(string path)
        {
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path);
                if (files.Length == 0) return;
                var recordLst = new List<RecordFileModel>();
                TotalProgres.Report(files.Length);
                for (var i=0;i<files.Length;i++)
                {
                    StopToken.ThrowIfCancellationRequested();
                    var json = File.ReadAllText(files[i]);                    
                    if (json.IsNullOrEmpty()) continue;
                    var recordItem = JsonConvert.DeserializeObject<RecordFileModel>(json);
                    if (recordItem == null) continue;
                    recordLst.Add(recordItem);
                    CurrentProgress.Report(i);
                }
                var db = new SQLiteDB();
                db.InsertRecordList(recordLst);
                Directory.Delete(path, true);
            }

        }
        private List<BizRecordMenu> ConvertTreeInfoToBizRecordMenus(TreeInfo treeInfo, string parentGuid)
        {
            StopToken.ThrowIfCancellationRequested();
            var bizRecordMenus = new List<BizRecordMenu>();

            var bizRecordMenu = new BizRecordMenu
            {
                Guid = treeInfo.GUID,
                Name = treeInfo.Name,
                IsDirectory = treeInfo.IsDirectory ? 1 : 0,
                ParentGuid = parentGuid
            };
            bizRecordMenus.Add(bizRecordMenu);

            foreach (var child in treeInfo.ChildMenus)
            {
                bizRecordMenus.AddRange(ConvertTreeInfoToBizRecordMenus(child, treeInfo.GUID));
            }

            return bizRecordMenus;
        }
        private TreeInfo ConvertToTreeInfo(TempTreeInfo tempTreeInfo)
        {
            StopToken.ThrowIfCancellationRequested();
            var treeInfo = new TreeInfo
            {
                GUID = tempTreeInfo.GUID,
                Name = tempTreeInfo.Name,
                IsDirectory = tempTreeInfo.IsDirectory
            };

            if (tempTreeInfo.Menu != null)
            {
                foreach (var directory in tempTreeInfo.Menu.Directories)
                {
                    treeInfo.ChildMenus.Add(ConvertToTreeInfo(directory));
                }
                foreach (var file in tempTreeInfo.Menu.Files)
                {
                    treeInfo.ChildMenus.Add(ConvertToTreeInfo(file));
                }
            }

            return treeInfo;
        }
    }
    public  class TempMenuTreeModel
    {
        public List<TempTreeInfo> Directories;
        public List<TempTreeInfo> Files;
    }
    public class TempTreeInfo
    {
        public string GUID;
        public string Name;
        public bool IsDirectory;
        public TempMenuTreeModel Menu;


        public override string ToString()
        {
            var path = Path.Combine(GlobalVariable.RecordFilePath, $"{GUID}{GlobalVariable.EndFileFormat}");
            if (File.Exists(path))
            {
                var obj = JsonConvert.DeserializeObject<RecordFileModel>(File.ReadAllText(path));
                if (obj != null)
                {
                    return obj.Title;
                }
            }
            return "";
        }
    }
}
