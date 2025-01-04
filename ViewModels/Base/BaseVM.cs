using CommunityToolkit.Mvvm.ComponentModel;

using NLog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using TaskTip.Models.Enums;
using TaskTip.Services;

namespace TaskTip.ViewModels.Base
{
    public class BaseVM : ObservableObject
    {
        private Logger LoggerHelper = GlobalVariable.LogHelper;
        protected string VMName;

        public override string ToString()
        {
            return $"{VMName}";
        }

        public void ExecuteLogger(string message, bool isShowBox = false, LogType logType = LogType.Info)
        {
            switch (logType)
            {
                case LogType.Info:
                    LoggerHelper.Info($"{message}");
                    break;
                case LogType.Debug:
                    LoggerHelper.Debug($"{message}");
                    break;
                case LogType.Warning:
                    LoggerHelper.Warn($"{message}");
                    break;
                case LogType.Error:
                    LoggerHelper.Error($"{message}");
                    break;
            }

            if (isShowBox)
            {
                MessageBox.Show(message);
            }
        }
    }
}
