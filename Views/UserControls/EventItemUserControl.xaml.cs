using HandyControl.Controls;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TaskTip.Models;

namespace TaskTip.UserControls
{
    /// <summary>
    /// EventItemUserControl.xaml µÄ½»»¥Âß¼­
    /// </summary>
    public partial class EventItemUserControl : UserControl
    {
        public HotEventModel _model = new();
        public bool IsLoadedImage = false;

        public EventItemUserControl()
        {
            InitializeComponent();
        }

        public EventItemUserControl(HotEventModel data)
        {
            InitializeComponent();
            _model = data;
            Title.Text = data.Title;
            if (!string.IsNullOrEmpty(data.PicUrl))
            {
                Image.Source =
                    (BitmapImage)Application.Current
                        .Resources["InitImage"]; //(new Uri("pack://application:,,,/Resources/InitImage.png"));
                //Image.Source = data.PicSource;
            }
            TextContent.Text = data.TextContent;
            Link.Content = data.UrlString;
        }

        private void Link_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            var uri = btn.Content.ToString();
            Process.Start(new ProcessStartInfo()
            {
                FileName = uri,
                UseShellExecute = true
            });
            //Clipboard.SetText(uri);
        }

        private void Image_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var image = sender as Image;
            if (e.ClickCount == 2 && image.Source != null)
            {
                new ImageBrowser(image.Source.ToString()).Show();
            }
        }

    }
}
