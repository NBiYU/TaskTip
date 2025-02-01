using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaskTip.Common.Extends;
using TaskTip.Models.DataModel;
using TaskTip.Models.Entities;
using TaskTip.Models.Enums;
using TaskTip.Models.ViewDataModels;

namespace TaskTip.Common.Converter.Map
{
    public static class MapConverter
    {
        public static TaskFileModel? Entity2TaskModel(this BizTask bizTask)
        {
            if (bizTask is null) return null;
            return new TaskFileModel
            {
                GUID = bizTask.Guid,
                TaskTimePlan = bizTask.TaskTimePlan ?? DateTime.MinValue,
                CompletedDateTime = bizTask.CompletedDateTime ?? DateTime.MinValue,
                IsCompleted = bizTask.IsCompleted == 1,
                EditTextTitle = bizTask.EditTextTitle ?? string.Empty,
                EditTextText = bizTask.EditTextText ?? string.Empty,
                CurrentTaskStatus = bizTask.BizTask2TaskStatusType()

            };
        } 
        public static WorkInfoModel? Entity2WorkModel(this BizRecordWork bizRecordWork)
        {
            if (bizRecordWork is null) return null;
            return new WorkInfoModel
            {
                RecordDate = bizRecordWork.RecordDate ?? string.Empty,
                WorkTime = bizRecordWork.WorkTime ?? string.Empty,
                StartTime = bizRecordWork.StartTime ?? string.Empty,
                EndTime = bizRecordWork.EndTime ?? string.Empty
            };
        }
        public static TreeInfo? Entity2TreeInfo(this List<BizRecordMenu> menus)
        {
            if (menus is null) return null;
            var rootMenu = new TreeInfo()
            {
                GUID = "RootDocument",
                Name = "根目录",
                IsDirectory = true
            };
            rootMenu.AddChildMenu(menus);
            return rootMenu;
        }
        public static RecordFileModel? Enity2RecordFileModel(this BizRecord bizRecordFile)
        {
            if (bizRecordFile is null) return null;
            return new RecordFileModel
            {
                GUID = bizRecordFile.Guid,
                Title = bizRecordFile.Title ?? string.Empty,
                Text = bizRecordFile.Text ?? string.Empty
            };
        }
        private static void AddChildMenu(this TreeInfo parentMenu, List<BizRecordMenu> menus)
        {
            var dic = menus.GroupBy(x => x.ParentGuid);
            var childMenus = dic.FirstOrDefault(x => x.Key == parentMenu.GUID);
            var childMenuList = new List<TreeInfo>();
            if (childMenus is null) return;
            foreach (var childMenu in childMenus)
            {
                var treeInfo = new TreeInfo()
                {
                    GUID = childMenu.Guid,
                    Name = childMenu.Name ?? string.Empty,
                    IsDirectory = childMenu.IsDirectory == 1
                };
                childMenuList.Add(treeInfo);
                AddChildMenu(treeInfo, menus);
            }
            parentMenu.ChildMenus = [.. childMenuList];
        }
    }
}
