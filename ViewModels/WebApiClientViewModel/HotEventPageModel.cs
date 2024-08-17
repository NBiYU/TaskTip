using CommunityToolkit.Mvvm.Input;
using ConnectClient;
using ConnectClient.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TaskTip.Models;
using TaskTip.Services;
using TaskTip.UserControls;
using MessageBox = HandyControl.Controls.MessageBox;

namespace TaskTip.ViewModels.WebApiClientViewModel
{
    internal partial class HotEventPageModel : BaseHappyPageModel
    {
        #region 属性

        private Dictionary<string, string> StationDictionary = new Dictionary<string, string>();
        private List<HotEventModel> HotEvents = new();

        private string _lastUpdateTime;

        public string LastUpdateTime
        {
            get => _lastUpdateTime;
            set => SetProperty(ref _lastUpdateTime, value);
        }


        private string _selectItem;
        public string SelectItem
        {
            get => _selectItem;
            set => SetProperty(ref _selectItem, value);
        }



        private ObservableCollection<string> _sourceKeyCollection = new();
        public ObservableCollection<string> SourceKeyCollection
        {
            get => _sourceKeyCollection;
            set => SetProperty(ref _sourceKeyCollection, value);
        }

        private ObservableCollection<EventItemUserControl> _searchResultCollection = new();
        public ObservableCollection<EventItemUserControl> SearchResultCollection
        {
            get => _searchResultCollection;
            set => SetProperty(ref _searchResultCollection, value);
        }
        #endregion

        #region 指令

        [RelayCommand]
        public async Task Search(string key)
        {

            if (StationDictionary.ContainsKey(key))
            {
                await SendAsync();
                LoadingVisibility = Visibility.Visible;
            }
            else MessageBox.Show($"未知站点：{key}");
        }

        [RelayCommand]
        public async Task LoadingImage(object sender)
        {
            if (sender is not ListBox listBox) return;
            if (listBox.Items.Count < 10) return;

            var scrollViewer = await GetScrollViewer((DependencyObject)sender);

            var verticalOffset = scrollViewer.VerticalOffset;
            var scrollableHeight = scrollViewer.ScrollableHeight;

            var itemGap = scrollableHeight / SearchResultCollection.Count * 0.9;
            for (int i = 0; i < verticalOffset / itemGap + 5; i++)
            {
                if (i >= SearchResultCollection.Count)
                    continue;
                if ((string.IsNullOrEmpty(SearchResultCollection[i]._model.PicUrl) || SearchResultCollection[i].IsLoadedImage))
                    continue;

                SearchResultCollection[i].Image.Source = new BitmapImage(new Uri(SearchResultCollection[i]._model.PicUrl));
                SearchResultCollection[i].IsLoadedImage = true;
            }
        }



        #endregion

        #region 功能函数


        /// <summary>
        /// 向下寻找子元素
        /// </summary>
        /// <param name="depObj"></param>
        /// <returns></returns>
        private async Task<ScrollViewer?> GetScrollViewer(DependencyObject depObj)
        {
            if (depObj is ScrollViewer scrollViewer)
                return scrollViewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                var result = await GetScrollViewer(child);
                if (result != null)
                    return result;
            }

            return null;
        }


        #endregion

        #region 初始化

        private void InitProperty()
        {
            var stationCollection =
                GlobalVariable.JsonConfiguration
                    .GetSection("WebApiStation")
                    .GetChildren();
            var dic = new Dictionary<string, string>();

            foreach (var obj in stationCollection)
            {
                var key = obj["Key"];
                var val = obj["Value"];
                if (!string.IsNullOrEmpty(key))
                {
                    dic.Add(key, val);
                }
            }

            StationDictionary = dic;
            SourceKeyCollection = new ObservableCollection<string>(StationDictionary.Keys);
        }

        public HotEventPageModel()
        {
            WebName = "HotEvent";

            InitProperty();
        }

        #endregion

        #region 继承

        protected override Task SendAsync()
        {
            Task.Run(async () =>
            {
                _client = new HttpRequest();
                var station = StationDictionary.FirstOrDefault(x => x.Key == SelectItem).Value;
                var errMsg = string.Empty;

                var queryParameter = new Dictionary<string, string>()
                {
                    { "type",  station}
                };

                var queryString = await new FormUrlEncodedContent(queryParameter).ReadAsStringAsync();
                var reqUrl = $"{BaseUri}/hotlist?{queryString}";

                var req = new RequestModel()
                {
                    Url = reqUrl,
                    Method = HttpMethod.Get,
                    Content = ""
                };

                var result = await _client.SendAsync(req);
                if (result.StatusCode == 200)
                {
                    try
                    {

                        var content = JObject.Parse(result.Content);
                        if (content.First.First.Value<bool>())
                        {
                            var updateTime = content["update_time"].Value<string>();
                            if (content["data"].Value<object>() != null)
                            {
                                HotEvents.Clear();
                                foreach (var data in content["data"])
                                {
                                    var picPath = data["pic"]?.Value<string>() ?? "";

                                    HotEvents.Add(new HotEventModel()
                                    {
                                        Title = data["title"]?.Value<string>() ?? "",
                                        TextContent = data["desc"]?.Value<string>() ?? "",
                                        PicUrl = picPath,
                                        UrlString = data["url"]?.Value<string>() ?? "",
                                    });
                                }

                                if (HotEvents.Count != 0)
                                {
                                    await CompleteInvoke<dynamic>(new
                                    {
                                        UpdateTime = updateTime,
                                        Controls = HotEvents
                                    });

                                }
                            }
                        }
                        else
                        {
                            errMsg = $"【{WebName}】获取失败：{content["message"].ToString()}";
                        }
                    }
                    catch (Exception e)
                    {
                        errMsg = $"【{WebName}】解析异常：{e.Message},JSON：{result.Content}";
                    }
                }
                else
                {
                    errMsg = $"【{WebName}】发送异常：{result.Message}";
                }

                

                Application.Current.Dispatcher.Invoke(() =>
                {
                    LoadingVisibility = Visibility.Collapsed;
                    if (!string.IsNullOrEmpty(errMsg)) MessageBox.Show(errMsg);
                });
            });
            return Task.CompletedTask;
        }

        protected override Task CompleteInvoke<T>(T collection)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
            {
                var obj = (dynamic)collection;
                LastUpdateTime = obj.UpdateTime;

                SearchResultCollection.Clear();

                var controls = (List<HotEventModel>)obj.Controls;
                foreach (var model in controls)
                {
                    SearchResultCollection.Add(new EventItemUserControl(model));
                }


                for (int i = 0; i < 10; i++)
                {
                    if (string.IsNullOrEmpty(SearchResultCollection[i]._model.PicUrl) || SearchResultCollection[i].IsLoadedImage)
                        continue;

                    SearchResultCollection[i].Image.Source = new BitmapImage(new Uri(SearchResultCollection[i]._model.PicUrl));
                    SearchResultCollection[i].IsLoadedImage = true;
                }

                LoadingVisibility = Visibility.Collapsed;
            });
            return Task.CompletedTask;
        }
        #endregion

    }

}
