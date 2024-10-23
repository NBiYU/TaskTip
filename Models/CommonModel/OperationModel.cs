using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TaskTip.Enums;

namespace TaskTip.Models.CommonModel
{
    public class OperationModel
    {
        public string GUID;
        public SyncFileCategory FileCategory;
        public OperationRequestType OperationType;
        public bool IsSync;
    }
}
