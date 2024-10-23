using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TaskTip.Enums;

namespace TaskTip.Models.DataModel
{
    internal class TcpDataModel
    {
        public string Key;
        public bool IsKeep;
        public SyncCommand CommandType;
        public TcpRequestData? RequestData;
    }

    public class TcpRequestData
    {
        public string GUID;
        public OperationRequestType OperationType;
        public SyncFileCategory SyncCategory;
        public dynamic FileData;

        public override bool Equals(object? obj)
        {
            var isEquals = true;
            if (obj is not TcpRequestData data) return false;
            if (GUID != data.GUID) return false;
            if (OperationType != data.OperationType) return false;
            if (SyncCategory != data.SyncCategory) return false;
            if (JsonConvert.SerializeObject(FileData) != JsonConvert.SerializeObject(data.FileData)) return false;
            return isEquals;
        }
    }
}
