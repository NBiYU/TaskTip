using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Newtonsoft.Json;

using Quartz;
using Quartz.Impl;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using TaskTip.Common;
using TaskTip.Common.Converter.Map;
using TaskTip.Common.ExecuteServices;
using TaskTip.Common.Extends;
using TaskTip.Common.Helpers;
using TaskTip.Models.CommonModel;
using TaskTip.Models.DataModel;
using TaskTip.Models.Entities;
using TaskTip.Models.Enums;
using TaskTip.Services;
using TaskTip.ViewModels.Base;
using TaskTip.ViewModels.UserViewModel;
using TaskTip.Views;
using TaskTip.Views.Windows.PopWindow;

namespace TaskTip.ViewModels.PageModel
{
    internal partial class TaskListPageModel : BaseVM
    {
        private double _oldScrollableHeight;

        private double taskMenoWidth;
        public double TaskMenoWidth
        {
            get => taskMenoWidth;
            set => SetProperty(ref taskMenoWidth, value);
        }

        private Visibility _loadingVisibility;
        public Visibility LoadingVisibility { get => _loadingVisibility; set => SetProperty(ref _loadingVisibility, value); }

        /// <summary>
        /// 待完成的任务集合
        /// </summary>
        private ObservableCollection<TaskFileModel> taskList;

        public ObservableCollection<TaskFileModel> TaskList
        {
            get => taskList;
            set { SetProperty(ref taskList, value); }
        }
        [ObservableProperty]
        public string _searchStr;

        private ISchedulerFactory schedulerFactory;
        private IScheduler scheduler;

        #region 控件指令处理函数

        /// <summary>
        /// 日报生成事件
        /// </summary>
        [RelayCommand]
        private void SendDailyMessage()
        {
            var outEndTime = GlobalVariable.DailyTaskEndTime;
            var isCreateTomorrowPlan = GlobalVariable.IsCreateTomorrowPlan;

            var outMode = DateTime.Now > DateTime.Parse(outEndTime)
                ? DateTime.Today
                : DateTime.Today.AddDays(-1);
            var db = new SQLiteDB();
            var todayTask = db.GetTaskListByDate(outMode)
                .Select(x => x.Entity2TaskModel())
                .Select((x, i) => $"{i + 1}.{x.EditTextTitle}:{x.EditTextText} ----- {(x.IsCompleted ? "已完成" : "未完成")}");
            var tomorrowTask = db.GetTaskListByDate(outMode.AddDays(1))
                .Select(x => x.Entity2TaskModel())
                .Select((x, i) => $"{i + 1}.{x.EditTextTitle}:{x.EditTextText}");

            if (todayTask.Count() != 0)
            {
                var text = string.Join("\n", todayTask.ToArray()) +
                           "\n\n\n\n明日计划：" + (isCreateTomorrowPlan && tomorrowTask.Count() != 0 ? string.Join("\n", tomorrowTask.ToArray()) : "");
                Clipboard.SetText(text);
                MessageBox.Show($"{DateTime.Today.ToString("yyyy-MM-dd ")}的日报已生成到剪贴板中" +
                                (isCreateTomorrowPlan && tomorrowTask.Count() != 0 ? $"{DateTime.Today.AddDays(1).ToString("yyyy-MM-dd ")} 的日报已生成到剪贴板中" : ""));
            }
            else
            {
                MessageBox.Show("当前没有记录或计划完成的任务");
            }


        }


        [RelayCommand]
        public void TaskLoaded()
        {
            LoadingVisibility = Visibility.Visible;
            LoadReadTaskFile();
            LoadingVisibility = Visibility.Collapsed;
        }

        [RelayCommand]
        public void TaskUnloaded()
        {
            TaskList.Clear();
        }

        [RelayCommand]
        public void ScrollChanged(object sender)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                if (scrollViewer.ScrollableHeight == 0)
                {
                    scrollViewer.ScrollToTop();
                    return;
                }
                else if (scrollViewer.ScrollableHeight == scrollViewer.VerticalOffset && _oldScrollableHeight != scrollViewer.ScrollableHeight)
                {
                    _oldScrollableHeight = scrollViewer.ScrollableHeight;
                    LoadingVisibility = Visibility.Visible;
                    LoadReadTaskFile();
                    LoadingVisibility = Visibility.Collapsed;
                }
            }
        }

        [RelayCommand]
        public void SearchDataHandler()
        {
            if (SearchStr.IsNullOrEmpty())
            {
                TaskLoaded();
                return;
            }
            var db = new SQLiteDB();
            var bizTasks = db.GetTaskListByText(SearchStr);
            TaskList = [..bizTasks.Select(x=>new TaskFileModel {
                    GUID = x.Guid,
                    TaskTimePlan = x.TaskTimePlan ?? DateTime.MinValue,
                    CompletedDateTime = x.CompletedDateTime ?? DateTime.MinValue,
                    IsCompleted = x.IsCompleted == 1,
                    EditTextTitle = x.EditTextTitle ?? string.Empty,
                    EditTextText = x.EditTextText ?? string.Empty
                })];
            //var searchResult = _fileDataCache.Where(x => x.EditTextTitle.Contains(SearchStr) || x.EditTextText.Contains(SearchStr));
            //TaskList = new ObservableCollection<TaskListItemUserControl>(ReadTaskFile(searchResult.Select(x => x.GUID).ToList()));
        }

        #region ListBoxItem RelayCommand

        /// <summary>
        /// 任务添加指令处理函数
        /// </summary>
        [RelayCommand]
        private void AddTaskList()
        {
            TaskList.Add(new TaskFileModel() { GUID = Guid.NewGuid().ToString(), IsCompleted = false });
            TaskListChanged();
        }

        [RelayCommand]
        public void ModelChanged(TaskFileModel model)
        {
            var db = new SQLiteDB();
            db.UpdateTaskListItem(model);
            TaskListChanged();
        }
        [RelayCommand]
        public void SelectPlanTime(TaskFileModel model)
        {
            var win = new ClockSelectorPop(model.TaskTimePlan == DateTime.MinValue ? DateTime.Now : model.TaskTimePlan);
            win.Confirmed += async (o, e) =>
            {
                if (o != null)
                {
                    model.TaskTimePlan = (DateTime)o;
                    var db = new SQLiteDB();
                    db.UpdateTaskListItem(model);
                    model.CurrentTaskStatus = model.TaskFileModel2TaskStatusType();
                    await AddTaskScheduleJob(model);
                }
            };
            win.ShowDialog();
        }
        [RelayCommand]
        public void DeleteItem(TaskFileModel model)
        {
            var db = new SQLiteDB();
            db.DeleteTaskListItem(model.GUID);
            TaskList.Remove(model);
            TaskListChanged();
        }
        #endregion

        #endregion

        #region 功能函数

        /// <summary>
        /// 把控件传来的作业和触发器加入到调度器中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task AddTaskScheduleJob(TaskFileModel task)
        {
            if ((task.TaskTimePlan - DateTime.Now).TotalMilliseconds < 500)
            {
                return;
            }


            if ((task.TaskTimePlan - DateTime.Now).TotalDays >= GlobalVariable.DeleteTimes)
            {
                //MessageBox.Show("计划时间于或等于定时删除时间，请调整");
                return;
            }

            IJobDetail job = JobBuilder.Create<TaskPlanJob>()
                .WithIdentity($"TipJob{task.GUID}", $"Group{task.GUID}")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity($"TipTrigger{task.GUID}", $"Group{task.GUID}")
                .StartAt(task.TaskTimePlan)
                .Build();
            string errorMsg = await scheduler.CheckExists(job.Key)
                ? await scheduler.DeleteJob(job.Key) ? "" : $"替换{job.Key}作业异常"
                : "";

            if (string.IsNullOrEmpty(errorMsg))
                await scheduler.ScheduleJob(job, trigger);
            else
                MessageBox.Show(errorMsg);
        }

        /// <summary>
        /// 完成状态列表重新排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsCompletedChanged(CorrespondenceModel corr)
        {
            TaskList = [..TaskList.OrderBy(x => x.IsCompleted)
                .OrderByDescending(x => x.CompletedDateTime)];
            TaskListChanged();
        }


        /// <summary>
        /// 加载Task文件夹对应路径的全部文件并生成控件
        /// </summary>
        /// <param name="fileType"></param>
        private void LoadReadTaskFile()
        {
            #region SQLite

            var db = new SQLiteDB();
            var count = TaskList.Count / GlobalVariable.LoadTaskFilePageSize;
            var taskModels = db.GetTaskListByPageSize(count, GlobalVariable.LoadTaskFilePageSize)
                .Select(x => x.Entity2TaskModel());

            TaskList = [.. taskModels];
            #endregion
            TaskListChanged();
        }

        private void TaskListChanged()
        {
            WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { Message = TaskList }, Const.CONST_TASK_LIST_CHANGED);
        }
        private void TaskListItemStatusChanged(TaskStatusModel taskStatus)
        {
            var item = TaskList.FirstOrDefault(x => x.GUID == taskStatus.GUID);
            if(item != null)
            {
                item.CurrentTaskStatus = taskStatus.Status;
            }
        }
        #endregion

        private void InitRegister()
        {
            WeakReferenceMessenger.Default.Register<string, string>(this, Const.CONST_TASK_RELOAD, (obj, msg) => { TaskList.Clear(); LoadReadTaskFile(); });
            WeakReferenceMessenger.Default.Register<TaskStatusModel, string>(this, Const.CONST_TASK_STATUS_CHANGED, (o, msg) => TaskListItemStatusChanged(msg));
        }
        private void UnRegister()
        {
            WeakReferenceMessenger.Default.Unregister<string, string>(this, Const.CONST_TASK_RELOAD);
            WeakReferenceMessenger.Default.Unregister<TaskStatusModel, string>(this, Const.CONST_TASK_STATUS_CHANGED);
        }

        public TaskListPageModel()
        {
            //需要增加路径不存在判断
            taskList = new ObservableCollection<TaskFileModel>();
            VMName = SyncFileCategory.TaskPlan.GetDesc();

            schedulerFactory = new StdSchedulerFactory();
            scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.Start();
            LoadingVisibility = Visibility.Collapsed;
            taskMenoWidth = SystemParameters.WorkArea.Height / 3;

            InitRegister();
            TaskLoaded();
        }

        ~TaskListPageModel()
        {
            UnRegister();
        }
    }
}
