using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Media;
using TaskTip.Properties;
using TaskTip.Services;
using MessageBox = System.Windows.MessageBox;

namespace TaskTip.ViewModels
{

    internal class FloatingViewModel : ObservableObject
    {
#pragma warning disable CS0649 // 从未对字段“FloatingViewModel.ClickEventHandler”赋值，字段将一直保持其默认值 null
        public EventHandler ClickEventHandler;
#pragma warning restore CS0649 // 从未对字段“FloatingViewModel.ClickEventHandler”赋值，字段将一直保持其默认值 null


        #region 悬浮窗属性
        /// <summary>
        /// 悬浮窗宽
        /// </summary>
        private double floatingSetWidth;
        public double FloatingSetWidth
        {
            get => floatingSetWidth;
            set => SetProperty(ref floatingSetWidth, value);
        }
        /// <summary>
        /// 悬浮窗高
        /// </summary>
        private double floatingSetHeight;
        public double FloatingSetHeight
        {
            get => floatingSetHeight;
            set => SetProperty(ref floatingSetHeight, value);
        }

#pragma warning disable CS0649 // 从未对字段“FloatingViewModel.radiusXValue”赋值，字段将一直保持其默认值 0
        private double radiusXValue;
#pragma warning restore CS0649 // 从未对字段“FloatingViewModel.radiusXValue”赋值，字段将一直保持其默认值 0
        /// <summary>
        /// 矩形起始点X轴
        /// </summary>
        public double RadiusXValue
        {
            get => radiusXValue;
            set => SetProperty(ref radiusYValue, value);
        }

        private double radiusYValue;
        /// <summary>
        /// 矩形起始点Y轴
        /// </summary>
        public double RadiusYValue
        {
            get => radiusYValue;
            set => SetProperty(ref radiusYValue, value);
        }

        /// <summary>
        /// 矩形形状
        /// </summary>
        private Rect rectangleGeometryValue;
        public Rect RectangleGeometryValue
        {
            get => rectangleGeometryValue;
            set => SetProperty(ref rectangleGeometryValue, value);

        }

        /// <summary>
        /// 悬浮窗图片路径
        /// </summary>
        private Uri imagePath;
        public Uri ImagePath
        {
            get => imagePath;
            set
            {
                if (File.Exists(value.LocalPath))
                {
                    SetProperty(ref imagePath, value);
                }
                else
                {
                    SetProperty(ref imagePath, new Uri("pack://application:,,,/Resources/ErrorImage.png"));
                    MessageBox.Show("悬浮窗路径不存在");
                }
            }
        }

        #endregion


        /// <summary>
        /// 窗体改变函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigValueChanged(object sender, EventArgs e)
        {
            var size = sender?.ToString().Split(':');
            FloatingSetHeight = double.Parse(size == null ? ConfigurationManager.AppSettings.Get(nameof(FloatingSetHeight)) : size[0]);
            FloatingSetWidth = double.Parse(size == null ? ConfigurationManager.AppSettings.Get(nameof(FloatingSetWidth)) : size[1]);
            RadiusXValue = this.FloatingSetWidth / 5;
            RadiusXValue = this.FloatingSetHeight / 5;
            RectangleGeometryValue = new Rect(0, 0, this.FloatingSetWidth, this.FloatingSetHeight);
        }

        public FloatingViewModel()
        {
            var iPath = ConfigurationManager.AppSettings["FloatingBgPath"];
            ImagePath = new Uri( string.IsNullOrEmpty(iPath) ? "pack://application:,,,/Resources/ErrorImage.png" : iPath);
            FloatingSetWidth = SystemParameters.WorkArea.Height / 10;
            FloatingSetHeight = SystemParameters.WorkArea.Height / 10;

            GlobalVariable.ConfigChanged += ConfigValueChanged;
            CustomSetViewModel.FloatingSizeEvent += ConfigValueChanged;
        }
    }
}
