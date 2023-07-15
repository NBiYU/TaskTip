using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTip.Services
{
    public static class Const
    {
        public static readonly string CONST_SHOW_CUSTOM = nameof(CONST_SHOW_CUSTOM);        //设置界面状态变更
        public static readonly string CONST_CONFIG_CHANGED = nameof(CONST_CONFIG_CHANGED);   //配置变更
        public static readonly string CONST_TASK_LIST_CHANGED = nameof(CONST_TASK_LIST_CHANGED);  //任务列表变更
        public static readonly string CONST_TASK_RELOAD = nameof(CONST_TASK_RELOAD);        //任务内容重新加载
        public static readonly string CONST_MENO_RELOAD = nameof(CONST_MENO_RELOAD);          //记事内容重新加载
        public static readonly string CONST_FLAOTING_SIZE_CHANGED = nameof(CONST_FLAOTING_SIZE_CHANGED); //悬浮窗界面变更
        public static readonly string CONST_DELETE_LISTITEM = nameof(CONST_DELETE_LISTITEM);  //删除对应列表行
        public static readonly string CONST_COMPLETE_TASK_GUID = nameof(CONST_COMPLETE_TASK_GUID); //任务完成状态变更
        public static readonly string CONST_SCHEDULE_CREATE = nameof(CONST_SCHEDULE_CREATE);    //定时任务创建
        public static readonly string CONST_DATETIME_RETURN = nameof(CONST_DATETIME_RETURN);   //时间窗口返回
        public static readonly string CONST_OPEN_APPLICATTION = nameof(CONST_OPEN_APPLICATTION); //打开应用时
        public static readonly string CONST_NOTIFY_RECORD_ITEM = nameof(CONST_NOTIFY_RECORD_ITEM); //通知父控件删除子控件
        public static readonly string CONST_LOAD_RECORD_FILE = nameof(CONST_LOAD_RECORD_FILE);   //加载选中文件
        public static readonly string CONST_RECORD_SAVE_TITLE = nameof(CONST_RECORD_SAVE_TITLE);   //加载选中文件
        public static readonly string CONST_RECORD_SAVE_CONFIG = nameof(CONST_RECORD_SAVE_CONFIG);  //保持目录树配置
    }
}
