using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskTip.Common.Converter.Version
{
    public class BaseConverter
    {
        protected IProgress<int> TotalProgres { get; set; }
        protected IProgress<int> CurrentProgress { get; set; }
        protected CancellationToken StopToken { get; set; }
        public BaseConverter(IProgress<int> total, IProgress<int> progress, CancellationToken token) {
            TotalProgres = total;
            CurrentProgress = progress;
            StopToken = token;
        }
    }
}
