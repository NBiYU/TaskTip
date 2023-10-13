using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using TaskTip.Services;
using MessageBox = System.Windows.MessageBox;

namespace TaskTip.ViewModels.WindowModel
{

    internal class FloatingViewModel : ObservableObject
    {

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

        private double radiusXValue;
        /// <summary>
        /// 矩形起始点X轴
        /// </summary>
        public double RadiusXValue
        {
            get => radiusXValue;
            set => SetProperty(ref radiusXValue, value);
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
                if (value != null && File.Exists(value.LocalPath))
                {
                    SetProperty(ref imagePath, value);
                }
                else
                {
                    SetProperty(ref imagePath, ((BitmapImage)Application.Current.Resources["ErrorImage"]).UriSource);
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
        private void ConfigValueChanged(string sender)
        {
            if (string.IsNullOrEmpty(sender))
            {
                FloatingSetHeight = GlobalVariable.FloatingSetHeight;
                FloatingSetHeight = GlobalVariable.FloatingSetWidth;
            }
            else
            {
                var size = sender.Split(':');
                FloatingSetHeight = double.Parse(size[0]);
                FloatingSetWidth = double.Parse(size[1]);
            }


            RadiusXValue = FloatingSetWidth / 5;
            RadiusXValue = FloatingSetHeight / 5;

            RectangleGeometryValue = new Rect(0, 0, FloatingSetWidth, FloatingSetHeight);
        }

        private void InitRegister()
        {
            WeakReferenceMessenger.Default.Register<string, string>(this, Const.CONST_CONFIG_CHANGED, (obj, msg) => ConfigValueChanged(msg));
            WeakReferenceMessenger.Default.Register<string, string>(this, Const.CONST_FLAOTING_SIZE_CHANGED, (obj, msg) => ConfigValueChanged(msg));
        }

        public FloatingViewModel()
        {

            var iPath = GlobalVariable.FloatingBgPath;
            ImagePath = string.IsNullOrEmpty(iPath) ? null : new Uri(iPath);
            FloatingSetWidth = SystemParameters.WorkArea.Height / 10;
            FloatingSetHeight = SystemParameters.WorkArea.Height / 10;

            InitRegister();
        }
    }
}
