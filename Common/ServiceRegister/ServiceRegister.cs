using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTip.Common.ExecuteServices;

namespace TaskTip.Common.ServiceRegister
{
    internal class ServiceRegister
    {
        public static IContainer InitRegister()
        {
            var build = new ContainerBuilder();
            build.RegisterType<TcpListenerService>().As<IHostedService>().SingleInstance();

            return build.Build();
        }
    }
}
