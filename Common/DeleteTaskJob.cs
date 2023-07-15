using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using CommunityToolkit.Mvvm.Messaging;
using Quartz;

namespace TaskTip.Services
{
    internal class DeleteTaskJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var guid = context.JobDetail.Key.ToString().Substring(context.JobDetail.Key.ToString().IndexOf("Job") + 3);

            WeakReferenceMessenger.Default.Send(guid, Const.CONST_DELETE_LISTITEM);

            return Task.CompletedTask;
        }
    }
}