using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TaskTip.ViewModels.ToolPageVM
{
    public partial class RegexVM : ObservableObject
    {
        #region 属性
        private string _inputRule;
        public string InputRule { get => _inputRule; set => SetProperty(ref _inputRule, value); }
        private string _inputContent;
        public string InputContent { get => _inputContent; set => SetProperty(ref _inputContent, value); }
        private string _matchingTotal;
        public string MatchingTotal { get => _matchingTotal; set => SetProperty(ref _matchingTotal, value); }
        private string _selectRetouch;
        public string SelectRetouch { get => _selectRetouch; set { SetProperty(ref _selectRetouch, value); InputChanged(); } }
        private FlowDocument _highLightContent;
        public FlowDocument HighLightContent { get => _highLightContent; set => SetProperty(ref _highLightContent, value); }

        public ObservableCollection<string> RetouchingCollection { get; set; } = new();
        public ObservableCollection<string> MatchingCollection { get; set; } = new();


        #endregion 属性

        #region 指令

        [RelayCommand]
        public void ShowHelp()
        {
            System.Windows.MessageBox.Show("Help");
        }
        [RelayCommand]
        public void RuleChanged()
        {
            InputChanged();
        }

        [RelayCommand]
        public void InputChanged()
        {
            if (string.IsNullOrEmpty(InputContent) || string.IsNullOrEmpty(InputRule)) return;
            MatchingCollection.Clear();
            var tempRule = InputRule;
            var tempString = InputContent;
            var tempRetouch = SelectRetouch switch
            {
                "n" => RegexOptions.None,
                "ic" => RegexOptions.IgnoreCase,
                "m" => RegexOptions.Multiline,
                "ex" => RegexOptions.ExplicitCapture,
                "c" => RegexOptions.Compiled,
                "s" => RegexOptions.Singleline,
                "iw" => RegexOptions.IgnorePatternWhitespace,
                "rtl" => RegexOptions.RightToLeft,
                "ec" => RegexOptions.ECMAScript,
                "ci" => RegexOptions.CultureInvariant,
                _ => RegexOptions.None,
            };
            var paragraph = new Paragraph();
            try
            {
                var matchResult = Regex.Matches(tempString, InputRule, tempRetouch);
                MatchingTotal = $"{matchResult.Count}";
                if (matchResult.Count != 0)
                {
                    foreach (var item in matchResult)
                    {
                        var itemStr = $"{item}";
                        MatchingCollection.Add(itemStr);
                        var matchIndex = tempString.IndexOf(itemStr);
                        if (matchIndex != 0)
                        {
                            var skip = matchIndex == -1 ? tempString.Length : matchIndex;
                            paragraph.Inlines.Add(new Run(tempString[..skip]));
                        }
                        paragraph.Inlines.Add(new Run(itemStr) { Background = new SolidColorBrush(Colors.Red) });
                        if (!string.IsNullOrEmpty(tempString))
                            tempString = tempString.Split(itemStr, 2)[1];
                    }
                    if (tempString.Length > 0) paragraph.Inlines.Add(new Run(tempString));
                    HighLightContent = new FlowDocument(paragraph);
                    return;
                }

            }
            catch (Exception ex) { }
            paragraph.Inlines.Add(new Run(tempString));
            HighLightContent = new FlowDocument(paragraph);
        }
        #endregion

        #region 初始化
        public RegexVM()
        {
            MatchingTotal = $"{MatchingCollection.Count}";
            RetouchingCollection.Add("n");
            RetouchingCollection.Add("ic");
            RetouchingCollection.Add("m");
            RetouchingCollection.Add("ex");
            RetouchingCollection.Add("c");
            RetouchingCollection.Add("s");
            RetouchingCollection.Add("iw");
            RetouchingCollection.Add("rtl");
            RetouchingCollection.Add("ec");
            RetouchingCollection.Add("ci");
            SelectRetouch = RetouchingCollection[0];
        }
        #endregion
    }
}
