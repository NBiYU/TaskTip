using System;
using System.Collections.Generic;
using System.Text;

namespace TaskTip.Extends.FictionAPI.LRY_API
{
    public class FictionsLRY_APIResponseModel
    {

        /// <summary>
        /// �ɹ�
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<DataListItem> data { get; set; }
    }

    public class ChaptersLRY_APIResponseModel
    {
        /// <summary>
        /// �ɹ�
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ChapterListItem data { get; set; }
    }

    public class FictionContentLRY_APIResponseModel
    {
        /// <summary>
        /// �ɹ�
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int count { get; set; }
        public List<string> data { get; set; }
    }

    public class DataListItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string fictionId { get; set; }
        /// <summary>
        /// ȫְ����
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        public string author { get; set; }
        /// <summary>
        /// ����С˵
        /// </summary>
        public string fictionType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string descs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cover { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string updateTime { get; set; }
    }
    public class ChapterListItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string fictionId { get; set; }
        /// <summary>
        /// ȫְ����
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        public string author { get; set; }
        /// <summary>
        /// ����С˵
        /// </summary>
        public string fictionType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string descs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cover { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string updateTime { get; set; }
        public List<Chapter> chapterList { get; set; }
    }

    public class Chapter
    {
        public string title { get; set; }
        public string chapterId { get; set; }
    }

    //public class JsonDefaultValue : JsonConverter<object>
    //{
    //    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    //    {
    //        // �ڷ����л�ʱ�Զ��崦��Ĭ��ֵ
    //        if (reader.TokenType == JsonTokenType.Null)
    //        {
    //            return null;
    //        }
    //        return null;
    //    }

    //    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    //    {
    //        // �����л�ʱ�Զ��崦��Ĭ��ֵ
    //        if (value == null)
    //        {
    //            writer.WriteStartArray();
    //        }
    //        else
    //        {
    //            // ��д��Ĭ��ֵ
    //        }
    //    }
    //}
}
