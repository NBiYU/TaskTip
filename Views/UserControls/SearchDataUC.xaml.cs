using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskTip.Models.DataModel;
using TaskTip.ViewModels.UserViewModel.CustomControl;

namespace TaskTip.Views.UserControls
{
    /// <summary>
    /// SearchDataUC.xaml 的交互逻辑
    /// </summary>
    public partial class SearchDataUC : UserControl
    {
        #region CacheData
        public List<SearchDataModel> SearchCache
        {
            get => (List<SearchDataModel>)GetValue(CacheDataProperty);
            set => SetValue(CacheDataProperty, value);
        }
        public static readonly DependencyProperty CacheDataProperty =
   DependencyProperty.Register("CacheData", typeof(List<SearchDataModel>), typeof(SearchDataUC), new PropertyMetadata());
        #endregion

        #region SearchResult
        public List<SearchDataModel> SearchResult
        {
            get => (List<SearchDataModel>)GetValue(BindableResultDataProperty);
            set => SetValue(BindableResultDataProperty, value);
        }
        public static readonly DependencyProperty BindableResultDataProperty =
            DependencyProperty.Register("ResultData", typeof(List<SearchDataModel>), typeof(SearchDataUC), new PropertyMetadata());
        #endregion


        public SearchDataUC()
        {
            InitializeComponent();
        }

        private void SearchClick_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchText.Text)) return;
            SearchResult = SearchCache.FindAll(x => x.Title.Contains(SearchText.Text) || x.Content.Contains(SearchText.Text));
        }
    }
}
