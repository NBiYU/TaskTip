using CommunityToolkit.Mvvm.ComponentModel;

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
using System.Windows.Shapes;

namespace TaskTip.Views.Windows.PopWindow
{
    /// <summary>
    /// ClockSelectorPop.xaml 的交互逻辑
    /// </summary>
    public partial class ClockSelectorPop : Window
    {
        public ClockSelectorPop()
        {
            InitializeComponent();
            DataContext = this;

            InitTimeInfo();
        }

        public ClockSelectorPop(DateTime date)
        {
            InitializeComponent();
            DataContext = this;

            InitTimeInfo(date);
        }

        #region TestClockControl

        private int _selectYear;
        private int _selectMonth;

        private int SelectYearValue
        {
            get => _selectYear;
            set
            {
                _selectYear = value;
                SelectYear.Content = value;
                OnYearOrMonthChanged();
            }
        }
        private int SelectMonthValue
        {
            get => _selectMonth;
            set
            {
                _selectMonth = value;
                SelectMonth.Content = value;
                OnYearOrMonthChanged();
            }
        }
        private int _selectHour;
        public int SelectHourValue
        {
            get => _selectHour;
            set
            {
                _selectHour = value;
            }
        }
        private int _selectMinute;
        public int SelectMinuteValue
        {
            get => _selectMinute;
            set
            {
                _selectMinute = value;
            }
        }
        private int _selectSecond;
        public int SelectSecondValue
        {
            get => _selectSecond;
            set
            {
                _selectSecond = value;
            }
        }

        public event EventHandler Confirmed;

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (((Control)sender).DataContext is CheckGroupItem model)
            {
                foreach (var item in DayContainer.Items)
                {
                    var obj = (CheckGroupItem)item;
                    if (obj != model)
                    {
                        obj.IsSelect = false;
                    }
                }
            }
        }
        private void InitTimeInfo(DateTime? date = null)
        {
            if(date == null)
            {
                date = DateTime.Now;
            }
            var time = date.Value;
            SwicthVisibility(1);

            SelectMonthValue = time.Month;
            SelectYearValue = time.Year;
            DayContainer.ItemsSource = GetDays(time.Year, time.Month);
            MonthContainer.ItemsSource = Enumerable.Range(1, 12);
            YearContainer.ItemsSource = GetAdjacentYears(time.Year);

            var lst = Enumerable.Range(0, 60).Select(x => x);

            HourListBox.ItemsSource = lst.Take(24);
            MinuteListBox.ItemsSource = lst;
            SecondListBox.ItemsSource = lst;

            SelectHourValue = time.Hour;
            SelectMinuteValue = time.Minute;
            SelectSecondValue = time.Second;
            OnTimeChanged();

        }
        private List<CheckGroupItem> GetDays(int year, int month)
        {
            var calendar = new List<CheckGroupItem>();

            // 获取当月的第一天和总天数
            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);

            // 获取第一天的星期（将 Sunday 转换为 7）
            int firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
            if (firstDayOfWeek == 0) firstDayOfWeek = 7;

            // 上月信息
            DateTime previousMonth = firstDayOfMonth.AddMonths(-1);
            int daysInPreviousMonth = DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);

            // 计算上月需要填充的天数
            int daysFromPreviousMonth = firstDayOfWeek - 1;

            // 填充上月日期
            for (int i = daysFromPreviousMonth; i > 0; i--)
            {
                var date = new DateTime(previousMonth.Year, previousMonth.Month, daysInPreviousMonth - i + 1);
                calendar.Add(new() { Content = $"{date.Day:D2}" });
            }

            var currentDate = DateTime.Now;
            // 填充当前月日期
            for (int i = 1; i <= daysInMonth; i++)
            {
                var date = new DateTime(year, month, i);
                calendar.Add(new() { Content = $"{date.Day:D2}", IsCurrentMonth = true, IsSelect = currentDate.Day == i });
            }

            // 下月信息
            DateTime nextMonth = firstDayOfMonth.AddMonths(1);
            int remainingSlots = 42 - calendar.Count;

            // 填充下月日期
            for (int i = 1; i <= remainingSlots; i++)
            {
                var date = new DateTime(nextMonth.Year, nextMonth.Month, i);
                calendar.Add(new() { Content = $"{date.Day:D2}" });
            }
            return calendar;
        }
        private List<int> GetAdjacentYears(int year)
        {
            var separations = 16 / 2;
            return Enumerable.Range(0, 16)
                .Select(x => x <= separations ? year + (-1) * x : year + (x - separations)).OrderBy(x => x).ToList();
        }
        /// <summary>
        /// 1： 天选择器 2：月选择器 3：年选择器
        /// </summary>
        /// <param name="type"></param>
        private void SwicthVisibility(int type)
        {
            switch (type)
            {
                case 1:
                    DayTimeSelector.Visibility = Visibility.Visible;
                    MonthTimeSelector.Visibility = Visibility.Collapsed;
                    YearTimeSelector.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    DayTimeSelector.Visibility = Visibility.Collapsed;
                    MonthTimeSelector.Visibility = Visibility.Visible;
                    YearTimeSelector.Visibility = Visibility.Collapsed;
                    break;
                case 3:
                    DayTimeSelector.Visibility = Visibility.Collapsed;
                    MonthTimeSelector.Visibility = Visibility.Collapsed;
                    YearTimeSelector.Visibility = Visibility.Visible;
                    break;
            }

        }

        private void OnYearOrMonthChanged()
        {
            if (SelectYearValue != 0 && SelectMonthValue != 0)
            {
                YearContainer.ItemsSource = GetAdjacentYears((int)SelectYearValue);
                DayContainer.ItemsSource = GetDays(SelectYearValue, SelectMonthValue);
                YearSelectorText.Text = $"{YearContainer.Items[0]} - {YearContainer.Items[^1]}";
            }

        }
        private void OnTimeChanged()
        {
            HourTextBox.Text = $"{SelectHourValue:D2}";
            MinuteTextBox.Text = $"{SelectMinuteValue:D2}";
            SecondTextBox.Text = $"{SelectSecondValue:D2}";
        }

        private void TryClose(bool dialogResult = false)
        {
            if (Owner != null)
            {
                DialogResult = dialogResult;
            }
            else
            {
                this.Close();
            }
        }

        private void PreviewYearPage_Click(object sender, RoutedEventArgs e)
        {
            var firstPageYear = YearContainer.Items[0];
            YearContainer.ItemsSource = GetAdjacentYears((int)firstPageYear);
            YearSelectorText.Text = $"{YearContainer.Items[0]} - {YearContainer.Items[^1]}";
        }

        private void NextYearPage_Click(object sender, RoutedEventArgs e)
        {
            var endPageYear = YearContainer.Items[^1];
            YearContainer.ItemsSource = GetAdjacentYears((int)endPageYear);
            YearSelectorText.Text = $"{YearContainer.Items[0]} - {YearContainer.Items[^1]}";
        }
        #endregion

        #region 各项事件
        private void MainGrid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TimePopup.IsOpen = !TimePopup.IsOpen;
            HourListBox.SelectedIndex = SelectHourValue;
            MinuteListBox.SelectedIndex = SelectMinuteValue;
            SecondListBox.SelectedIndex = SelectSecondValue;
        }

        private void PreviewYear_Click(object sender, RoutedEventArgs e)
        {
            SelectYearValue--;
        }

        private void PreviewMonth_Click(object sender, RoutedEventArgs e)
        {
            if(SelectMonthValue!=1) { 
                SelectMonthValue--; }
            else
            {
                SelectYearValue--;
                SelectMonthValue = 12;
            }
        }

        private void YearSelector_Click(object sender, RoutedEventArgs e)
        {
            SwicthVisibility(3);
        }

        private void MonthSelector_Click(object sender, RoutedEventArgs e)
        {
            SwicthVisibility(2);
        }

        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            if (SelectMonthValue != 1)
            {
                SelectMonthValue++;
            }
            else
            {
                SelectYearValue++;
                SelectMonthValue = 1;
            }

        }

        private void NextYear_Click(object sender, RoutedEventArgs e)
        {
            SelectYearValue++;
        }

        private void YearSelect_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                SelectYearValue = (int)btn.Content;
                SwicthVisibility(1);
            }
        }

        private void MonthSelect_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                SelectMonthValue = (int)btn.Content;
                SwicthVisibility(1);
            }
        }
        private void TimeConfrim_Click(object sender, RoutedEventArgs e)
        {
            TimePopup.IsOpen = !TimePopup.IsOpen;
            SelectHourValue = HourListBox.SelectedIndex;
            SelectMinuteValue = MinuteListBox.SelectedIndex;
            SelectSecondValue = SecondListBox.SelectedIndex;
            OnTimeChanged();
        }

        private void TimeCancel_Click(object sender, RoutedEventArgs e)
        {
            TimePopup.IsOpen = !TimePopup.IsOpen;
        }

        private void HourTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox box)
            {
                if (int.TryParse(box.Text, out var value))
                {
                    SelectHourValue = value;
                    box.BorderBrush = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    box.BorderBrush = new SolidColorBrush(Colors.Red);
                }
            }

        }

        private void MinuteTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox box)
            {
                if (int.TryParse(box.Text, out var value))
                {
                    SelectMinuteValue = value;
                    box.BorderBrush = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    box.BorderBrush = new SolidColorBrush(Colors.Red);
                }
            }
        }

        private void SecondTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox box)
            {
                if (int.TryParse(box.Text, out var value))
                {
                    SelectSecondValue = value;
                    box.BorderBrush = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    box.BorderBrush = new SolidColorBrush(Colors.Red);
                }
            }
        }

        private void DateCancel_Click(object sender, RoutedEventArgs e)
        {
            TryClose();
        }

        private void DateConfirm_Click(object sender, RoutedEventArgs e)
        {
            var dayString = string.Empty;
            foreach (var item in DayContainer.Items)
            {
                if (((CheckGroupItem)item).IsSelect)
                {
                    dayString = ((CheckGroupItem)item).Content;
                }
            }
            if (int.TryParse(dayString, out var day))
            {
                Confirmed?.Invoke(new DateTime(SelectYearValue, SelectMonthValue, day, SelectHourValue, SelectMinuteValue, SelectSecondValue), null);
                TryClose(true);
            }
            else
            {
                MessageBox.Show("请正确选择日期");
            }

        }

        #endregion
    }
    public partial class CheckGroupItem : ObservableObject
    {
        [ObservableProperty]
        private string _content;
        [ObservableProperty]
        private bool _isSelect = false;
        [ObservableProperty]
        private bool _isCurrentMonth = false;
    }
}
