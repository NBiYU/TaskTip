using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TaskTip.Base;
using TaskTip.Common;
using TaskTip.Common.Extends;
using TaskTip.Enums;
using TaskTip.Models;
using TaskTip.Models.CommonModel;
using TaskTip.Models.DataModel;
using TaskTip.Services;
using TaskTip.ViewModels.UserViewModel;
using TaskTip.Views;

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
        /// ����ɵ����񼯺�
        /// </summary>
        private ObservableCollection<TaskListItemUserControl> taskList;

        public ObservableCollection<TaskListItemUserControl> TaskList
        {
            get => taskList;
            set { SetProperty(ref taskList, value); }
        }
        [ObservableProperty]
        public string _searchStr;
        
        private ISchedulerFactory schedulerFactory;
        private IScheduler scheduler;
        private List<TaskFileModel> _fileDataCache;

        /// <summary>
        /// ����һ���µ�TaskListItem�տؼ�
        /// </summary>
        /// <returns></returns>
        private TaskListItemUserControl AddTaskListItemControl(string guid)
        {
            if (string.IsNullOrEmpty(guid)) guid = Guid.NewGuid().ToString();
            var taskControl = new TaskListItemUserControl();
            var taskControlModel = taskControl.TaskGrid.DataContext as TaskListItemUserControlModel;
            taskControlModel.GUID = guid;
            taskControl = InitItemControl(taskControl);
            return taskControl;
        }

        private TaskListItemUserControl AddTaskListItemControl(TaskFileModel taskFile)
        {
            var taskControl = new TaskListItemUserControl();
            var taskControlModel = taskControl.TaskGrid.DataContext as TaskListItemUserControlModel;
            taskControlModel!.PropertySetValue(taskFile, true);
            taskControl = InitItemControl(taskControl);
            return taskControl;
        }

        private TaskListItemUserControl InitItemControl(TaskListItemUserControl control)
        {
            FocusManager.AddGotFocusHandler(control, GotFocusText);
            control.CompletedLine.X2 = TaskMenoWidth * 0.9;
            control.Width = TaskMenoWidth * 0.9;
            return control;
        }

        /// <summary>
        /// ��ʧȥ���㣬�����б�Ԫ�ص�����չ������Զ��ջ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GotFocusText(object sender, EventArgs e)
        {
            var taskControl = (TaskListItemUserControl)sender;
            Keyboard.ClearFocus();
            for (int i = 0; i < taskList.Count; i++)
            {
                if (TaskList[i].Guid.Text == taskControl.Guid.Text)
                {
                    TaskList[i].EditTaskText.Visibility = Visibility.Visible;
                    if (TaskList[i].EditTaskTitle.IsFocused)
                        Keyboard.Focus(TaskList[i].EditTaskTitle);
                    continue;
                }
                TaskList[i].EditTaskText.Visibility = Visibility.Collapsed;
            }
        }


        #region �ؼ�ָ�����

        /// <summary>
        /// �������ָ�����
        /// </summary>
        [RelayCommand]
        private void AddTaskList()
        {
            WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { GUID = Guid.NewGuid().ToString(), Operation = OperationRequestType.Add }, Const.CONST_LISTITEM_CHANGED);

        }


        /// <summary>
        /// �ձ������¼�,???Ϊʲô��ֱ����TaskMenoList�����������
        /// </summary>
        [RelayCommand]
        private void SendDailyMessage()
        {
            var dirPath = GlobalVariable.TaskFilePath;
            var outEndTime = GlobalVariable.DailyTaskEndTime;

            if (!Directory.Exists(dirPath))
            {
                MessageBox.Show($"{dirPath}������,��ǰ�����������趨·��");
                return;
            }

            var filePaths = Directory.GetFiles(dirPath);
            var todayTask = new List<string>();
            var tomorrowTask = new List<string>();
            var isCreateTomorrowPlan = GlobalVariable.IsCreateTomorrowPlan;

            //���ļ��л�ȡ�������������
            foreach (var filePath in filePaths)
            {
                var obj = JsonConvert.DeserializeObject<TaskFileModel>(File.ReadAllText(filePath));
                try
                {
                    //��ʽ�� ���.������⣺�������� ------- �Ƿ����
                    //���磺 1.����ձ����ɣ�ͨ����ȡ�����¼��Ԥ�Ƶ�����ɵ����������ı������Ƶ����а��� ----- δ���

                    var outMode = DateTime.Now > DateTime.Parse(outEndTime)
                        ? DateTime.Today
                        : DateTime.Today.AddDays(-1);

                    if (obj.TaskTimePlan.Date == outMode)
                        todayTask.Add(todayTask.Count + 1 + "." + obj.EditTextTitle + "��" +
                                      obj.EditTextText + " ----- " +
                                      (obj.IsCompleted ? "�����" : "δ���"));

                    if (isCreateTomorrowPlan && obj.TaskTimePlan.Date == outMode.AddDays(1))
                    {
                        tomorrowTask.Add(tomorrowTask.Count + 1 + "." + obj.EditTextTitle + "��" + obj.EditTextText);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show($"��ȡ������������ʧ�ܣ�������Ϣ��{e}");
                    throw;
                }
            }


            if (todayTask.Count != 0)
            {
                var text = string.Join("\n", todayTask.ToArray()) +
                           "\n\n\n\n���ռƻ���" + (isCreateTomorrowPlan && tomorrowTask.Count != 0 ? string.Join("\n", tomorrowTask.ToArray()) : "");
                Clipboard.SetText(text);
                MessageBox.Show($"{DateTime.Today.ToString("yyyy-MM-dd ")}���ձ������ɵ���������" +
                                (isCreateTomorrowPlan && tomorrowTask.Count != 0 ? $"{DateTime.Today.AddDays(1).ToString("yyyy-MM-dd ")} ���ձ������ɵ���������" : ""));
            }
            else
            {
                MessageBox.Show("��ǰû�м�¼��ƻ���ɵ�����");
            }


        }


        [RelayCommand]
        public void TaskLoaded()
        {
            LoadingVisibility = Visibility.Visible;
            LoadReadTaskFile(GlobalVariable.TaskFilePath);
            LoadingVisibility = Visibility.Collapsed;
            WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { Message = TaskList }, Const.CONST_TASK_LIST_CHANGED);
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
                    LoadReadTaskFile(GlobalVariable.TaskFilePath);
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
            var searchResult = _fileDataCache.Where(x => x.EditTextTitle.Contains(SearchStr) || x.EditTextText.Contains(SearchStr));
            TaskList = new ObservableCollection<TaskListItemUserControl>(ReadTaskFile(searchResult.Select(x => x.GUID).ToList()));
        }
        #endregion

        #region �ⲿ�ؼ������¼�


        private async Task ListItemChanged(CorrespondenceModel corr)
        {
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    var msgModel = corr.Message as TaskFileModel;

                    if (TaskList.FirstOrDefault(x => x.Guid.Text.ToString() == corr.GUID) != null
                        && corr.Operation == OperationRequestType.Add)
                    {
                        corr.Operation = OperationRequestType.Update;
                    }

                    switch (corr.Operation)
                    {
                        case OperationRequestType.Delete:
                            var item = TaskList.FirstOrDefault(x => x.Guid.Text == corr.GUID);
                            if (item == null) return;
                            var deleteModel = item.TaskGrid.DataContext as TaskListItemUserControlModel;
                            OperationRecord.OperationRecordWrite(new TcpRequestData() { GUID = corr.GUID, OperationType = corr.Operation, SyncCategory = SyncFileCategory.TaskPlan, FileData = deleteModel.TaskFile });
                            await DeleteListItem(corr.GUID);
                            break;
                        case OperationRequestType.Update:
                            var control = TaskList.FirstOrDefault(x => x.Guid.Text == corr.GUID);
                            if (control == null) return;

                            var vm = control.TaskGrid.DataContext as TaskListItemUserControlModel;
                            if (vm == null) throw new Exception("δ�ҵ�VM����");

                            if (vm.TaskFile.Equals(msgModel)) break;

                            vm.PropertySetValue(msgModel);

                            OperationRecord.OperationRecordWrite(new TcpRequestData() { GUID = corr.GUID, OperationType = corr.Operation, SyncCategory = SyncFileCategory.TaskPlan, FileData = msgModel });
                            break;
                        case OperationRequestType.Add:
                            if (msgModel == null)
                            {
                                TaskList.Insert(0, AddTaskListItemControl(corr.GUID));
                            }
                            else
                            {
                                TaskList.Insert(0, AddTaskListItemControl(msgModel));
                            }
                            OperationRecord.OperationRecordWrite(new TcpRequestData() { GUID = corr.GUID, OperationType = corr.Operation, SyncCategory = SyncFileCategory.TaskPlan, FileData = msgModel });

                            break;
                    }

                    WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { Message = TaskList }, Const.CONST_TASK_LIST_CHANGED);
                }
                catch (Exception ex)
                {
                    GlobalVariable.LogHelper.Error($"��{this}�����¼�·�ɡ���{corr.Operation.GetDesc()}����{corr.GUID}��������һ��С���⣺{ex}");
                }

            });

        }

        /// <summary>
        /// Listɾ��ָ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task DeleteListItem(string sender)
        {
            var path = Path.Combine(GlobalVariable.TaskFilePath, sender + GlobalVariable.EndFileFormat);

            try
            {
                var tipJovKey = new JobKey($"Tip{sender}");
                var deleteJovKey = new JobKey($"Delete{sender}");
                if (await scheduler.CheckExists(tipJovKey))
                {
                    await scheduler.DeleteJob(tipJovKey);
                }

                if (await scheduler.CheckExists(deleteJovKey))
                {
                    await scheduler.DeleteJob(deleteJovKey);
                }

                TaskList.Remove(TaskList.FirstOrDefault(x => x.Guid.Text == sender));

            }
            catch
            {
                //�׳��쳣һ��Ϊ��ʱɾ����������߳�ռ�ã����Կ����߳�������ɾ��
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TaskList.Remove(TaskList.FirstOrDefault(x => x.Guid.Text == sender && x.IsCompleted.IsChecked == true));
                });
            }

            if (File.Exists(path))
                File.Delete(path);

            ExecuteLogger($"��{this}��������ɾ������{sender}�� ��ɾ��");
        }

        #endregion

        #region ���ܺ���

        /// <summary>
        /// �ѿؼ���������ҵ�ʹ��������뵽��������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task AddTaskScheduleJob(TaskTimeModel task)
        {
            string errorMsg = await scheduler.CheckExists(task.Job.Key)
                ? await scheduler.DeleteJob(task.Job.Key) ? "" : $"�滻{task.Job.Key}��ҵ�쳣"
                : "";

            if (string.IsNullOrEmpty(errorMsg))
                await scheduler.ScheduleJob(task.Job, task.Trigger);
            else
                MessageBox.Show(errorMsg);
        }

        /// <summary>
        /// ���״̬�б���������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsCompletedChanged(CorrespondenceModel corr)
        {
            SortList();
        }

        private void SortList()
        {
            TaskList = new(TaskList.OrderByDescending(x => (x.TaskGrid.DataContext as TaskListItemUserControlModel)!.CompletedDateTime).OrderBy(x => (x.TaskGrid.DataContext as TaskListItemUserControlModel)!.IsCompleted));
        }


        /// <summary>
        /// ����Task�ļ��ж�Ӧ·����ȫ���ļ������ɿؼ�
        /// </summary>
        /// <param name="fileType"></param>
        private void LoadReadTaskFile(string dirPath)
        {
            if (string.IsNullOrEmpty(dirPath))
                return;
            var config = new ConfigurationHelper();


            var filePaths = Directory.GetFiles(dirPath, "*.task");

            var take = int.Parse(config["Global:LoadTaskFilePageSize"] ?? "0");
            var modelList = new List<TaskFileModel>();

            foreach (var item in filePaths)
            {
                modelList.Add(JsonConvert.DeserializeObject<TaskFileModel>(File.ReadAllText(item)));
            }
            var readFile = modelList.OrderByDescending(x => x.CompletedDateTime).OrderBy(x => x.IsCompleted).Skip(TaskList.Count).Take(TaskList.Count + take > filePaths.Length ? filePaths.Length - TaskList.Count : take).ToList();
            var taskListControl = new List<TaskListItemUserControl>(TaskList);
            taskListControl.AddRange(ReadTaskFile(readFile.Select(x => x.GUID).ToList()));
            _fileDataCache = modelList;
            TaskList = new ObservableCollection<TaskListItemUserControl>(taskListControl);

        }

        private List<TaskListItemUserControl> ReadTaskFile(List<string> files)
        {
            var taskListControl = new List<TaskListItemUserControl>();
            foreach (var filePath in files)
            {
                var guid = Path.GetFileNameWithoutExtension(filePath);
                var taskControl = AddTaskListItemControl(guid);
                taskListControl.Add(taskControl);
            }
            return taskListControl;
        }

        #endregion


        private void InitRegister()
        {
            WeakReferenceMessenger.Default.Register<string, string>(this, Const.CONST_TASK_RELOAD, (obj, msg) => { LoadReadTaskFile(msg); });
            WeakReferenceMessenger.Default.Register<CorrespondenceModel, string>(this, Const.CONST_LISTITEM_CHANGED, async (obj, msg) => { await ListItemChanged(msg); });
            WeakReferenceMessenger.Default.Register<CorrespondenceModel, string>(this, Const.CONST_TASK_LIST_CHANGED, (obj, msg) => { IsCompletedChanged(msg); });
            WeakReferenceMessenger.Default.Register<TaskTimeModel, string>(this, Const.CONST_SCHEDULE_CREATE,
                 async (obj, msg) => { await AddTaskScheduleJob(msg); });
        }
        private void UnRegister()
        {
            WeakReferenceMessenger.Default.Unregister<string, string>(this, Const.CONST_TASK_RELOAD);
            WeakReferenceMessenger.Default.Unregister<CorrespondenceModel, string>(this, Const.CONST_LISTITEM_CHANGED);
            WeakReferenceMessenger.Default.Unregister<CorrespondenceModel, string>(this, Const.CONST_TASK_LIST_CHANGED);
            WeakReferenceMessenger.Default.Unregister<TaskTimeModel, string>(this, Const.CONST_SCHEDULE_CREATE);
        }


        public TaskListPageModel()
        {
            //��Ҫ����·���������ж�
            taskList = new ObservableCollection<TaskListItemUserControl>();
            VMName = SyncFileCategory.TaskPlan.GetDesc();

            schedulerFactory = new StdSchedulerFactory();
            scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.Start();
            LoadingVisibility = Visibility.Collapsed;
            taskMenoWidth = SystemParameters.WorkArea.Height / 3;

            InitRegister();
        }

        ~TaskListPageModel()
        {
            UnRegister();
        }
    }
}
