using System;
using System.ComponentModel;
using System.Reflection;
using System.Web;

namespace TaskTip.Common
{
    

    public enum OperationRequestType
    {
        [Description("默认")]
        Default,
        [Description("增加")]
        Add,
        [Description("更新")]
        Update,
        [Description("删除")]
        Delete,
        [Description("删除并删除文件")]
        DeleteNotWithFile,
        [Description("移动")]
        Move,
        [Description("完成")]
        Completed,
    }

    public enum LinkType
    {
        [Description("FilePath")]
        File,
        [Description("DirPath")]
        Directory,
        [Description("WebLinkPath")]
        WebLink,
    }

    public enum SaveImageType
    {
        [Description("PNG")] PNG,
        [Description("JPEG")] JPEG,
    }

    public enum TaskStatusType
    {
        [Description("默认")]
        Default,
        [Description("完成")]
        Complete,
        [Description("超时")]
        Timeout,
        [Description("执行中")]
        Runtime,
        [Description("未定义")]
        Undefined
    }

    public enum DownloadStatusType
    {
        [Description("默认")]
        Default, 
        [Description("执行中")]
        Runtime,
        [Description("完成")]
        Complete,
        [Description("失败")]
        Failure,
        [Description("可更新")]
        CanUpdate,
    }

    public enum LogType
    {
        [Description("Info")] Info,
        [Description("Debug")] Debug,
        [Description("Warning")] Warning,
        [Description("Error")] Error,
    }

    public enum SyncFileCategory
    {
        [Description("任务计划")] TaskPlan,
        [Description("记录")] Record,
        [Description("记录文件内容")] RecordFile
    }

    public enum SyncCommand
    {
        [Description("轮询")] Similar,
        [Description("支持")] Support,
        [Description("请求同步")] SyncRequest,
        [Description("请求文件内容")] FileRequest,
        [Description("推送")] Push,
        [Description("请求修改")] Modify,
        [Description("请求保留原内容")] Keep
    }
}
