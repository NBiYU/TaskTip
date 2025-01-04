using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using TaskTip.Common.Extends;
using TaskTip.Models.DataModel;
using TaskTip.Models.Enums;
using TaskTip.Services;
using TaskTip.ViewModels.WindowModel.PopVM;
using TaskTip.Views.Windows.PopWindow;

using Path = System.IO.Path;

namespace TaskTip.Views.FictionPage
{
    /// <summary>
    /// FictionItemUC.xaml 的交互逻辑
    /// </summary>
    public partial class FictionItemUC : UserControl
    {
        private FictionProgressModel model;

        public delegate void EventHandler(object sender, EventArgs e);

        public event EventHandler ItemDelete;
        public event EventHandler ItemClick;

        private void InitToolTip(FictionProgressModel progressModel)
        {
            BookNameToolTip.Text = progressModel.Title;
            AuthorNameToolTip.Text = progressModel.Author;
            DescToolTip.Text = string.Join("\n", progressModel.Description?.ChunkString(20) ?? Array.Empty<string>());
            LastUpdateTimeToolTip.Text = progressModel.LastUpdateTime;
        }

        public FictionItemUC()
        {
            InitializeComponent();
        }

        public FictionItemUC(FictionProgressModel progressModel, bool IsUpdate = false, bool IsOperation = true)
        {

            InitializeComponent();
            model = progressModel;
            FictionName.Text = model.Title;
            AuthorName.Text = model.Author;
            LastUpdateTime.Text = model.LastUpdateTime;
            LastUpdateTime.Foreground = new SolidColorBrush(IsUpdate ? Colors.Red : Colors.Black);
            Refresh.Visibility = IsOperation ? Visibility.Visible : Visibility.Collapsed;
            Delete.Visibility = IsOperation ? Visibility.Visible : Visibility.Collapsed;
            InitToolTip(progressModel);
            //获取图片
            try
            {
                var localPath = Path.Combine(GlobalVariable.FictionCachePath, progressModel.Id, progressModel.CoverImageSource);
                var isLocalPath = File.Exists(localPath);
                if (isLocalPath)
                {
                    CoverImage.Source = AddImage(localPath);
                }
                else
                {
                    Task.Run(() =>
                    {
                        var imageSource = ConvertImage(progressModel.CoverImageSource);  //?? ((BitmapImage)Application.Current.Resources["ErrorImage"]).UriSource;
                        if (imageSource == null) imageSource = (BitmapImage)Application.Current.Resources["ErrorImage"];
                        Application.Current.Dispatcher.Invoke(() => { CoverImage.Source = imageSource; });
                    });
                }
            }
            catch (COMException ex)
            {
                CoverImage.Source = new BitmapImage(((BitmapImage)Application.Current.Resources["ErrorImage"]).UriSource);
            }
        }

        private BitmapImage? AddImage(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    var binaryReader = new BinaryReader(File.Open(path, FileMode.Open));
                    var fileInfo = new FileInfo(path);
                    var bytes = binaryReader.ReadBytes((int)fileInfo.Length);
                    binaryReader.Close();

                    var bmTemp = new Bitmap(new MemoryStream(bytes));

                    var bmNew = new Bitmap(bmTemp.Width, bmTemp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    bmNew.SetResolution(96, 96);
                    //绘制图片
                    using (var g = Graphics.FromImage(bmNew))
                    {
                        g.Clear(System.Drawing.Color.Transparent);
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(bmTemp, new System.Drawing.Rectangle(0, 0, bmNew.Width, bmNew.Height), 0, 0, bmTemp.Width, bmTemp.Height, GraphicsUnit.Pixel);
                        g.Dispose();
                    }

                    var bitmapImage = new BitmapImage();
                    //载入内存
                    using (var ms = new MemoryStream())
                    {
                        bmNew.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = ms;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();

                        ms.Dispose();
                    }

                    return bitmapImage;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
            //return AddImage(new BitmapImage(new Uri(path)),path);
        }

        private void Refresh_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            CoverImage.Source = (BitmapImage)Application.Current.Resources["ErrorImage"];
            ItemDelete?.Invoke(model, null);
        }

        private void ButtonBase_OnClick(object sender, MouseButtonEventArgs e)
        {
            //写入该书记录，并弹出阅读窗口
            if (e.ClickCount == 2)
            {
                if (FictionReadView.HotKeyManagers.Count != 0) return; // 阻止第二个阅读界面出现

                var progress = string.Empty;
                var saveProgress = new List<FictionProgressModel>();

                if (File.Exists(GlobalVariable.FictionProgressPath))
                {
                    progress = File.ReadAllText(GlobalVariable.FictionProgressPath);
                }


                if (!string.IsNullOrEmpty(progress))
                {
                    var objList = JsonConvert.DeserializeObject<List<FictionProgressModel>>(progress);
                    if (objList is { Count: > 0 })
                        saveProgress.AddRange(objList);
                }

                var localProgress = saveProgress.FirstOrDefault(x => x.Title == model.Title);
                if (localProgress == null)
                {
                    var coverCachePath = Path.Combine(GlobalVariable.FictionCachePath, model.Id, model.CoverImageSource.Replace("/", "_").Replace(":", "_"));
                    SaveImage((BitmapImage)CoverImage.Source, SaveImageType.JPEG, coverCachePath);
                    model.CoverImageSource = model.CoverImageSource.Replace("/", "_").Replace(":", "_");

                    saveProgress.Add(model);
                    File.WriteAllText(GlobalVariable.FictionProgressPath, JsonConvert.SerializeObject(saveProgress, Formatting.Indented));
                }
                else
                {
                    model = localProgress;
                }

                var window = new FictionReadView()
                {
                    DataContext = new FictionReadVM(model)
                };
                window.Show();
                ItemClick?.Invoke(model, null);
            }

        }

        private void SaveImage(BitmapSource source, SaveImageType type, string filePath)
        {
            dynamic encoder = type switch
            {
                SaveImageType.PNG => new PngBitmapEncoder(),
                SaveImageType.JPEG => new JpegBitmapEncoder(),
                _ => new PngBitmapEncoder(),
            };

            if (!Directory.Exists(Path.GetDirectoryName(filePath))) Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            encoder.Frames.Add(BitmapFrame.Create(source));

            using (var file = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(file);
            }
        }

        public bool TryDownloadImage(string imageUrl, out byte[] imageData)
        {
            imageData = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(imageUrl);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    if (stream != null)
                    {
                        // 尝试从网络获取图像数据
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            memoryStream.Write(buffer, 0, bytesRead);
                        }

                        imageData = memoryStream.ToArray();
                        return true; // 图像获取成功
                    }
                }
            }
            catch (Exception ex)
            {
                // 捕获异常，图像获取失败
                Console.WriteLine("Error downloading image: " + ex.Message);
            }

            return false; // 图像获取失败
        }

        public BitmapImage? ConvertImage(string imageUrl)
        {


            if (!TryDownloadImage(imageUrl, out var imageData)) return null;

            if (imageData == null || imageData.Length == 0) return null;

            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                using (MemoryStream stream = new MemoryStream(imageData))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze(); // 冻结图像以提高性能和线程安全性
                }

                return bitmapImage; // 返回转换后的BitmapImage
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error converting byte[] to BitmapImage: " + ex.Message);
                return null; // 返回null表示转换失败
            }
        }

    }
}