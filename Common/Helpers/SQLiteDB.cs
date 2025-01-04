using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using TaskTip.Models.Entities;
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
        public void UpdateTaskListItem(TaskFileModel model)
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
    }
}
