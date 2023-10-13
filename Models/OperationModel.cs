using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TaskTip.Common;

namespace TaskTip.Models
{
    public class OperationModel
    {
        public string GUID;
        public SyncFileCategory FileCategory;
        public OperationRequestType OperationType;
        public bool IsSync;
    }
}
