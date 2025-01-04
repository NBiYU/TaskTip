using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualBasic;

using TaskTip.Common;
using TaskTip.Models.DataModel;
using TaskTip.Models.Enums;
using TaskTip.Models.SettingModel;
using TaskTip.Services;
using TaskTip.ViewModels.Converters;
using TaskTip.ViewModels.UserViewModel;
using TaskTip.Views.UserControls;

namespace TaskTip.ViewModels.WindowModel.PopVM
{
    internal partial class GradientSelectorVM : ObservableObject
    {

        private bool IsInit = true;
        private Regex numRegex = new Regex("^[0-9]+(\\.[0-9]+){0,1}$");

        #region 起终点XY

        private string _startPointX;

        public string StartPointX
        {
            get => _startPointX;
            set
            {
                SetProperty(ref _startPointX, value);
                RectangleChanged();
            }
        }

        private string _startPointY;

        public string StartPointY
        {
            get => _startPointY;
            set
            {
                SetProperty(ref _startPointY, value);
                RectangleChanged();
            }
        }

        private string _endPointX;

        public string EndPointX
        {
            get => _endPointX;
            set
            {
                SetProperty(ref _endPointX, value);

                RectangleChanged();


            }
        }

        private string _endPointY;

        public string EndPointY
        {
            get => _endPointY;
            set
            {
                SetProperty(ref _endPointY, value);
                RectangleChanged();
            }
        }

        #endregion

        private bool _isLinear;

        public bool IsLinear
        {
            get => _isLinear;
            set
            {
                SetProperty(ref _isLinear, value);
                RectangleChanged();
            }
        }

        private ObservableCollection<ColorSelectorUC> _gradientColors;

        public ObservableCollection<ColorSelectorUC> GradientColors
        {
            get => _gradientColors;
            set
            {
                SetProperty(ref _gradientColors, value);
                RectangleChanged();
            }
        }

        private GradientBrush _previewRectangle;

        public GradientBrush PreviewRectangle
        {
            get => _previewRectangle;
            set => SetProperty(ref _previewRectangle, value);
        }

        [RelayCommand]
        public Task AddUCItem()
        {
            GradientColors.Add(AddItem());
            return Task.CompletedTask;
        }


        private ColorSelectorUC AddItem()
        {
            return new ColorSelectorUC();
        }

        private ColorSelectorUC AddItem(ColorModel colorModel)
        {
            var item = AddItem();
            var color = new GradientStop((Color)ColorConverter.ConvertFromString(colorModel.ColorHex), colorModel.Offset);
            item.DataContext = new ColorSelectorUCM(color);
            return item;
        }

        private void RectangleChanged()
        {
            if (IsInit) return;
            if (!numRegex.IsMatch(StartPointX)) return;
            if (!numRegex.IsMatch(StartPointY)) return;
            if (!numRegex.IsMatch(EndPointX)) return;
            if (!numRegex.IsMatch(EndPointY)) return;

            var colors = new GradientStopCollection();
            var startPoint = new Point(double.Parse(StartPointX), double.Parse(StartPointY));
            var endPoint = new Point(double.Parse(EndPointX), double.Parse(EndPointY));

            foreach (var item in GradientColors)
            {
                if (item.DataContext is ColorSelectorUCM vm)
                {
                    colors.Add(new GradientStop(vm.SelectBg.Color, double.Parse(vm.ColorOffset)));
                }
            }

            if (colors.Count == 0) colors.Add(new GradientStop(Colors.White, 1));


            PreviewRectangle = IsLinear
                ? new LinearGradientBrush(colors, startPoint, endPoint)
                : new RadialGradientBrush(colors);

        }

        private void Init()
        {
            IsInit = true;
            StartPointX = "0.0";
            StartPointY = "0.0";
            EndPointX = "0.0";
            EndPointY = "0.0";
            GradientColors = new ObservableCollection<ColorSelectorUC>();

            InitRegister();

            IsInit = false;
        }

        private void InitRegister()
        {
            WeakReferenceMessenger.Default.Register<CorrespondenceModel, string>(this, Const.CONST_PREVIEW_COLOR_UPDATE,
                (o, m) =>
                {
                    switch (m.Operation)
                    {
                        case OperationRequestType.Delete:
                            GradientColors.Remove((ColorSelectorUC)m.Message);
                            RectangleChanged();
                            break;
                        case OperationRequestType.Update:
                            RectangleChanged();
                            break;
                    }

                });
        }

        public GradientSelectorVM()
        {
            Init();
        }

        public GradientSelectorVM(GradientColorModel model)
        {
            Init();
            IsLinear = model.IsLinear;
            foreach (var item in model.ColorModels)
            {
                GradientColors.Add(AddItem(item));
            }

            StartPointX = model.StartX.ToString();
            StartPointY = model.StartY.ToString();
            EndPointX = model.EndX.ToString();
            EndPointY = model.EndY.ToString();

        }
    }
}
