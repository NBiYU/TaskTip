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
using TaskTip.Models.DataModel;
using TaskTip.Models.Entities;
using TaskTip.Common.Helpers;
using TaskTip.Common.Converter.Map;
using TaskTip.Models.Enums;
using TaskTip.Models.ViewDataModels;

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
            //var path = Path.Combine(GlobalVariable.TaskFilePath, $"{guid}{GlobalVariable.EndFileFormat}");
            var errorMsg = string.Empty;

            var content = new TaskFileModel();
            //还有JSON文件的读取
            try
            {
                var db = new SQLiteDB();
                content = db.GetTaskListByGuid(guid)?.Entity2TaskModel();

                if (content == null)
                {
                    errorMsg = $"未找到{guid}的数据";
                }
            }
            catch
            {
                errorMsg = $"{guid} 异常，获取数据失败";
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
