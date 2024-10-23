using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTip.Models.CommonModel
{
    public class OptionModel<T>
    {
        public string Name { get; set; }
        public T Value { get; set; }
    }
}
