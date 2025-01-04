using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

using TaskTip.Common;
using TaskTip.Pages;
using TaskTip.Services;

namespace TaskTip.ViewModels.WindowModel
{
    internal class EditFullScreenViewModel : ObservableObject
    {
        public Page FrameRecordPage
        {
            get => WindowResource.RecordPage;
        }

        public EditFullScreenViewModel()
        {

        }
    }
}
