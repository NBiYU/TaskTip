using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using TaskTip.Common;
using TaskTip.Common.Converter.Map;
using TaskTip.Common.Helpers;
using TaskTip.Models.DataModel;
using TaskTip.Models.Entities;
using TaskTip.Services;
using TaskTip.Views;

namespace TaskTip.ViewModels.WindowModel
{
    public partial class FloatingTaskVM : ObservableRecipient
    {
        #region 属性
        [ObservableProperty]
        public Visibility _dtlVisibility;

        [ObservableProperty]
        public Visibility _topListVisibility;

        [ObservableProperty]
        public int _taskCount;

        [ObservableProperty]
        public string _dtlContent;

        [ObservableProperty]
        public ObservableCollection<TaskFileModel> _topCollection;

        #endregion

        #region 指令

        [RelayCommand]
        public void ShowDtlContent(string currentContent)
        {
            if (DtlContent == currentContent)
            {
                DtlContent = string.Empty;
                DtlVisibility = Visibility.Collapsed;
            }
            else
            {
                DtlContent = currentContent;
                DtlVisibility = Visibility.Visible;
            }
        }
        [RelayCommand]
        public void ModelChanged(TaskFileModel model)
        {
            var db = new SQLiteDB();
            db.UpdateTaskListItem(model);
            TaskCount = TopCollection.Count(x => !x.IsCompleted);
        }
        #endregion

        #region 初始化

        public FloatingTaskVM()
        {
            DtlVisibility = Visibility.Collapsed;
            LoadReadTaskFile();
        }

        /// <summary>
        /// 加载Task文件夹对应路径的全部文件并生成控件
        /// </summary>
        /// <param name="fileType"></param>
        private void LoadReadTaskFile()
        {
            #region SQLite

            var db = new SQLiteDB();
            var taskModels = db.GetTaskListByNotIsCompleted()
                .Select(x => x.Entity2TaskModel());

            TopCollection = [.. taskModels];
            TaskCount = TopCollection.Count;
            #endregion
        }
        #endregion
    }
}
