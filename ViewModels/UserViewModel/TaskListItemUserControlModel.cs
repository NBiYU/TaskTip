using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.IO;
using System.Text;
using System.Windows;
using TaskTip.Models;
using TaskTip.Services;
using TaskTip.Views;
using TaskTip.Common;
using MessageBox = System.Windows.MessageBox;
using TaskTip.Common.ExecuteServices;
using TaskTip.Models.DataModel;
using TaskTip.Models.CommonModel;
using TaskTip.Models.Entities;
using TaskTip.Models.Enums;

namespace TaskTip.ViewModels.UserViewModel
{

    internal partial class TaskListItemUserControlModel : ObservableObject
    {






























        /// <summary>
        /// �Ƿ������������ʱ����
        /// </summary>
                                                                                                                                                                                                                                                                                            public bool isInitialize = false;






























        /// <summary>
        /// ʱ��ѡ����
        /// </summary>
                                                                                                                                                                                                                                                                    private DateTimeGetView taskTime;

        public TaskFileModel TaskFile { get; set; } = new();









































        #region ��������

        /// <summary>
        /// Ψһ��ʶGUID��
        /// </summary>
        public string GUID
        {
            get => TaskFile.GUID;
            set
            {
                //SetProperty(ref TaskFile.GUID, value);
                ReadFileTextData();
            }
        }
































        /// <summary>
        /// �Ƿ���ɣ���ѡ������
        /// </summary>
                                                                                                                                                                                                                                                                                        public bool IsCompleted
        {
            get => TaskFile.IsCompleted;
            set
            {
                //SetProperty(ref TaskFile.IsCompleted, value);

                CompleteVisibility = TaskFile.IsCompleted == true ? Visibility.Visible : Visibility.Collapsed;
                if (TaskFile.IsCompleted == true)
                    CompletedDateTime = DateTime.Now;
                if (!isInitialize)
                    //WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { GUID = GUID, Operation = OperationRequestType.Update, Message = TaskFile }, Const.CONST_LISTITEM_CHANGED);
                IsEnableControl = TaskFile.IsCompleted == false;

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
        /// �ؼ��Ƿ����
        /// </summary>
                                                                                                                                                                                                                                                                        private bool isEnableControl = true;
        public bool IsEnableControl
        {
            get => isEnableControl;
            set => SetProperty(ref isEnableControl, value);
        }































        /// <summary>
        /// �����Ƿ���ʾ
        /// </summary>
                                                                                                                                                                                                                                                                        private Visibility completedVisibility;
        public Visibility CompleteVisibility
        {
            get => completedVisibility;
            set => SetProperty(ref completedVisibility, value);
        }
































        /// <summary>
        /// �������
        /// </summary>
                                                                                                                                                                                                                                                                public string EditTextTitle
        {
            get => TaskFile.EditTextTitle;
            set
            {
                //SetProperty(ref TaskFile.EditTextTitle, value);
                SaveDataText();
            }
        }































        /// <summary>
        /// ������ϸ
        /// </summary>
                                                                                                                                                                                                                                                                public string EditTextText
        {
            get => TaskFile.EditTextText;
            set
            {
                //SetProperty(ref TaskFile.EditTextText, value);
                SaveDataText();
            }
        }































        /// <summary>
        /// ���������Ƿ�չ��
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
        /// ��������ʱ��
        /// </summary>
                                                                                                                                                                                                                                                                        public DateTime TaskTimePlan
        {
            get => TaskFile.TaskTimePlan;
            set
            {
                //SetProperty(ref TaskFile.TaskTimePlan, value);
                ToolTaskTime = TaskFile.TaskTimePlan.ToString("yyyy-MM-dd HH:mm:ss");

                CurrentTaskStatus = TaskTimePlan == DateTime.MinValue ? TaskStatusType.Undefined :
                    IsCompleted ? TaskStatusType.Complete :
                    TaskTimePlan > DateTime.Now ? TaskStatusType.Runtime : TaskStatusType.Timeout;

                SaveDataText();
                TaskTipTimer();
                TaskDeleteTimer();
            }
        }


























        /// <summary>
        /// ��ʾ�ַ�
        /// </summary>
                                                                                                                                                                                                                                                                private string toolTaskTime;

        public string ToolTaskTime
        {
            get => toolTaskTime;
            set => SetProperty(ref toolTaskTime, DateTime.Parse(value) == DateTime.MinValue ? "��ǰ�޼ƻ��¼�" : "�ƻ� " + value + "���");

        }

        private TaskStatusType _currentTaskStatus;
        public TaskStatusType CurrentTaskStatus
        {
            get => _currentTaskStatus;
            set => SetProperty(ref _currentTaskStatus, value);
        }

        public DateTime CompletedDateTime { get; set; }








































        #endregion

        #region ��ʱ����
        /// <summary>
                             /// ��ѡ���ʱ�䷢���ı�ʱ,����һ����ʱ������ҵ����������
                             /// </summary>
        private void TaskTipTimer()
        {
            if ((TaskTimePlan - DateTime.Now).TotalMilliseconds < 500)
            {
                return;
            }


            if ((TaskTimePlan - DateTime.Now).TotalDays >= GlobalVariable.DeleteTimes)
            {
                //MessageBox.Show("�ƻ�ʱ���ڻ���ڶ�ʱɾ��ʱ�䣬�����");
                return;
            }

            if (!File.Exists(Path.Combine(GlobalVariable.TaskFilePath, $"{GUID}{GlobalVariable.EndFileFormat}")))
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

            //WeakReferenceMessenger.Default.Send(new TaskTimeModel() { Job = job, Trigger = trigger }, Const.CONST_SCHEDULE_CREATE);

        }


        private void TaskDeleteTimer()
        {
            if (!(File.Exists(Path.Combine(GlobalVariable.TaskFilePath, $"{GUID}{GlobalVariable.EndFileFormat}")) && GlobalVariable.IsEnableAutoDelete))
            {
                return;
            }

            IJobDetail job = JobBuilder.Create<DeleteTaskJob>()
                .WithIdentity($"DeleteJob{GUID}", $"Group{GUID}")
                .Build();

            ITrigger trigger = TriggerBuilder.Create().WithIdentity($"DeleteTrigger{GUID}")
                .StartAt(TaskTimePlan.AddDays(GlobalVariable.DeleteTimes) < DateTime.Now
                    ? DateTimeOffset.Now.AddMinutes(1) : TaskTimePlan.AddDays(GlobalVariable.DeleteTimes)).Build();

            //WeakReferenceMessenger.Default.Send(new TaskTimeModel() { Job = job, Trigger = trigger }, Const.CONST_SCHEDULE_CREATE);
        }









































        #endregion


        /// <summary>
        /// ɾ���¼�����
        /// </summary>
                                                                                                                                                                                                                                                                        [RelayCommand]
        public void Del()
        {
            //WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { GUID = GUID, Operation = OperationRequestType.Delete }, Const.CONST_LISTITEM_CHANGED);
        }









































        #region ʱ��ѡ����
        /// <summary>
                              /// ʱ�䰴ť����󵯳�ʱ��ѡ�񴰿�
                              /// </summary>
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               [RelayCommand]
        private void SelectTaskPlan()
        {
            if (DateTimeGetView.IsClosed)
            {
                taskTime = new DateTimeGetView();
                taskTime.Guid.Text = GUID;
                taskTime.TitleName.Text = EditTextTitle;
                taskTime.CalendarWithClock.Confirmed += SelectTaskPlanReceiver;
                taskTime.NoneTime.Click += (o, e) =>
                {
                    NoneTaskPlanTime();
                };
                taskTime.Show();
            }
        }


        private void NoneTaskPlanTime()
        {
            TaskTimePlan = DateTime.MinValue;
            //WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { GUID = GUID, Operation = OperationRequestType.Update, Message = TaskFile }, Const.CONST_LISTITEM_CHANGED);
            taskTime?.Close();
        }



















































        /// <summary>
        /// ����ʱ��ѡ�񴰿ڷ��ص�ʱ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      private void SelectTaskPlanReceiver()
        {
            TaskTimePlan = DateTime.Parse(taskTime.CalendarWithClock.SelectedDateTime.ToString());
            taskTime?.Close();
        }





        #endregion

        #region ���ܺ���


        // <summary>
        // ����readJson�ֶμ��ؿؼ�
        // </summary>
        // <param name = "readJson" ></ param >
        // < exception cref="Exception"></exception>
        public void PropertySetValue(object readJson, bool isSave = false)
        {
            try
            {
                foreach (var property in readJson.GetType().GetFields())
                {
                    var thisProperty = GetType().GetProperty(property.Name);
                    var modelProperty = readJson.GetType().GetField(property.Name);

                    if (thisProperty == null || modelProperty == null)
                        throw new Exception($"{property.Name}  :  �����������ȡ���ĳ�Ա��һ��");

                    if (thisProperty.GetValue(this) == modelProperty.GetValue(readJson)) continue;

                    isInitialize = true;
                    thisProperty.SetValue(this, modelProperty.GetValue(readJson));
                    isInitialize = false;
                }

                if (isSave)
                {
                    SaveDataText();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"{JsonConvert.SerializeObject(readJson, Formatting.Indented)}ת��ʧ�ܣ��쳣{e}");
            }

        }









































        #region �ļ�����

        /// <summary>
        /// ����JSON��ʽ������
        /// </summary>
                                                                                                                                                                                                                                                                                                                                                                 private void SaveDataText()
        {
            if (isInitialize) return;
            isInitialize = true;

            var path = Path.Combine(GlobalVariable.TaskFilePath, $"{GUID}{GlobalVariable.EndFileFormat}");
            TaskTimePlan = TaskFile.TaskTimePlan;


            if (IsCompleted)
            {
                CompletedDateTime = DateTime.Now;
            }

            var text = new TaskFileModel()
            {
                GUID = GUID,
                IsCompleted = IsCompleted,
                TaskTimePlan = TaskTimePlan,
                CompletedDateTime = CompletedDateTime,
                EditTextTitle = EditTextTitle,
                EditTextText = EditTextText
            };


            File.WriteAllText(path, JsonConvert.SerializeObject(text, Formatting.Indented), encoding: Encoding.UTF8);
            OperationRecord.OperationRecordWrite(new TcpRequestData() { GUID = GUID, OperationType = OperationRequestType.Update, SyncCategory = SyncFileCategory.TaskPlan, FileData = TaskFile });
            isInitialize = false;
        }































        /// <summary>
        /// ��ȡJSON��ʽ��Task�ļ�
        /// </summary>
                                                                                                                                                                                                                                                                                                                                                                                     private void ReadFileTextData()
        {
            if (isInitialize) return;

            var path = Path.Combine(GlobalVariable.TaskFilePath, $"{GUID}{GlobalVariable.EndFileFormat}");

            if (!File.Exists(path)) return;

            var text = File.ReadAllText(path);

            try
            {
                var readJson = JsonConvert.DeserializeObject<TaskFileModel>(text);
                if (readJson == null)
                {
                    MessageBox.Show($"����{GUID}�г�δʶ�����ЧJSON��ʽ����");
                    return;
                }

                readJson.GUID ??= GUID;

                PropertySetValue(readJson);
                TaskTipTimer();   // ���붨ʱ��
                                  //OperationRecord.OperationRecordWrite(new TcpRequestData() { GUID = guid, OperationType = OperationRequestType.Add, SyncCategory = SyncFileCategory.TaskPlan, FileData = TaskFile });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"����{GUID}�г���δ�����ֶλ�����\n{ex}");
            }
        }

        private string TimeSpanToString(TimeSpan val)
        {
            var valString = val.ToString();
            var result = string.Empty;
            try
            {
                if (valString.Contains("."))
                {
                    result += valString.Substring(0, valString.IndexOf('.')) + "��";
                    valString = valString.Substring(valString.IndexOf('.') + 1);
                }

                if (valString.Contains(':'))
                {
                    var timeString = valString.Split(':');
                    result += timeString[0] + "ʱ";
                    result += timeString[1] + "��";
                    result += timeString[2].Substring(0, timeString[2].IndexOf('.')) + "��";
                }

                return result;
            }
            catch (Exception e)
            {
                return $"�쳣:{e}";
            }
        }





        #endregion

        #endregion


        private void InitRegister()
        {
            WeakReferenceMessenger.Default.Register<TaskStatusModel, string>(this, Const.CONST_TASK_STATUS_CHANGED,
                (obj, msg) =>
                {
                    if (GUID != msg.GUID) return;
                    CurrentTaskStatus = msg.Status;
                    //WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { GUID = GUID, Operation = OperationRequestType.Update, Message = TaskFile }, Const.CONST_LISTITEM_CHANGED);
                });
            WeakReferenceMessenger.Default.Register<CorrespondenceModel, string>(this, Const.CONST_TASK_STYLE_CHANGED,
                (o, m) =>
                {
                    var temp = CurrentTaskStatus;
                    CurrentTaskStatus = TaskStatusType.Default;
                    CurrentTaskStatus = temp;
                });
        }

        /// <summary>
        /// Model��ʼ��
        /// </summary>
        public void InitUserControlModel()
        {

            VisibilityEditText = Visibility.Collapsed;
            CompleteVisibility = Visibility.Collapsed;

            //CurrentTaskStatus = TaskTimePlan == DateTime.MinValue ? TaskStatus.Undefined :
            //    IsCompleted  ? TaskStatus.Complete :
            //    TaskTimePlan > DateTime.Now ? TaskStatus.Runtime: TaskStatus.Timeout;

            InitRegister();
        }

        public TaskListItemUserControlModel()
        {
            GUID = Guid.NewGuid().ToString();

            InitUserControlModel();
        }

        public TaskListItemUserControlModel(TaskFileModel taskFile)
        {
            PropertySetValue(taskFile);
            InitUserControlModel();
        }

    }


}
