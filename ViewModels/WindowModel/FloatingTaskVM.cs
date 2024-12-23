using CommunityToolkit.Mvvm.ComponentModel;

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
using TaskTip.Models.DataModel;
using TaskTip.Services;
using TaskTip.Views;

namespace TaskTip.ViewModels.WindowModel
{
    public partial class FloatingTaskVM : ObservableObject
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



        #endregion

        #region 初始化

        public FloatingTaskVM()
        {
            DtlVisibility = Visibility.Collapsed;
            LoadReadTaskFile(GlobalVariable.TaskFilePath);


        }

        /// <summary>
        /// 加载Task文件夹对应路径的全部文件并生成控件
        /// </summary>
        /// <param name="fileType"></param>
        private void LoadReadTaskFile(string dirPath)
        {
            if (string.IsNullOrEmpty(dirPath) || !Directory.Exists(dirPath))
                return;
            var config = new ConfigurationHelper();


            var filePaths = Directory.GetFiles(dirPath, "*.task");

            var take = 3;
            var modelList = new List<TaskFileModel>();

            foreach (var item in filePaths)
            {
                modelList.Add(JsonConvert.DeserializeObject<TaskFileModel>(File.ReadAllText(item)));
            }
            var readFile = modelList.OrderByDescending(x => x.CompletedDateTime).Where(x => !x.IsCompleted).ToList();
            TopCollection = new ObservableCollection<TaskFileModel>(readFile);
            TaskCount = readFile.Count;
        }

        #endregion
    }
}
