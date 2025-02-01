using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTip.Common.Extends;
using TaskTip.Services;
using TaskTip.Models.DataModel;
using TaskTip.Models.Entities;
using TaskTip.Models.Enums;
using TaskTip.Models.ViewDataModels;

namespace TaskTip.Common
{
    public class OperationRecord
    {
        public static bool isRecording = true;
        public static bool OperationRecordWrite(TcpRequestData op)
        {

            //GlobalVariable.OperationRecordPath =
            //    "E:\\vs2020\\TaskTipProject\\bin\\Debug\\net7.0-windows\\OperationRecordList.json";
            try
            {
                if (!isRecording) return false;

                var existsedList = OperationRecordRead();
                //优化处理
                existsedList.RemoveAll(x => x.Equals(op));

                if (op.FileData != null)
                {
                    op.FileData = op.SyncCategory switch
                    {
                        SyncFileCategory.TaskPlan => JsonConvert.DeserializeObject<TaskFileModel>(JsonConvert.SerializeObject(op.FileData)),
                        SyncFileCategory.Record => JsonConvert.DeserializeObject<TreeInfo>(JsonConvert.SerializeObject(op.FileData)),
                    };
                }

                switch (op.OperationType)
                {
                    case OperationRequestType.Add:
                        var deleteOp = existsedList.Where(x => x.GUID == op.GUID && x.OperationType == OperationRequestType.Delete).ToList();
                        if (deleteOp.Count != 0)
                            deleteOp.RemoveAt(0);
                        existsedList = existsedList.Except(deleteOp).ToList();
                        break;
                    case OperationRequestType.Update:
                        var addOpToUpdate = existsedList.Where(x => x.FileData != null && x.FileData?.GUID.ToString() == op.GUID && x.OperationType == OperationRequestType.Add).ToList();
                        existsedList = existsedList.Except(addOpToUpdate).ToList();
                        op.OperationType = OperationRequestType.Add;
                        break;
                    case OperationRequestType.Delete:
                        var addOp = existsedList.Where(x => x.FileData != null && x.FileData?.GUID.ToString() == op.GUID && x.OperationType == OperationRequestType.Add).ToList();
                        if (addOp.Count != 0)
                            addOp.RemoveAt(0);
                        existsedList = existsedList.Except(addOp).ToList();
                        break;
                    case OperationRequestType.Move:
                        break;
                }

                existsedList.Add(op);

                File.WriteAllText(GlobalVariable.OperationRecordPath, JsonConvert.SerializeObject(existsedList, Formatting.Indented));
                GlobalVariable.LogHelper.Info($"【操作记录】【{op.SyncCategory.GetDesc()}】【{op.OperationType.GetDesc()}】【{op.GUID}】记录完成");
                return true;
            }
            catch (Exception ex)
            {
                GlobalVariable.LogHelper.Error($"【操作记录】【{op.OperationType.GetDesc()}】【{op.OperationType.GetDesc()}】【{op.GUID}】【异常】写入{JsonConvert.SerializeObject(op, Formatting.Indented)}操作失败,异常信息：{ex}");
                return false;
            }
        }


        public static List<TcpRequestData> OperationRecordRead()
        {
            var text = string.Empty;

            if (File.Exists(GlobalVariable.OperationRecordPath))
                text = File.ReadAllText(GlobalVariable.OperationRecordPath);

            if (string.IsNullOrEmpty(text)) return new List<TcpRequestData>();

            return JsonConvert.DeserializeObject<List<TcpRequestData>>(text);
        }
    }
}
