using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TaskTip.Services;
using TaskTip.Views;

namespace TaskTip.ViewModels
{
    internal class TaskListPageModel : ObservableObject
    {

        private double taskMenoWidth;
        public double TaskMenoWidth
        {
            get => taskMenoWidth;
            set => SetProperty(ref taskMenoWidth, value);
        }


        /// <summary>
        /// 待完成的任务集合
        /// </summary>
        private ObservableCollection<TaskListItemUserControl> taskMenoList;

        public ObservableCollection<TaskListItemUserControl> TaskMenoList
        {
            get => taskMenoList;
            set { SetProperty(ref taskMenoList, value); }
        }


        private ISchedulerFactory schedulerFactory;
        private IScheduler scheduler;


        /// <summary>
        /// 生成一个新的TaskListItem空控件
        /// </summary>
        /// <returns></returns>
        private TaskListItemUserControl AddTaskListItemControl()
        {
            var taskControl = new TaskListItemUserControl();
            FocusManager.AddGotFocusHandler(taskControl, GotFocusText);
            //FocusManager.SetFocusedElement(control,control.EditTaskTitle);
            //Keyboard.Focus(taskControl.EditTaskTitle);
            taskControl.CompletedLine.X2 = TaskMenoWidth * 0.9;
            taskControl.Width = TaskMenoWidth * 0.9;
            return taskControl;
        }

        /// <summary>
        /// 当失去焦点，其他列表元素的下拓展输入框自动收回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GotFocusText(object sender, EventArgs e)
        {
            var taskControl = (TaskListItemUserControl)sender;
            Keyboard.ClearFocus();
            for (int i = 0; i < taskMenoList.Count; i++)
            {
                if (TaskMenoList[i].Guid.Text == taskControl.Guid.Text)
                {
                    TaskMenoList[i].EditTaskText.Visibility = Visibility.Visible;
                    if (TaskMenoList[i].EditTaskTitle.IsFocused)
                        Keyboard.Focus(TaskMenoList[i].EditTaskTitle);
                    continue;
                }
                TaskMenoList[i].EditTaskText.Visibility = Visibility.Collapsed;
            }
        }


        public static event EventHandler TaskListChanged;

        #region 控件指令
        /// <summary>
        /// 添加Task控件按钮触发指令
        /// </summary>
        public RelayCommand AddTaskListCommand { get; set; }

        /// <summary>
        /// 发送日报指令
        /// </summary>
        public RelayCommand SendDailyMessageCommand { get; set; }


        #endregion

        #region 控件指令处理函数

        /// <summary>
        /// 任务添加指令处理函数
        /// </summary>
        private void AddTaskList()
        {
            TaskMenoList.Insert(0, AddTaskListItemControl());
            TaskListChanged?.Invoke(TaskMenoList, null);
        }


        /// <summary>
        /// 日报生成事件,???为什么不直接拿TaskMenoList里面的数据呢
        /// </summary>
        private void SendDailyMessage()
        {
            var dirPath = ConfigurationManager.AppSettings["TaskFilePath"];
            var outEndTime = ConfigurationManager.AppSettings["DailyTaskEndTime"];

            if (!Directory.Exists(dirPath))
            {
                MessageBox.Show($"{dirPath}不存在,请前往设置重新设定路径");
                return;
            }

            var filePaths = Directory.GetFiles(dirPath);
            var todayTask = new List<string>();
            var tomorrowTask = new List<string>();
            var isCreateTomorrowPlan = bool.Parse(ConfigurationManager.AppSettings["IsCreateTomorrowPlan"]);

            //从文件中获取当天的任务内容
            foreach (var filePath in filePaths)
            {
                var json = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(filePath));
                try
                {
                    //格式： 序号.任务标题：任务内容 ------- 是否完成
                    //例如： 1.完成日报生成：通过获取当天记录或预计当天完成的任务生成文本并复制到剪切板中 ----- 未完成

                    var outMode = DateTime.Now > DateTime.Parse(outEndTime)
                        ? DateTime.Today
                        : DateTime.Today.AddDays(-1);

                    if (json["TaskTimePlan"].Value<DateTime>().Date == outMode)
                        todayTask.Add(todayTask.Count + 1 + "." + json["EditTextTitle"].Value<string>() + "：" +
                                      json["EditTextText"].Value<string>() + " ----- " +
                                      (json["IsCompleted"].Value<bool>() ? "已完成" : "未完成"));

                    if (isCreateTomorrowPlan && json["TaskTimePlan"].Value<DateTime>().Date == outMode.AddDays(1))
                    {
                        tomorrowTask.Add(tomorrowTask.Count + 1 + "." + json["EditTextTitle"].Value<string>() + "：" +
                                         json["EditTextText"].Value<string>());
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show($"获取当天任务内容失败，错误信息：{e}");
                    throw;
                }
            }


            if (todayTask.Count != 0)
            {
                var text = string.Join("\n", todayTask.ToArray()) +
                           ("\n\n\n\n明日计划：" + (isCreateTomorrowPlan && tomorrowTask.Count != 0 ? string.Join("\n", tomorrowTask.ToArray()) : ""));
                Clipboard.SetText(text);
                MessageBox.Show($"{DateTime.Today.ToString("yyyy-MM-dd ")}的日报已生成到剪贴板中" +
                                (isCreateTomorrowPlan && tomorrowTask.Count != 0 ? $"{DateTime.Today.AddDays(1).ToString("yyyy-MM-dd ")} 的日报已生成到剪贴板中" : ""));
            }
            else
            {
                MessageBox.Show("当前没有记录或计划完成的任务");
            }


        }

        #endregion

        #region 外部控件处理事件

        /// <summary>
        /// List删除指令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteListItem(object sender, EventArgs e)
        {
            var path = ConfigurationManager.AppSettings.Get("TaskFilePath") + "\\" + (string)sender + ConfigurationManager.AppSettings.Get("EndFileFormat");


            try
            {
                var tipJovKey = new JobKey($"Tip{sender.ToString()}");
                var deleteJovKey = new JobKey($"Delete{sender.ToString()}");
                if (scheduler.CheckExists(tipJovKey).Result)
                {
                    scheduler.DeleteJob(tipJovKey);
                }

                if (scheduler.CheckExists(deleteJovKey).Result)
                {
                    scheduler.DeleteJob(deleteJovKey);
                }

                TaskMenoList.Remove(TaskMenoList.FirstOrDefault(x => x.Guid.Text == sender.ToString()));
                TaskListChanged?.Invoke(TaskMenoList, null);

            }
            catch
            {
                //抛出异常一般为定时删除任务存在线程占用，所以开个线程来进行删除
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TaskMenoList.Remove(TaskMenoList.FirstOrDefault(x => x.Guid.Text == sender.ToString() && x.IsCompleted.IsChecked == true));
                    TaskListChanged?.Invoke(TaskMenoList, null);
                });
            }

            if (File.Exists(path))
                File.Delete(path);

        }

        #endregion

        /// <summary>
        /// 把控件传来的作业和触发器加入到调度器中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddTaskScheduleJob(object sender, EventArgs e)
        {
            IJobDetail job;
            ITrigger trigger;

            (job, trigger) = ((IJobDetail, ITrigger))sender;

            string errorMsg = scheduler.CheckExists(job.Key).Result
                ? scheduler.DeleteJob(job.Key).Result ? "" : $"替换{job.Key}作业异常"
                : "";

            if (string.IsNullOrEmpty(errorMsg))
                scheduler.ScheduleJob(job, trigger);
            else
                MessageBox.Show(errorMsg);
        }

        /// <summary>
        /// 完成状态变更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsCompleted(object sender, EventArgs e)
        {
            var isCompleteItem = taskMenoList.FirstOrDefault(x => x.Guid.Text == sender.ToString());


            if (isCompleteItem != null)
            {
                TaskListChanged?.Invoke(TaskMenoList, null);
                SortList();
            }
        }

        private void SortList()
        {
            TaskMenoList =
                new ObservableCollection<TaskListItemUserControl>(TaskMenoList.OrderBy(x => x.IsCompleted.IsChecked));
        }


        /// <summary>
        /// 加载Task文件夹对应路径的全部文件并生成控件
        /// </summary>
        /// <param name="fileType"></param>
        private ObservableCollection<TaskListItemUserControl> LoadReadTaskFile(string fileType)
        {
            var dirPath = ConfigurationManager.AppSettings.Get(fileType);

            if (string.IsNullOrEmpty(dirPath))
                return new ObservableCollection<TaskListItemUserControl>();

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            //if (taskMenoList.Count != 0)
            //{
            //    taskMenoList = new ObservableCollection<TaskListItemUserControl>();
            //}

            var taskListControl = new ObservableCollection<TaskListItemUserControl>();

            var filePaths = Directory.GetFiles(dirPath);


            foreach (var filePath in filePaths)
            {
                var startIndex = filePath.LastIndexOf('\\') + 1;
                var endIndex = filePath.LastIndexOf('.');
                var text = filePath.Substring(startIndex, endIndex - startIndex);

                if (!filePath.EndsWith(ConfigurationManager.AppSettings.Get("EndFileFormat")))
                    continue;

                if (startIndex == -1 || endIndex == -1)
                    continue;

                var taskControl = AddTaskListItemControl();
                taskControl.Guid.Text = text;
                taskListControl.Add(taskControl);
            }

            if (taskListControl.Count == 0)
            {
                taskListControl.Add(AddTaskListItemControl());
            }

            return taskListControl;
        }

        public TaskListPageModel()
        {
            //需要增加路径不存在判断
            taskMenoList = new ObservableCollection<TaskListItemUserControl>();


            schedulerFactory = new StdSchedulerFactory();
            scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.Start();

            SendDailyMessageCommand = new RelayCommand(SendDailyMessage);
            AddTaskListCommand = new RelayCommand(AddTaskList);


            TaskListItemUserControlModel.DeleteMsg += DeleteListItem;
            DeleteTaskJob.DeleteMsg += DeleteListItem;
            TaskListItemUserControl.IsCompleteMsg += IsCompleted;
            TaskListItemUserControlModel.TaskSchedule += AddTaskScheduleJob;

            taskMenoWidth = SystemParameters.WorkArea.Height / 3;

            TaskMenoList = new ObservableCollection<TaskListItemUserControl>(LoadReadTaskFile("TaskFilePath"));
            SortList();

            TaskListChanged?.Invoke(TaskMenoList, EventArgs.Empty);
        }
    }
}
