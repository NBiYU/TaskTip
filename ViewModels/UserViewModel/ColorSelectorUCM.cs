using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using TaskTip.Common;
using TaskTip.Models;
using TaskTip.Services;
using TaskTip.Views.UserControls;
using TaskTip.Views.Windows.PopVM;

namespace TaskTip.ViewModels.UserViewModel
{
    public partial class ColorSelectorUCM : ObservableObject
    {
        private Regex numRegex = new Regex("^[0-9]+(\\.[0-9]+){0,1}$");

        private SolidColorBrush _selectBg;

        public SolidColorBrush SelectBg
        {
            get => _selectBg;
            set
            {
                if (value.ToString() == _selectBg?.ToString()) return;

                _selectBg ??= value; // 防止无法初始化

                SetProperty(ref _selectBg, value);
                SendNotify(new CorrespondenceModel() { Operation = OperationRequestType.Update });
            }
        }

        private string _colorOffset;

        public string ColorOffset
        {
            get => _colorOffset;
            set
            {
                SetProperty(ref _colorOffset, value);
                if (numRegex.IsMatch(value))
                    SendNotify(new CorrespondenceModel() { Operation = OperationRequestType.Update });
            }
        }

        private bool _colorSelectorIsOpen;

        public bool ColorSelectorIsOpen
        {
            get => _colorSelectorIsOpen;
            set => SetProperty(ref _colorSelectorIsOpen, value);
        }


        [RelayCommand]
        public void ColorSelectorShow() => ColorSelectorIsOpen = true;

        [RelayCommand]
        public void DeleteColor(object control) => WeakReferenceMessenger.Default.Send(new CorrespondenceModel()
        {
            Operation = OperationRequestType.Delete,
            Message = control
        }, Const.CONST_PREVIEW_COLOR_UPDATE);

        private void SendNotify(CorrespondenceModel corr) => WeakReferenceMessenger.Default.Send(corr, Const.CONST_PREVIEW_COLOR_UPDATE);

        public ColorSelectorUCM()
        {
            ColorOffset = "0.0";
            SelectBg = new SolidColorBrush(Colors.White);
        }
        public ColorSelectorUCM(GradientStop color)
        {
            ColorOffset = color.Offset.ToString();
            SelectBg = new SolidColorBrush(color.Color);
        }
    }
}
