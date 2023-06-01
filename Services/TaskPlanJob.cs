using System;
using HandyControl.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using HandyControl.Data;
using TaskTip.Views;
using MessageBox = System.Windows.MessageBox;

namespace TaskTip.Services
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

            var path = ConfigurationManager.AppSettings["TaskFilePath"] + "\\" + guid + ConfigurationManager.AppSettings["EndFileFormat"];

            if (!File.Exists(path))
            {
                Growl.ErrorGlobal($"未检索到与{guid}对应的任务内容");
                return Task.CompletedTask;
            }

            //还有JSON文件的读取

            JObject json = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(path));

            if (json == null && json.GetValue("EditTextTitle") == null)
            {
                MessageBox.Show($"【log】{path}中并没有找到对应的JSON文件数据");
                return Task.CompletedTask;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                Notification.Show(new TaskTipView(json.GetValue("EditTextTitle")?.ToString(),
                    json.GetValue("EditTextText")?.ToString(),
                    DateTime.Parse(json.GetValue("TaskTimePlan")?.ToString())), ShowAnimation.HorizontalMove);
            });

            //Growl.InfoGlobal(json.GetValue("EditTextTitle")?.ToString());

            return Task.CompletedTask;
        }

    }

    public enum TaskPlanJobType
    {
        [Description("错误提示")]
        Error,

        [Description("异常提示")]
        Exception,

        [Description("警告提示")]
        Warning,

        [Description("信息提示")]
        Message

    }

}
