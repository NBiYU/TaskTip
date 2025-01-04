using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Themes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TaskTip.Common;
using TaskTip.Common.Helpers;
using TaskTip.Models.CommonModel;
using TaskTip.Models.ConfigModel;
using TaskTip.Models.SettingModel;
using TaskTip.Services;
using TaskTip.ViewModels.Base;
using TaskTip.Views.UserControls;

namespace TaskTip.ViewModels.PageModel
{
    public partial class SysRuntimeStatusSetPageVM : BaseVM
    {
        #region 属性

        [ObservableProperty]
        public ObservableCollection<string> _networkCardList = new();
        [ObservableProperty]
        public string _selectNetworkCard;
        [ObservableProperty]
        public ObservableCollection<ThemeSelectorUC> _themeList;
        [ObservableProperty]
        public ObservableCollection<FloatingSideModel> _actionsList;
        #endregion

        #region 指令

        [RelayCommand]
        public void GetMachineInfo()
        {
            var adapters = NetworkAdapterHelper.GetNetworkInterfaces();
            NetworkCardList = new ObservableCollection<string>(adapters);
            SelectNetworkCard = NetworkCardList.FirstOrDefault() ?? string.Empty;
        }
        #endregion

        #region 初始化

        private void Init()
        {
            var adapters = NetworkAdapterHelper.GetNetworkInterfaces();
            var config = JsonConfigHelper.ReadConfig<SysRuntimeConfigModel>(Const.RuntimeStatusConfig);
            NetworkCardList = new ObservableCollection<string>(adapters);
            SelectNetworkCard = config?.NetworkCardName;
            ThemeList = new ObservableCollection<ThemeSelectorUC>(GetThemeUC(config));
            ActionsList = new ObservableCollection<FloatingSideModel>(new List<FloatingSideModel> {
                new FloatingSideModel{
                    Key = "1",
                    IamgeUri = ((BitmapImage)Application.Current.Resources["SaveMenuRound"]).UriSource.ToString(),
                    Command = new RelayCommand<object>(Save)
                }});
        }
        private IEnumerable<ThemeSelectorUC> GetThemeUC(SysRuntimeConfigModel config)
        {
            var theme = config?.FontTheme ?? GetDefaultTheme();
            var themeControl = new List<ThemeSelectorUC>();
            var list = theme.CategoryThemes.OrderBy(x => x.ID).ToList();
            foreach (GradientColorModel item in list)
            {
                var control = new ThemeSelectorUC(item,false);
                control.ThemeSelectUCGrid.IsEnabled = theme.IsCustom;
                yield return control;
            }
        }
        private void Save(object? obj)
        {
            if (MessageBox.Show("是否保存", "Tip", MessageBoxButton.YesNo) == MessageBoxResult.No) return;

            var model = new SysRuntimeConfigModel
            {
                NetworkCardName = SelectNetworkCard,
                FontTheme = new ThemeModel
                {
                    ThemeName = "Default",
                    IsCustom = true,
                    CategoryThemes = ThemeList.Select(x => x.GradientColor).ToList()
                }
            };
            var result = JsonConfigHelper.SaveConfig(model, Const.RuntimeStatusConfig);

            MessageBox.Show($"保存{(result ? "成功" : "失败")}");
        }
        private ThemeModel GetDefaultTheme()
        {
            return new ThemeModel
            {
                ThemeName = "Default",
                IsCustom = true,
                CategoryThemes = new()
                {
                    new (){
                        ID = 0,
                        CategoryName = "CPU",
                        IsLinear = true,
                        StartX = 0.0,
                        StartY = 0.0,
                        EndX = 1.0,
                        EndY = 1.0,
                        ColorModels = new List<ColorModel>{ new() { ColorHex = "#000000", Offset = 0.0 } }
                    },
                    new (){
                        ID = 1,
                        CategoryName = "内存",
                        IsLinear = true,
                        StartX = 0.0,
                        StartY = 0.0,
                        EndX = 1.0,
                        EndY = 1.0,
                        ColorModels = new List<ColorModel>{ new() { ColorHex = "#000000", Offset = 0.0 } }

                    },
                    new (){
                        ID = 2,
                        CategoryName = "网络上传",
                        IsLinear = true,
                        StartX = 0.0,
                        StartY = 0.0,
                        EndX = 1.0,
                        EndY = 1.0,
                        ColorModels = new List<ColorModel>{ new() { ColorHex = "#000000",Offset = 0.0 } }

                    },
                    new (){
                        ID = 3,
                        CategoryName = "网络下载",
                        IsLinear = true,
                        StartX = 0.0,
                        StartY = 0.0,
                        EndX = 1.0,
                        EndY = 1.0,
                        ColorModels = new List<ColorModel>{ new() { ColorHex = "#000000",Offset = 0.0 } }
                    },
                    new ()
                    {
                        ID=4,
                        CategoryName = "背景颜色",
                        IsLinear = true,
                        StartX = 0.0,
                        StartY = 0.0,
                        EndX = 1.0,
                        EndY = 1.0,
                        ColorModels = new List<ColorModel>{ new ColorModel() { ColorHex = Colors.AliceBlue.ToString(),Offset=0.0 } }
                    }
                }
            };
        }

        #endregion

        public SysRuntimeStatusSetPageVM()
        {
            Init();
        }
    }
}
