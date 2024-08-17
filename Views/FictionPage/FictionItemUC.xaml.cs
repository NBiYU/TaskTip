using Newtonsoft.Json;
using System.IO;
using TaskTip.Common.Extends;
    /// <summary>
        {
            BookNameToolTip.Text = progressModel.Title;
            AuthorNameToolTip.Text = progressModel.Author;
            DescToolTip.Text = string.Join("\n", progressModel.Description?.ChunkString(20) ?? Array.Empty<string>());
            LastUpdateTimeToolTip.Text = progressModel.LastUpdateTime;
        }
                {
                    CoverImage.Source = AddImage(localPath);
                }else
                {
                    Task.Run(() =>
                    {
                        var imageSource = ConvertImage(progressModel.CoverImageSource);  //?? ((BitmapImage)Application.Current.Resources["ErrorImage"]).UriSource;
                        if(imageSource == null) imageSource = (BitmapImage)Application.Current.Resources["ErrorImage"];
                        Application.Current.Dispatcher.Invoke(() => { CoverImage.Source = imageSource; });
                    });
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
                     //����ͼƬ
                     using (var g = Graphics.FromImage(bmNew))
                     {
                         g.Clear(System.Drawing.Color.Transparent);
                         g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                         g.DrawImage(bmTemp, new System.Drawing.Rectangle(0, 0, bmNew.Width, bmNew.Height), 0, 0, bmTemp.Width, bmTemp.Height, GraphicsUnit.Pixel);
                         g.Dispose();
                     }

                     var bitmapImage = new BitmapImage();
                     //�����ڴ�
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