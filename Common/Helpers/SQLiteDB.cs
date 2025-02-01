using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using TaskTip.Models.DataModel;
using TaskTip.Models.Entities;
using TaskTip.Models.ViewDataModels;
using TaskTip.ViewModels.UserViewModel;

namespace TaskTip.Common.Helpers
{
    public  class SQLiteDB
    {
        private TaskTipDbContext Context { get; set; }
        public SQLiteDB()
        {
            Context = new TaskTipDbContext();
        }

        #region SysParam
        public Dictionary<string,string> GetSysParamsAllForDic()
        {
            return Context.SysParams.ToDictionary(x => x.Key, x => x.Value)!;
        }
        public  List<SysParam> GetSysParams()
        {
            return Context.SysParams.ToList();
        }
        public  string? GetSysParamByKey(string key)
        {
            var result = Context.SysParams.Where(x => x.Key == key).FirstOrDefault();
            return result?.Value;
        }
        public  void UpdateSysParams(Dictionary<string,string> keyValues)
        {
            foreach(var keyValue in keyValues)
            {
                var result = Context.SysParams
                    .Where(x => x.Key == keyValue.Key).
                    ExecuteUpdate(x => x.SetProperty(s => s.Value, keyValue.Value));
            }
        }
        public  void UpdateSysParam(string key,object value)
        {
            Context.SysParams
                .Where(x => x.Key == key)
                .ExecuteUpdate(x => x.SetProperty(s => s.Value, value.ToString()));
        }

        #endregion

        #region BizTask
        public List<BizTask> GetTaskListByDate(DateTime dateTime)
        {
            var result = Context.BizTasks
                .Where(x => x.CompletedDateTime != null && x.CompletedDateTime.Value.Date == dateTime.Date)
                .ToList();
            return result;
        }
        public BizTask? GetTaskListByGuid(string guid)
        {
            var result = Context.BizTasks.FirstOrDefault(x => x.Guid == guid);
            return result;
        }
        public List<BizTask> GetTaskListByText(string searchStr)
        {
            var result = Context.BizTasks
                .Where(x => (!string.IsNullOrEmpty(x.EditTextText) && x.EditTextText.Contains(searchStr)) 
                                || (!string.IsNullOrEmpty(x.EditTextTitle) && x.EditTextTitle.Contains(searchStr)))
                .ToList();
            return result;
        }
        public List<BizTask> GetTaskListByPageSize(int currrentCount,int pageSize)
        {
            var result = Context.BizTasks
                .OrderBy(x => x.IsCompleted)
                .OrderByDescending(x => x.CompletedDateTime)
                .Take(pageSize).ToList();
            return result;
        }
        public List<BizTask> GetTaskListByNotIsCompleted()
        {
            var result = Context.BizTasks.Where(x => x.IsCompleted == 0).ToList();
            return result;
        }
        public void ReplaceTaskListItem(TaskFileModel model)
        {
            var existedModel = Context.BizTasks.Where(x => x.Guid == model.GUID).FirstOrDefault();
            var isInsert = existedModel == null;
            if (isInsert) existedModel = new BizTask();
            existedModel.Guid = model.GUID;
            existedModel.TaskTimePlan = model.TaskTimePlan;
            existedModel.IsCompleted = model.IsCompleted ? 1 : 0;
            existedModel.EditTextText = model.EditTextText;
            existedModel.EditTextTitle = model.EditTextTitle;
            existedModel.CompletedDateTime = model.IsCompleted ? DateTime.Now : DateTime.MinValue;
            if (isInsert)
            {
                Context.BizTasks.Add(existedModel);
            }
            else
            {
                Context.BizTasks.Update(existedModel);
            }
            Context.SaveChanges();
        }
        public void InsertTaskList(List<TaskFileModel> tasks)
        {
            foreach(var item in tasks)
            {
                Context.BizTasks.Add(new BizTask
                {
                    Guid = item.GUID,
                    TaskTimePlan = item.TaskTimePlan,
                    IsCompleted = item.IsCompleted ? 1 : 0,
                    EditTextText = item.EditTextText,
                    EditTextTitle = item.EditTextTitle,
                    CompletedDateTime = item.IsCompleted ? DateTime.Now : DateTime.MinValue
                });
            }
            Context.SaveChanges();
        }
        public void DeleteTaskListItem(string guid)
        {
            var result = Context.BizTasks
                .Where(x => x.Guid == guid)
                .ExecuteDelete();
            Context.SaveChanges();
        }
        #endregion

        #region BizWorkRecord

        public void InsertWorkRecord(WorkInfoModel model)
        {
            var existedModel = Context.BizRecordWorks.Where(x => x.RecordDate == model.RecordDate).FirstOrDefault();
            var isInsert = existedModel == null;
            if (isInsert) existedModel = new BizRecordWork();
            existedModel.Guid = model.RecordDate;
            existedModel.WorkTime = model.WorkTime;
            existedModel.StartTime = model.StartTime;
            existedModel.EndTime = model.EndTime;
            existedModel.RecordDate = model.RecordDate;
            if (isInsert)
            {
                Context.BizRecordWorks.Add(existedModel);
            }
            else
            {
                Context.BizRecordWorks.Update(existedModel);
            }
            Context.SaveChanges();
        }
        public void InsertWorkRecord(List<WorkInfoModel> workInfoModels)
        {
            foreach(var model in workInfoModels)
            {
                var existedModel = Context.BizRecordWorks.Where(x => x.RecordDate == model.RecordDate).FirstOrDefault();
                var isInsert = existedModel == null;
                if (isInsert) existedModel = new BizRecordWork();
                existedModel.Guid = model.RecordDate;
                existedModel.WorkTime = model.WorkTime;
                existedModel.StartTime = model.StartTime;
                existedModel.EndTime = model.EndTime;
                existedModel.RecordDate = model.RecordDate;
                if (isInsert)
                {
                    Context.BizRecordWorks.Add(existedModel);
                }
                else
                {
                    Context.BizRecordWorks.Update(existedModel);
                }
            }
            Context.SaveChanges();
        }
        public List<BizRecordWork> GetAllWorkRecord()
        {
            return [.. Context.BizRecordWorks];
        }
        public void DeleteWorkRecordByRecordDate(string recordDate)
        {
            Context.BizRecordWorks.Where(x=>x.RecordDate == recordDate).ExecuteDelete();
            Context.SaveChanges();
        }
        #endregion

        #region BizRecordMenu

        public List<BizRecordMenu> GetAllMenus()
        {
            return Context.BizRecordMenus.ToList();
        }
        public void InsertMenuByParentGuid(string parentGuid,TreeInfo info)
        {
            Context.BizRecordMenus.Add(new BizRecordMenu
            {
                Guid = info.GUID,
                Name = info.Name,
                IsDirectory = info.IsDirectory ? 1 : 0,
                ParentGuid = parentGuid
            });
            Context.SaveChanges();
        }
        internal void InsertBizRecordMenus(List<BizRecordMenu> bizRecordMenus)
        {
            Context.BizRecordMenus.AddRange(bizRecordMenus);
            Context.SaveChanges();
        }
        public void UpdateMenu(TreeInfo info)
        {
            Context.BizRecordMenus.First(x => x.Guid == info.GUID).Name = info.Name;
            Context.SaveChanges();
        }

        public void DeleteMenuByGuid(string guid)
        {
            Context.BizRecordMenus.Where(x => x.Guid == guid).ExecuteDelete();
            Context.SaveChanges();
        }
        #endregion

        #region BizRecord

        public BizRecord? GetRecordByGuid(string guid)
        {
            return Context.BizRecords.FirstOrDefault(x => x.Guid == guid);
        }
        public void ReplaceRecord(RecordFileModel record)
        {
            var existedRecord = Context.BizRecords.FirstOrDefault(x => x.Guid == record.GUID);
            var isInsert = existedRecord == null;
            if (isInsert) existedRecord = new BizRecord();
            existedRecord.Guid = record.GUID;
            existedRecord.Title = record.Title;
            existedRecord.Text = record.Text;
            if (isInsert)
            {
                Context.BizRecords.Add(existedRecord);
            }
            else
            {
                Context.BizRecords.Update(existedRecord);
                Context.BizRecordMenus.First(x => x.Guid == existedRecord.Guid).Name = existedRecord.Title;
            }
            Context.SaveChanges();
        }

        internal void InsertRecordList(List<RecordFileModel> recordLst)
        {
            foreach (var record in recordLst)
            {
                Context.BizRecords.Add(new BizRecord
                {
                    Guid = record.GUID,
                    Title = record.Title,
                    Text = record.Text
                });
            }
            Context.SaveChanges();
        }


        #endregion
    }
}
