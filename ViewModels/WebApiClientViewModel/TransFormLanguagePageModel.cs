using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConnectClient;
using ConnectClient.Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TaskTip.ViewModels.WebApiClientViewModel
{
    internal partial class TransFormLanguagePageModel : BaseHappyPageModel
    {
        #region 属性

        private string _sourceLanguage;
        public string SourceLanguage
        {
            get => _sourceLanguage;
            set => SetProperty(ref _sourceLanguage, value);
        }

        private string _targetLanguage;
        public string TargetLanguage
        {
            get => _targetLanguage;
            set => SetProperty(ref _targetLanguage, value);
        }

        private string _inputText;
        public string InputText
        {
            get => _inputText;
            set => SetProperty(ref _inputText, value);
        }

        private string _outputText;
        public string OutputText
        {
            get => _outputText;
            set => SetProperty(ref _outputText, value);
        }

        #endregion

        [RelayCommand]
        public void InputTextChanged(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            LoadingVisibility = Visibility.Visible;

            SendAsync().ContinueWith(_ =>
            {
                Console.WriteLine($"【{WebName}】执行完成,等待更新UI");
            });
        }

        public TransFormLanguagePageModel()
        {
            WebName = "TransFormLanguage";
        }

        protected override Task SendAsync()
        {
            Task.Run(() =>
            {
                _client = new HttpRequest();
                var queryParameters = new Dictionary<string, string>()
                {
                    {"text",InputText}
                };

                var queryString = new FormUrlEncodedContent(queryParameters).ReadAsStringAsync().Result;

                var reqUrl = $"{BaseUri}/fy?{queryString}";

                var req = new RequestModel()
                {
                    Url = reqUrl,
                    Method = HttpMethod.Get,
                    Content = ""
                };

                var result = _client.SendAsync(req).Result;
                if (result.StatusCode != 200)
                {
                    Console.WriteLine(result.Message);
                    return;
                }

                var content = JsonConvert.DeserializeObject<JObject>(result.Content);

                if (content["success"].ToObject<bool>())
                {
                    var languageToLanguage = content["type"].ToObject<string>().Split("2");
                    if (content["data"] != null)
                    {
                        CompleteInvoke<dynamic>(new
                        {
                            LanguageToLanguage = languageToLanguage,
                            Output = content["data"]["fanyi"].ToString()
                        });
                    }
                }
            });
            return Task.CompletedTask;
        }

        protected override Task CompleteInvoke<T>(T collection)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var obj = (dynamic)collection;

                SourceLanguage = obj.LanguageToLanguage[0];
                TargetLanguage = obj.LanguageToLanguage[1];

                OutputText = obj.Output;

                LoadingVisibility = Visibility.Collapsed;
            });
            return Task.CompletedTask;
        }
    }

}
