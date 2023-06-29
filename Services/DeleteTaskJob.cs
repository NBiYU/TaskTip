using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Quartz;

namespace TaskTip.Services
{
    internal class DeleteTaskJob : IJob
    {
        public static event EventHandler DeleteMsg;
        public Task Execute(IJobExecutionContext context)
        {
            var guid = context.JobDetail.Key.ToString().Substring(context.JobDetail.Key.ToString().IndexOf("Job") + 3);
            

            DeleteMsg?.Invoke(guid, null);

            return Task.CompletedTask;
        }
    }
}