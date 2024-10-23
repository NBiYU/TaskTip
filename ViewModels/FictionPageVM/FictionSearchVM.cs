using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using TaskTip.Extends.FictionAPI.LRY_API;
using TaskTip.Extends.FictionAPI.OptionEnum;
using TaskTip.Models.DataModel;
using TaskTip.Views.FictionPage;

namespace TaskTip.ViewModels.FictionPageVM
{
    public partial class FictionSearchVM:ObservableObject
    {

        private LRY_APIRequest _request;

        private string _searchContent;
        public string SearchContent
        {
            get => _searchContent;
            set => SetProperty(ref _searchContent,value);
        }

        private string _searchType;

        public string SearchType
        {
            get => _searchType;
            set => SetProperty(ref _searchType, value);
        }

        private Visibility _loadingVisibility;
        public Visibility LoadingVisibility
        {
            get=> _loadingVisibility;
            set => SetProperty(ref _loadingVisibility, value);
        }

        private ObservableCollection<string> _searchTypes;
        public ObservableCollection<string> SearchTypes
        {
            get => _searchTypes;
            set => SetProperty(ref _searchTypes, value);
        }

        private ObservableCollection<FictionItemUC> _fictions;

        public ObservableCollection<FictionItemUC> Fictions
        {
            get => _fictions;
            set => SetProperty(ref _fictions, value);
        }

        [RelayCommand]
        public async Task SearchFiction()
        {
            if (LoadingVisibility == Visibility.Visible) return;
            LoadingVisibility = Visibility.Visible;
            try
            {
                var optionType = _searchType switch
                {
                    "作者" => LRY_APIOptionEnum.SearchAuthor,
                    "书籍" => LRY_APIOptionEnum.SearchTitle
                };

                var result = await _request.SearchFiction(optionType, SearchContent, 1, 30);
                if (result.code != 0 && result.count == 0)
                {
                    System.Windows.MessageBox.Show("搜索失败：" + result.msg);
                    return;
                }

                var fictions = new List<FictionItemUC>();
                foreach (var item in result.data)
                {
                    fictions.Add(new FictionItemUC(new FictionProgressModel()
                    {
                        Author = item.author,
                        CoverImageSource = item.cover,
                        Id = item.fictionId,
                        Description = item.descs,
                        LastChapterReadIndex = 0,
                        LastContentReadIndex = 0,
                        LastUpdateTime = item.updateTime,
                        Title = item.title,
                    }, IsOperation: false));
                }

                Fictions = new ObservableCollection<FictionItemUC>(fictions);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show($"搜索异常： {e.Message}");
            }
            finally
            {
                LoadingVisibility = Visibility.Collapsed;
            }
        }

        public FictionSearchVM()
        {
            _request = new();
            LoadingVisibility = Visibility.Collapsed;
            Fictions = new ObservableCollection<FictionItemUC>();
            SearchTypes = new ObservableCollection<string>
            {
                "书籍",
                "作者",
            };
        }
    }
}
