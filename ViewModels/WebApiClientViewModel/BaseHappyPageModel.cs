using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using ConnectClient;

namespace TaskTip.ViewModels.WebApiClientViewModel
{
    public abstract class BaseHappyPageModel : ObservableObject
    {

        public HttpRequest _client;
        public string BaseUri = "https://api.vvhan.com/api/";

        private Visibility _loadingVisibility = Visibility.Collapsed;
        public Visibility LoadingVisibility
        {
            get => _loadingVisibility;
            set => SetProperty(ref _loadingVisibility, value);
        }


        private string _webName;
        public string WebName
        {
            get => _webName;
            set => SetProperty(ref _webName, value);
        }

        protected abstract Task SendAsync();
        protected abstract Task CompleteInvoke<T>(T collection);
    }
}
