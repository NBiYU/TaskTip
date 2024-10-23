using TaskTip.Extends.ConnectClient;
using TaskTip.Extends.ConnectClient.Model;
using TaskTip.Extends.FictionAPI.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaskTip.Extends.FictionAPI.OptionEnum;

namespace TaskTip.Extends
{
    internal interface IFictionAPI
    {
        string ConnectUrl { get; set; }


        /// <summary>
        /// 查询小说
        /// </summary>
        /// <param name="option">查询类型</param>
        /// <param name="searchKey">查询内容</param>
        /// <param name="startIndex">结果起始位置</param>
        /// <param name="size">结果数量，最大30</param>
        /// <returns></returns>
        public Task<FictionResponseModel> SearchFiction(LRY_APIOptionEnum option, string searchKey, int startIndex, int size);
        /// <summary>
        /// 章节查询
        /// </summary>
        /// <param name="fictionId">小说ID，通过查询小说获取</param>
        /// <returns></returns>
        public Task<FictionResponseModel> SearchChapter(string fictionId);
        /// <summary>
        /// 小说内容
        /// </summary>
        /// <param name="chapterId">章节ID,通过查询章节获取</param>
        /// <returns></returns>
        public Task<FictionResponseModel> SearchContent(string chapterId);

    }
}
