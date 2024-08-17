using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConnectClient;
using ConnectClient.Model;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Application;

namespace TaskTip.ViewModels.WebApiClientViewModel
{
    internal partial class EverydayEventPageModel : BaseHappyPageModel
    {
        #region  属性



        private string _newCalendar;
        public string NewCalendar
        {
            get => _newCalendar;
            set => SetProperty(ref _newCalendar, value);
        }

        private string _lunar;

        public string Lunar
        {
            get => _lunar;
            set => SetProperty(ref _lunar, value);
        }

        private string _week;

        public string Week
        {
            get => _week;
            set => SetProperty(ref _week, value);
        }


        private string _textContent;

        public string TextContent
        {
            get => _textContent;
            set => SetProperty(ref _textContent, value);
        }

        private Uri _imageSource;

        public Uri ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        #endregion

        private ObservableCollection<string> _everydayEvents = new();
        public ObservableCollection<string> EverydayEvents
        {
            get => _everydayEvents;
            set => SetProperty(ref _everydayEvents, value);
        }

        #region 指令

        [RelayCommand]
        public void CopyContent(object sender)
        {
            if (sender is not Button control) return;

            Clipboard.SetText(control.Content.ToString());
        }
        [RelayCommand]
        public async Task InitPage()
        {
            LoadingVisibility = Visibility.Visible;

            await SendAsync();
        }
        #endregion

        #region 功能函数

        private Button AddItem(string content)
        {
            Button btn = new();
            btn.Content = content;
            return btn;
        }



        #endregion

        #region 初始化

        public EverydayEventPageModel()
        {
            WebName = "每天60秒读懂世界";
        }

        #endregion


        protected override async Task SendAsync()
        {
            await Task.Run(async () =>
            {
                _client = new HttpRequest();
                var baseUrl = "https://api.vvhan.com/api/";
                var errMsg = string.Empty;

                var queryParameter = new Dictionary<string, string>()
                {
                    { "type", "json" }
                };

                var queryString = await new FormUrlEncodedContent(queryParameter).ReadAsStringAsync();
                var reqUrl = $"{baseUrl}/60s?{queryString}";

                var req = new RequestModel()
                {
                    Url = reqUrl,
                    Method = HttpMethod.Get,
                    Content = ""
                };

                var result = await _client.SendAsync(req);
                if (result.StatusCode == 200)
                {
                    var content = JObject.Parse(result.Content);
                    if (content.First.First.ToObject<bool>())
                    {
                        var time = content["time"].ToObject<List<string>>();
                        var items = content["data"].ToObject<List<string>>();
                        await CompleteInvoke<dynamic>(new
                        {
                            Time = time,
                            Items = items
                        });
                    }else
                    {
                        errMsg = $"【{WebName}】返回异常：结果[{content.First.First.ToObject<bool>()}]";
                    }
                }
                else
                {
                    errMsg = $"【{WebName}】请求错误：代码：{result.StatusCode}";
                }

                Application.Current.Dispatcher.Invoke(() => {
                    LoadingVisibility = Visibility.Collapsed;
                    if (!string.IsNullOrEmpty(errMsg)) MessageBox.Show(errMsg);
                });
            });
        }

        protected override Task CompleteInvoke<T>(T collection)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                var obj = (dynamic)collection;
                NewCalendar = obj.Time[0];
                Lunar = obj.Time[1];
                Week = obj.Time[2];


                TextContent = string.Join("\n\t", obj.Items);
                EverydayEvents = new ObservableCollection<string>(obj.Items);
                //EverydayEvents = new ObservableCollection<Button>(items);

                LoadingVisibility = Visibility.Collapsed;
            });
            return Task.CompletedTask;
        }
    }
}
