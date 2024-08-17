using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using TaskTip.Extends.FictionAPI.LRY_API;
using TaskTip.Extends.FictionAPI.OptionEnum;
using TaskTip.Views.FictionPage;
using TaskTipProject.Models;

namespace TaskTip.ViewModels.FictionPageVM
{
    public partial class FictionCategoryVM : ObservableObject
    {
        private LRY_APIRequest _request;
        private int _startindex;
        private int _gap;
        private string _currentCategory;
        private bool _isLoading;


        #region 属性

        private string _listDesc;
        public string ListDesc
        {
            get => _listDesc;
            set=>SetProperty(ref _listDesc, value);
        }

        private Visibility _descVisibility;
        public Visibility DescVisibility
        {
            get => _descVisibility;
            set=>SetProperty(ref _descVisibility, value);
        }
        private Visibility _loadingVisibility;
        public Visibility LoadingVisibility
        {
            get => _loadingVisibility;
            set=>SetProperty(ref _loadingVisibility, value);
        }

        private ObservableCollection<string> _headers;
        public ObservableCollection<string> Headers
        {
            get => _headers;
            set => SetProperty(ref _headers, value);
        }

        private ObservableCollection<FictionItemUC> _categorieContents=new();
        public ObservableCollection<FictionItemUC> CategoriaContents
        {
            get => _categorieContents;
            set => SetProperty(ref _categorieContents, value);
        }

        #endregion

        #region 指令函数

        [RelayCommand]
        public async Task TabSelectChanged(object sender)
        {
            if (sender is string categoryName)
            {
                if (categoryName != _currentCategory)
                {
                    if (_isLoading)
                    {
                        MessageBox.Show("切换频繁，清稍后在尝试！！");
                        return;
                    }

                    _startindex = 1;
                    _gap = 30;
                    DescVisibility = Visibility.Collapsed;
                    CategoriaContents.Clear();

                    await LoadCategories(categoryName);
                }
            }
                
        }

        [RelayCommand]
        public async Task ScrollChanged(object sender)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                if (scrollViewer.ScrollableHeight == 0)
                {
                    scrollViewer.ScrollToTop();
                    return;
                }else if (scrollViewer.ScrollableHeight == scrollViewer.VerticalOffset && !_isLoading)
                {
                    _isLoading = true;
                    _startindex = _gap;
                    _gap += 30;
                    ListDesc = "正在加载中";
                    DescVisibility = Visibility.Visible;
                    await LoadCategories(_currentCategory);
                }else
                {
                    DescVisibility = Visibility.Collapsed;
                }
            }
        }

        [RelayCommand]
        public async Task FictionCategory()
        {
            _request = new LRY_APIRequest();
            await LoadCategories(Headers[0]);
        }

        #endregion

        #region 功能函数

        private Task LoadCategories(string categoryName)
        {
            //_cancellationTokenSource.Cancel();
            _currentCategory = categoryName;
            var categoryEnum =  EnumExtend.TryGetEnum<LRY_APIFictionCagetory>(categoryName);
            LoadingVisibility = Visibility.Visible;
            int startindex = _startindex;
            int gap = _gap;

            Task.Run(async() =>
            {

                _isLoading = true; //在这里再次标识是防止其他地方没有成功表示
                await Task.Delay(100);
                var collection = new List<FictionProgressModel>();
                try
                {
                    var content = await _request.SearchCategoryFiction(categoryEnum, startindex, gap);

                    if (content.code == 0)
                    {
                        foreach (var item in content.data)
                        {
                            collection.Add(new FictionProgressModel()
                            {
                                Id = item.fictionId,
                                Title = item.title,
                                Author = item.author,
                                Description = item.descs,
                                LastUpdateTime = item.updateTime,
                                CoverImageSource = item.cover,
                                LastChapterReadIndex = 0,
                                LastContentReadIndex = 0
                            });
                        }
                    }
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (content.code != 0)
                        {
                            ListDesc = content.msg;
                            DescVisibility = Visibility.Visible;
                            LoadingVisibility = Visibility.Collapsed;
                            _isLoading = false;
                            return;
                        }

                        //var collectionUC = new List<FictionItemUC>();
                        foreach (var item in collection)
                        {
                            CategoriaContents.Add(new FictionItemUC(item, IsOperation: false));
                        }
                        //CategoriaContents = new ObservableCollection<FictionItemUC>(collectionUC);
                        _isLoading = false;
                        DescVisibility = Visibility.Collapsed;
                        LoadingVisibility = Visibility.Collapsed;
                    });
                }
                catch (Exception e)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        LoadingVisibility = Visibility.Collapsed;
                        _isLoading = false;
                        MessageBox.Show(e.Message);
                    });

                }
            });

            return Task.CompletedTask;
        }

        #endregion

        private void InitProperty()
        {
            _startindex = 1;
            _gap = 30;
            DescVisibility = Visibility.Collapsed;

        }

        public FictionCategoryVM() {
            InitProperty();
            Headers = new ObservableCollection<string>(EnumExtend.GetEnumAllDesc<LRY_APIFictionCagetory>());

        }
    }
}
