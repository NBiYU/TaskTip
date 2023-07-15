using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Text;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using TaskTip.Models;
using TaskTip.Services;
using TaskTip.Views;
using MessageBox = System.Windows.MessageBox;

namespace TaskTip.ViewModels
{

    internal class TaskListItemUserControlModel : ObservableRecipient
    {
        /// <summary>
        /// 是否允许更改属性时保存
        /// </summary>
        private bool isInitialize = true;
        /// <summary>
        /// 时间选择器
        /// </summary>
        private DateTimeGetView taskTime;


        #region 个人属性

        /// <summary>
        /// 唯一标识GUID码
        /// </summary>
        private string guid;
        public string GUID
        {
            get => guid;
            set
            {
                SetProperty(ref guid, value);
                ReadFileTextData();
            }
        }


        /// <summary>
        /// 是否完成，勾选框属性
        /// </summary>
        private bool isCompleted;
        public bool IsCompleted
        {
            get => isCompleted;
            set
            {
                SetProperty(ref isCompleted, value);

                CompleteVisibility = isCompleted == true ? Visibility.Visible : Visibility.Collapsed;
                if (isCompleted == true)
                    CompletedDateTime = DateTime.Now;

                //IsCompleteMsg?.Invoke(Index,null);
                IsEnableControl = isCompleted == false;

                SaveDataText();
            }
        }


        private string trashImagePath;
        public string TrashImagePath
        {
            get => trashImagePath;
            set => SetProperty(ref trashImagePath, value);
        }

        /// <summary>
        /// 控件是否可用
        /// </summary>
        private bool isEnableControl = true;
        public bool IsEnableControl
        {
            get => isEnableControl;
            set => SetProperty(ref isEnableControl, value);
        }

        /// <summary>
        /// 划线是否显示
        /// </summary>
        private Visibility completedVisibility;
        public Visibility CompleteVisibility
        {
            get => completedVisibility;
            set => SetProperty(ref completedVisibility, value);
        }


        /// <summary>
        /// 任务标题
        /// </summary>
        private string editTextTitle;
        public string EditTextTitle
        {
            get => editTextTitle;
            set
            {
                SetProperty(ref editTextTitle, value);
                SaveDataText();
            }
        }

        /// <summary>
        /// 任务详细
        /// </summary>
        private string editTextText;
        public string EditTextText
        {
            get => editTextText;

            set
            {
                SetProperty(ref editTextText, value);
                SaveDataText();
            }
        }

        /// <summary>
        /// 任务详情是否展开
        /// </summary>
        private Visibility visibilityEditText;
        public Visibility VisibilityEditText
        {
            get => visibilityEditText;
            set
            {
                SetProperty(ref visibilityEditText, value);
            }
        }


        /// <summary>
        /// 任务提醒时间
        /// </summary>
        private DateTime taskTimePlan;
        public DateTime TaskTimePlan
        {
            get => taskTimePlan;
            set
            {
                SetProperty(ref taskTimePlan, value);
                ToolTaskTime = taskTimePlan.ToString("yyyy-MM-dd HH:mm:ss");

                CurrentTaskStatus = TaskTimePlan == DateTime.MinValue ? TaskStatus.Undefined :
                    IsCompleted ? TaskStatus.Complete :
                    TaskTimePlan > DateTime.Now ? TaskStatus.Runtime : TaskStatus.Timeout;

                SaveDataText();
                TaskTipTimer();
                TaskDeleteTimer();
            }
        }

        /// <summary>
        /// 提示字符
        /// </summary>
        private string toolTaskTime;

        public string ToolTaskTime
        {
            get => toolTaskTime;
            set => SetProperty(ref toolTaskTime,
                "计划 " + value + "完成");

        }

        private TaskStatus _currentTaskStatus;
        public TaskStatus CurrentTaskStatus
        {
            get => _currentTaskStatus;
            set => SetProperty(ref _currentTaskStatus, value);
        }

#pragma warning disable CS0169 // 从不使用字段“TaskListItemUserControlModel.completedDateTime”
        private DateTime completedDateTime;
#pragma warning restore CS0169 // 从不使用字段“TaskListItemUserControlModel.completedDateTime”
        public DateTime CompletedDateTime { get; set; }

        #endregion



        #region 控件命令

        /// <summary>
        /// 删除Item命令
        /// </summary>
        public RelayCommand DelCommand { get; set; }

        /// <summary>
        /// 时间计划选择按钮
        /// </summary>
        public RelayCommand SelectTaskPlanCommand { get; set; }




        #region 定时任务
        /// <summary>
        /// 当选择的时间发生改变时,生成一个定时提醒作业发给调度器
        /// </summary>
        private void TaskTipTimer()
        {
            if ((TaskTimePlan - DateTime.Now).TotalMilliseconds < 500)
            {
                return;
            }


            if ((TaskTimePlan - DateTime.Now).TotalDays >= GlobalVariable.DeleteTimes)
            {
                MessageBox.Show("计划时间于或等于定时删除时间，请调整");
                return;
            }

            if (!File.Exists($@"{GlobalVariable.TaskFilePath}\{GUID}.task"))
            {
                return;
            }

            IJobDetail job = JobBuilder.Create<TaskPlanJob>()
                .WithIdentity($"TipJob{GUID}", $"Group{GUID}")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity($"TipTrigger{GUID}", $"Group{GUID}")
                .StartAt(TaskTimePlan)
                .Build();

            WeakReferenceMessenger.Default.Send(new { Job = job, Trigger = trigger }, Const.CONST_SCHEDULE_CREATE);

        }


        private void TaskDeleteTimer()
        {
            if (!File.Exists($@"{GlobalVariable.TaskFilePath}\{GUID}.task") && !GlobalVariable.IsEnableAutoDelete)
            {
                return;
            }

            IJobDetail job = JobBuilder.Create<DeleteTaskJob>()
                .WithIdentity($"DeleteJob{GUID}", $"Group{GUID}")
                .Build();

            ITrigger trigger = TriggerBuilder.Create().WithIdentity($"DeleteTrigger{GUID}")
                .StartAt(TaskTimePlan.AddDays(GlobalVariable.DeleteTimes) < DateTime.Now
                    ? DateTimeOffset.Now.AddMinutes(1) : TaskTimePlan.AddDays(GlobalVariable.DeleteTimes)).Build();

            WeakReferenceMessenger.Default.Send(new { Job = job, Trigger = trigger }, Const.CONST_SCHEDULE_CREATE);
        }

        #endregion


        #endregion

        /// <summary>
        /// 删除事件推送
        /// </summary>
        public void DeleteSendMsg()
        {
            WeakReferenceMessenger.Default.Send(GUID, Const.CONST_DELETE_LISTITEM);
        }



        #region 时间选择器
        /// <summary>
        /// 时间按钮点击后弹出时间选择窗口
        /// </summary>
        private void SelectTaskPlanHandler()
        {
            taskTime = new DateTimeGetView();
            taskTime.Guid.Text = GUID;
            taskTime.CalendarWithClock.Confirmed += SelectTaskPlanReceiver;
            taskTime.Show();
        }

        /// <summary>
        /// 接收时间选择窗口返回的时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectTaskPlanReceiver()
        {
            //var recevie = taskTime.CalendarWithClock.SelectedDateTime.ToString().Split(';');
            //if (GUID == recevie[0])
            //{
            //    TaskTimePlan = DateTime.Parse(recevie[1]);
            //}

            TaskTimePlan = DateTime.Parse(taskTime.CalendarWithClock.SelectedDateTime.ToString());


            taskTime?.Close();
        }

        #endregion

        #region 功能函数

        private void PropertySetValue(string propertyName, object model)
        {
            try
            {
                var thisProperty = this.GetType().GetProperty(propertyName);
                var modelProperty = model.GetType().GetProperty(propertyName);
                if (thisProperty == null || modelProperty == null)
                    throw new Exception($"{propertyName}  :  内置属性与读取到的属性不一致");

                thisProperty.SetValue(this, modelProperty.GetValue(model));
            }
            catch (Exception e)
            {
                throw new Exception($"{propertyName}转换失败：异常{e}");
            }
        }

        #region 文件处理

        /// <summary>
        /// 保存JSON格式的数据
        /// </summary>
        private void SaveDataText()
        {
            if (isInitialize)
                return;
            isInitialize = true;
            var path = GlobalVariable.TaskFilePath + "\\" + GUID + GlobalVariable.EndFileFormat;
            TaskTimePlan = taskTimePlan;


            if (IsCompleted)
            {
                CompletedDateTime = DateTime.Now;
            }

            var text = new TaskFileModel()
            {
                IsCompleted = IsCompleted,
                TaskTimePlan = TaskTimePlan,
                CompletedDateTime = CompletedDateTime,
                EditTextTitle = EditTextTitle,
                EditTextText = EditTextText
            };


            File.WriteAllText(path, JsonConvert.SerializeObject(text), encoding: Encoding.UTF8);

            isInitialize = false;
        }

        /// <summary>
        /// 读取JSON格式的Task文件
        /// </summary>
        private void ReadFileTextData()
        {
            var path = GlobalVariable.TaskFilePath + "\\" + GUID + GlobalVariable.EndFileFormat;

            if (File.Exists(path))
            {
                var text = File.ReadAllText(path);


                try
                {
                    var readJson = JsonConvert.DeserializeObject<TaskFileModel>(text);

                    if (readJson == null)
                    {
                        MessageBox.Show($"错误：{GUID}中出未识别出有效JSON格式数据");
                        return;
                    }

                    isInitialize = true;
                    foreach (var property in readJson.GetType().GetProperties())
                    {
                        PropertySetValue(property.Name, readJson);
                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show($"错误：{GUID}中出现未解析字段或不完整\n{ex}");
                }
            }

            isInitialize = false;
        }

        private string TimeSpanToString(TimeSpan val)
        {
            var valString = val.ToString();
            var result = string.Empty;
            try
            {
                if (valString.Contains("."))
                {
                    result += valString.Substring(0, valString.IndexOf('.')) + "天";
                    valString = valString.Substring(valString.IndexOf('.') + 1);
                }

                if (valString.Contains(':'))
                {
                    var timeString = valString.Split(':');
                    result += timeString[0] + "时";
                    result += timeString[1] + "分";
                    result += timeString[2].Substring(0, timeString[2].IndexOf('.')) + "秒";
                }

                return result;
            }
            catch (Exception e)
            {
                return $"异常:{e}";
            }
        }


        #endregion
        #endregion


        private void InitRegister()
        {
            //WeakReferenceMessenger.Default.Register<string,string>(this,Const.CONST_DATETIME_RETURN,
            //    (obj, msg) => { SelectTaskPlanReceiver(msg);});
        }

        /// <summary>
        /// Model初始化
        /// </summary>
        public void InitUserControlModel()
        {

            VisibilityEditText = Visibility.Collapsed;
            CompleteVisibility = Visibility.Collapsed;

            //CurrentTaskStatus = TaskTimePlan == DateTime.MinValue ? TaskStatus.Undefined :
            //    IsCompleted  ? TaskStatus.Complete :
            //    TaskTimePlan > DateTime.Now ? TaskStatus.Runtime: TaskStatus.Timeout;

            DelCommand = new RelayCommand(DeleteSendMsg);
            SelectTaskPlanCommand = new RelayCommand(SelectTaskPlanHandler);
            InitRegister();
        }

        public TaskListItemUserControlModel()
        {
            GUID = Guid.NewGuid().ToString();

            InitUserControlModel();
        }

        public TaskListItemUserControlModel(string guid)
        {
            GUID = guid;
            InitUserControlModel();
        }
    }

    public enum TaskStatus
    {
        [Description("完成")]
        Complete,
        [Description("超时")]
        Timeout,
        [Description("执行中")]
        Runtime,
        [Description("未定义")]
        Undefined
    }
}
