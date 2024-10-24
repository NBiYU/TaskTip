using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTip.Models.DataModel
{
    public class PopInteractiveModel
    {
        public string Title { get; set; }
        public List<PopInteractiveItemModel> InteractiveItemModels { get; set; }
    }
    public class PopInteractiveItemModel
    {
        public string Tip { get; set; }
        public object Data { get; set; }
        public object SelectData { get; set; }
        public string InputRegexRule { get; set; }
        public ControlTypeEnum ControlType { get; set; }
    }

    public enum ControlTypeEnum
    {
        [Description("文本")]
        Text,
        [Description("输入")]
        Input,
        [Description("下拉")]
        Dropdown,
        [Description("多选")]
        CheckBox,
        [Description("自定义")]
        Custom
    }

}
