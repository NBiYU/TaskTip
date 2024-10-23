using System;
using HandyControl.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Data;
using Hardcodet.Wpf.TaskbarNotification;
using TaskTip.Views;
using MessageBox = System.Windows.MessageBox;
using TaskTip.Models;
using TaskTip.Services;
using System.Reflection.Metadata;
using TaskTip.Enums;
using TaskTip.Models.DataModel;

namespace TaskTip.Common.ExecuteServices
{
    internal class TaskPlanJob : IJob
    {

        /// <summary>
        /// 到点提醒
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {

            var guid = context.JobDetail.Key.ToString().Substring(context.JobDetail.Key.ToString().IndexOf("Job") + 3);
            var path = Path.Combine(GlobalVariable.TaskFilePath, $"{guid}{GlobalVariable.EndFileFormat}");
            var errorMsg = string.Empty;

            if (!File.Exists(path))
            {
                errorMsg = $"未检索到与{guid}对应的任务内容";
            }

            var content = new TaskFileModel();
            //还有JSON文件的读取
            try
            {
                content = JsonConvert.DeserializeObject<TaskFileModel>(File.ReadAllText(path));

                if (content == null)
                {
                    errorMsg = $"{path}中并没有找到对应的JSON文件数据";
                }
            }
            catch
            {
                errorMsg = $"文件 {guid} 异常，解析失败";
            }

            Application.Current.Dispatcher.Invoke(() =>
            {

                var (title, msg, icon) = string.IsNullOrEmpty(errorMsg)
                    ? (content.EditTextTitle, content.EditTextText, BalloonIcon.Info)
                    : ("处理出现异常", errorMsg, BalloonIcon.Error);


                var taskBalloonTip = new TaskbarIcon();
                taskBalloonTip.ShowBalloonTip(title, msg, icon);

                WeakReferenceMessenger.Default.Send(
                    new TaskStatusModel()
                    {
                        GUID = guid,
                        Status = TaskStatusType.Timeout
                    }, Const.CONST_TASK_STATUS_CHANGED);
            });


            return Task.CompletedTask;
        }

    }



}
