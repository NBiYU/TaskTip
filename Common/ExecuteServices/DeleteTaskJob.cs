using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using CommunityToolkit.Mvvm.Messaging;
using Quartz;
using TaskTip.Models;
using TaskTip.Services;

namespace TaskTip.Common.ExecuteServices
{
    internal class DeleteTaskJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var guid = context.JobDetail.Key.ToString().Substring(context.JobDetail.Key.ToString().IndexOf("Job") + 3);

            WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { GUID = guid, Operation = OperationRequestType.Delete }, Const.CONST_LISTITEM_CHANGED);

            return Task.CompletedTask;
        }
    }
}