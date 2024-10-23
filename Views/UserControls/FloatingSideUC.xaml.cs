using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using TaskTip.Models.CommonModel;

namespace TaskTip.Views.UserControls
{
    /// <summary>
    /// FloatingSideUC.xaml 的交互逻辑
    /// </summary>
    public partial class FloatingSideUC : UserControl
    {
        #region 依赖属性
        public static readonly DependencyProperty ActionsProperty =
                    DependencyProperty.Register(
                        nameof(Actions), // 属性名称
                        typeof(ObservableCollection<FloatingSideModel>), // 属性类型
                        typeof(FloatingSideUC), // 所属类型
        new PropertyMetadata(null, OnItemsChanged)); // 默认值

        // CLR 包装器
        public ObservableCollection<FloatingSideModel> Actions
        {
            get { return (ObservableCollection<FloatingSideModel>)GetValue(ActionsProperty); }
            set { 
                SetValue(ActionsProperty, value); 
            }
        }
        private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FloatingSideUC;
            if (e.OldValue is ObservableCollection<string> oldCollection)
            {
                // 如果之前有绑定的集合，移除事件监听
                oldCollection.CollectionChanged -= control.OnCollectionChanged;
            }

            if (e.NewValue is ObservableCollection<string> newCollection)
            {
                // 监听新的集合的变化
                newCollection.CollectionChanged += control.OnCollectionChanged;
            }
        }
        // 集合改变时的处理方法（可以根据需要处理逻辑）
        private void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // 集合内容更新时的逻辑
            // 通常情况下 WPF 会自动更新绑定的 UI，因此这里可以不处理
        }
        #endregion
        private void InitTemplate()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FloatingSideUC),
            new FrameworkPropertyMetadata(typeof(FloatingSideUC)));
        }

        public FloatingSideUC()
        {
            InitializeComponent();
            InitTemplate();
        }
        public FloatingSideUC(List<FloatingSideModel> list)
        {
            InitializeComponent();
            InitTemplate();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ActionsUC.Visibility = ActionsUC.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
