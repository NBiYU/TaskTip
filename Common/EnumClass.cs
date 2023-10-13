using System;
using System.ComponentModel;
using System.Reflection;
using System.Web;

namespace TaskTip.Common
{
    

    public enum OperationRequestType
    {
        [Description("Ĭ��")]
        Default,
        [Description("����")]
        Add,
        [Description("����")]
        Update,
        [Description("ɾ��")]
        Delete,
        [Description("�ƶ�")]
        Move,
        [Description("���")]
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
        [Description("Ĭ��")]
        Default,
        [Description("���")]
        Complete,
        [Description("��ʱ")]
        Timeout,
        [Description("ִ����")]
        Runtime,
        [Description("δ����")]
        Undefined
    }

    public enum DownloadStatusType
    {
        [Description("Ĭ��")]
        Default, 
        [Description("ִ����")]
        Runtime,
        [Description("���")]
        Complete,
        [Description("ʧ��")]
        Failure,
        [Description("�ɸ���")]
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
        [Description("����ƻ�")] TaskPlan,
        [Description("��¼")] Record,
        [Description("��¼�ļ�����")] RecordFile
    }

    public enum SyncCommand
    {
        [Description("��ѯ")] Similar,
        [Description("֧��")] Support,
        [Description("����ͬ��")] SyncRequest,
        [Description("�����ļ�����")] FileRequest,
        [Description("����")] Push,
        [Description("�����޸�")] Modify,
        [Description("������ԭ����")] Keep
    }
}
