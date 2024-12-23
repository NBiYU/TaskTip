using CommunityToolkit.Mvvm.ComponentModel;using CommunityToolkit.Mvvm.Input;using CommunityToolkit.Mvvm.Messaging;using Newtonsoft.Json;using Quartz;using System;using System.IO;using System.Text;using System.Windows;using TaskTip.Models;using TaskTip.Services;using TaskTip.Views;using TaskTip.Common;using MessageBox = System.Windows.MessageBox;using TaskTip.Common.ExecuteServices;using TaskTip.Enums;using TaskTip.Models.DataModel;using TaskTip.Models.CommonModel;namespace TaskTip.ViewModels.UserViewModel{    internal partial class TaskListItemUserControlModel : ObservableObject    {






























        /// <summary>        /// 是否允许更改属性时保存        /// </summary>                                                                                                                                                                                                                                                                                            public bool isInitialize = false;






























        /// <summary>        /// 时间选择器        /// </summary>                                                                                                                                                                                                                                                                    private DateTimeGetView taskTime;        public TaskFileModel TaskFile { get; set; } = new();









































        #region 个人属性
        /// <summary>        /// 唯一标识GUID码        /// </summary>                                                                                                                                                                                                                                                                                    public string GUID        {            get => TaskFile.GUID;            set            {                SetProperty(ref TaskFile.GUID, value);                ReadFileTextData();            }        }
































        /// <summary>        /// 是否完成，勾选框属性        /// </summary>                                                                                                                                                                                                                                                                                        public bool IsCompleted        {            get => TaskFile.IsCompleted;            set            {                SetProperty(ref TaskFile.IsCompleted, value);                CompleteVisibility = TaskFile.IsCompleted == true ? Visibility.Visible : Visibility.Collapsed;                if (TaskFile.IsCompleted == true)                    CompletedDateTime = DateTime.Now;                if (!isInitialize)                    WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { GUID = GUID, Operation = OperationRequestType.Update, Message = TaskFile }, Const.CONST_LISTITEM_CHANGED);                IsEnableControl = TaskFile.IsCompleted == false;                SaveDataText();            }        }        private string trashImagePath;        public string TrashImagePath        {            get => trashImagePath;            set => SetProperty(ref trashImagePath, value);        }































        /// <summary>        /// 控件是否可用        /// </summary>                                                                                                                                                                                                                                                                        private bool isEnableControl = true;        public bool IsEnableControl        {            get => isEnableControl;            set => SetProperty(ref isEnableControl, value);        }































        /// <summary>        /// 划线是否显示        /// </summary>                                                                                                                                                                                                                                                                        private Visibility completedVisibility;        public Visibility CompleteVisibility        {            get => completedVisibility;            set => SetProperty(ref completedVisibility, value);        }
































        /// <summary>        /// 任务标题        /// </summary>                                                                                                                                                                                                                                                                public string EditTextTitle        {            get => TaskFile.EditTextTitle;            set            {                SetProperty(ref TaskFile.EditTextTitle, value);                SaveDataText();            }        }































        /// <summary>        /// 任务详细        /// </summary>                                                                                                                                                                                                                                                                public string EditTextText        {            get => TaskFile.EditTextText;            set            {                SetProperty(ref TaskFile.EditTextText, value);                SaveDataText();            }        }































        /// <summary>        /// 任务详情是否展开        /// </summary>                                                                                                                                                                                                                                                                                private Visibility visibilityEditText;        public Visibility VisibilityEditText        {            get => visibilityEditText;            set            {                SetProperty(ref visibilityEditText, value);            }        }
































        /// <summary>        /// 任务提醒时间        /// </summary>                                                                                                                                                                                                                                                                        public DateTime TaskTimePlan        {            get => TaskFile.TaskTimePlan;            set            {                SetProperty(ref TaskFile.TaskTimePlan, value);                ToolTaskTime = TaskFile.TaskTimePlan.ToString("yyyy-MM-dd HH:mm:ss");                CurrentTaskStatus = TaskTimePlan == DateTime.MinValue ? TaskStatusType.Undefined :                    IsCompleted ? TaskStatusType.Complete :                    TaskTimePlan > DateTime.Now ? TaskStatusType.Runtime : TaskStatusType.Timeout;                SaveDataText();                TaskTipTimer();                TaskDeleteTimer();            }        }


























        /// <summary>        /// 提示字符        /// </summary>                                                                                                                                                                                                                                                                private string toolTaskTime;        public string ToolTaskTime        {            get => toolTaskTime;            set => SetProperty(ref toolTaskTime, DateTime.Parse(value) == DateTime.MinValue ? "当前无计划事件" : "计划 " + value + "完成");        }        private TaskStatusType _currentTaskStatus;        public TaskStatusType CurrentTaskStatus        {            get => _currentTaskStatus;            set => SetProperty(ref _currentTaskStatus, value);        }        public DateTime CompletedDateTime { get; set; }








































        #endregion
        #region 定时任务        /// <summary>                             /// 当选择的时间发生改变时,生成一个定时提醒作业发给调度器                             /// </summary>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            private void TaskTipTimer()        {            if ((TaskTimePlan - DateTime.Now).TotalMilliseconds < 500)            {                return;            }            if ((TaskTimePlan - DateTime.Now).TotalDays >= GlobalVariable.DeleteTimes)            {
                //MessageBox.Show("计划时间于或等于定时删除时间，请调整");
                return;            }            if (!File.Exists(Path.Combine(GlobalVariable.TaskFilePath, $"{GUID}{GlobalVariable.EndFileFormat}")))            {                return;            }            IJobDetail job = JobBuilder.Create<TaskPlanJob>()                .WithIdentity($"TipJob{GUID}", $"Group{GUID}")                .Build();            ITrigger trigger = TriggerBuilder.Create()                .WithIdentity($"TipTrigger{GUID}", $"Group{GUID}")                .StartAt(TaskTimePlan)                .Build();            WeakReferenceMessenger.Default.Send(new TaskTimeModel() { Job = job, Trigger = trigger }, Const.CONST_SCHEDULE_CREATE);        }        private void TaskDeleteTimer()        {            if (!(File.Exists(Path.Combine(GlobalVariable.TaskFilePath, $"{GUID}{GlobalVariable.EndFileFormat}")) && GlobalVariable.IsEnableAutoDelete))            {                return;            }            IJobDetail job = JobBuilder.Create<DeleteTaskJob>()                .WithIdentity($"DeleteJob{GUID}", $"Group{GUID}")                .Build();            ITrigger trigger = TriggerBuilder.Create().WithIdentity($"DeleteTrigger{GUID}")                .StartAt(TaskTimePlan.AddDays(GlobalVariable.DeleteTimes) < DateTime.Now                    ? DateTimeOffset.Now.AddMinutes(1) : TaskTimePlan.AddDays(GlobalVariable.DeleteTimes)).Build();            WeakReferenceMessenger.Default.Send(new TaskTimeModel() { Job = job, Trigger = trigger }, Const.CONST_SCHEDULE_CREATE);        }









































        #endregion

        /// <summary>        /// 删除事件推送        /// </summary>                                                                                                                                                                                                                                                                        [RelayCommand]        public void Del()        {            WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { GUID = GUID, Operation = OperationRequestType.Delete }, Const.CONST_LISTITEM_CHANGED);        }









































        #region 时间选择器        /// <summary>                              /// 时间按钮点击后弹出时间选择窗口                              /// </summary>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               [RelayCommand]        private void SelectTaskPlan()        {            if (DateTimeGetView.IsClosed)            {                taskTime = new DateTimeGetView();                taskTime.Guid.Text = GUID;                taskTime.TitleName.Text = EditTextTitle;                taskTime.CalendarWithClock.Confirmed += SelectTaskPlanReceiver;                taskTime.NoneTime.Click += (o, e) =>                {                    NoneTaskPlanTime();                };                taskTime.Show();            }        }        private void NoneTaskPlanTime()        {            TaskTimePlan = DateTime.MinValue;
            WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { GUID = GUID, Operation = OperationRequestType.Update, Message = TaskFile }, Const.CONST_LISTITEM_CHANGED);            taskTime?.Close();        }



















































        /// <summary>        /// 接收时间选择窗口返回的时间        /// </summary>        /// <param name="sender"></param>        /// <param name="e"></param>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      private void SelectTaskPlanReceiver()        {            TaskTimePlan = DateTime.Parse(taskTime.CalendarWithClock.SelectedDateTime.ToString());            taskTime?.Close();        }





        #endregion
        #region 功能函数

        // <summary>
        // 根据readJson字段加载控件
        // </summary>
        // <param name = "readJson" ></ param >
        // < exception cref="Exception"></exception>
        public void PropertySetValue(object readJson, bool isSave = false)        {            try            {                foreach (var property in readJson.GetType().GetFields())                {                    var thisProperty = GetType().GetProperty(property.Name);                    var modelProperty = readJson.GetType().GetField(property.Name);                    if (thisProperty == null || modelProperty == null)                        throw new Exception($"{property.Name}  :  内置属性与读取到的成员不一致");                    if (thisProperty.GetValue(this) == modelProperty.GetValue(readJson)) continue;                    isInitialize = true;                    thisProperty.SetValue(this, modelProperty.GetValue(readJson));                    isInitialize = false;                }                if (isSave)                {                    SaveDataText();                }            }            catch (Exception e)            {                throw new Exception($"{JsonConvert.SerializeObject(readJson, Formatting.Indented)}转换失败：异常{e}");            }        }









































        #region 文件处理
        /// <summary>        /// 保存JSON格式的数据        /// </summary>                                                                                                                                                                                                                                                                                                                                                                 private void SaveDataText()        {            if (isInitialize) return;            isInitialize = true;            var path = Path.Combine(GlobalVariable.TaskFilePath, $"{GUID}{GlobalVariable.EndFileFormat}");            TaskTimePlan = TaskFile.TaskTimePlan;            if (IsCompleted)            {                CompletedDateTime = DateTime.Now;            }            var text = new TaskFileModel()            {                GUID = GUID,                IsCompleted = IsCompleted,                TaskTimePlan = TaskTimePlan,                CompletedDateTime = CompletedDateTime,                EditTextTitle = EditTextTitle,                EditTextText = EditTextText            };            File.WriteAllText(path, JsonConvert.SerializeObject(text, Formatting.Indented), encoding: Encoding.UTF8);            OperationRecord.OperationRecordWrite(new TcpRequestData() { GUID = GUID, OperationType = OperationRequestType.Update, SyncCategory = SyncFileCategory.TaskPlan, FileData = TaskFile });            isInitialize = false;        }































        /// <summary>        /// 读取JSON格式的Task文件        /// </summary>                                                                                                                                                                                                                                                                                                                                                                                     private void ReadFileTextData()        {            if (isInitialize) return;            var path = Path.Combine(GlobalVariable.TaskFilePath, $"{GUID}{GlobalVariable.EndFileFormat}");            if (!File.Exists(path)) return;            var text = File.ReadAllText(path);            try            {                var readJson = JsonConvert.DeserializeObject<TaskFileModel>(text);                if (readJson == null)                {                    MessageBox.Show($"错误：{GUID}中出未识别出有效JSON格式数据");                    return;                }                readJson.GUID ??= GUID;                PropertySetValue(readJson);                TaskTipTimer();   // 加入定时器
                                  //OperationRecord.OperationRecordWrite(new TcpRequestData() { GUID = guid, OperationType = OperationRequestType.Add, SyncCategory = SyncFileCategory.TaskPlan, FileData = TaskFile });
            }            catch (Exception ex)            {                MessageBox.Show($"错误：{GUID}中出现未解析字段或不完整\n{ex}");            }        }        private string TimeSpanToString(TimeSpan val)        {            var valString = val.ToString();            var result = string.Empty;            try            {                if (valString.Contains("."))                {                    result += valString.Substring(0, valString.IndexOf('.')) + "天";                    valString = valString.Substring(valString.IndexOf('.') + 1);                }                if (valString.Contains(':'))                {                    var timeString = valString.Split(':');                    result += timeString[0] + "时";                    result += timeString[1] + "分";                    result += timeString[2].Substring(0, timeString[2].IndexOf('.')) + "秒";                }                return result;            }            catch (Exception e)            {                return $"异常:{e}";            }        }





        #endregion
        #endregion

        private void InitRegister()        {            WeakReferenceMessenger.Default.Register<TaskStatusModel, string>(this, Const.CONST_TASK_STATUS_CHANGED,                (obj, msg) =>                {                    if (GUID != msg.GUID) return;                    CurrentTaskStatus = msg.Status;                    WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { GUID = GUID, Operation = OperationRequestType.Update, Message = TaskFile }, Const.CONST_LISTITEM_CHANGED);                });            WeakReferenceMessenger.Default.Register<CorrespondenceModel, string>(this, Const.CONST_TASK_STYLE_CHANGED,                (o, m) =>                {                    var temp = CurrentTaskStatus;                    CurrentTaskStatus = TaskStatusType.Default;                    CurrentTaskStatus = temp;                });        }

        /// <summary>        /// Model初始化        /// </summary>        public void InitUserControlModel()        {            VisibilityEditText = Visibility.Collapsed;            CompleteVisibility = Visibility.Collapsed;

            //CurrentTaskStatus = TaskTimePlan == DateTime.MinValue ? TaskStatus.Undefined :
            //    IsCompleted  ? TaskStatus.Complete :
            //    TaskTimePlan > DateTime.Now ? TaskStatus.Runtime: TaskStatus.Timeout;

            InitRegister();        }        public TaskListItemUserControlModel()        {            GUID = Guid.NewGuid().ToString();            InitUserControlModel();        }        public TaskListItemUserControlModel(TaskFileModel taskFile)        {            PropertySetValue(taskFile);            InitUserControlModel();        }    }}