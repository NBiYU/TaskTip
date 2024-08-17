using TaskTip.Extends.ConnectClient;
using TaskTip.Extends.FictionAPI.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TaskTip.Extends.ConnectClient.Model;
using TaskTip.Extends.FictionAPI.OptionEnum;

namespace TaskTip.Extends.FictionAPI.LRY_API
{
    public class LRY_APIRequest
    {
        private string ConnectUrl { get; set; }
        private HttpRequest HttpRequest { get; set; }

        //private JsonSerializerOptions options = new JsonSerializerOptions
        //{
        //    Converters = { new JsonDefaultValue() }
        //};
        public async Task<FictionsLRY_APIResponseModel?> SearchCategoryFiction(LRY_APIFictionCagetory cagetory, int startIndex, int size)
        {
            return await SearchFiction(LRY_APIOptionEnum.SearchCategory, cagetory.GetDesc(), startIndex, size);
        }

        public async Task<FictionsLRY_APIResponseModel?> SearchFiction(LRY_APIOptionEnum option, string searchKey, int startIndex, int size)
        {
            var searchTargetType = option switch
            {
                LRY_APIOptionEnum.SearchTitle => "title",
                LRY_APIOptionEnum.SearchAuthor => "author",
                LRY_APIOptionEnum.SearchCategory => "fictionType"
            };

            if (startIndex < 1 || startIndex > size) throw new Exception("startIndex 需要为大于 1 的正整数 并且 不能大于 size");
            if (size < 1 || size - startIndex > 30) throw new Exception("size与startIndex的差 需要为小于 30 大于 0 的正整数");

            var result = await Send("https://api.pingcc.cn/fiction/search/",
                HttpMethod.Get,
                "",
                new Dictionary<string, string>()
                {
                                {"option", searchTargetType},
                                {"key", searchKey },
                                {"from",startIndex.ToString() },
                                {"limit", size.ToString() },
                });

            return JsonConvert.DeserializeObject<FictionsLRY_APIResponseModel>(result.Content);
            //return JsonSerializer.Deserialize<FictionsLRY_APIResponseModel>(result.Content, options);
        }
        public async Task<ChaptersLRY_APIResponseModel?> SearchChapter(string fictionId)
        {
            var result = await Send($"https://api.pingcc.cn/fictionChapter/search/{fictionId}", HttpMethod.Get, "");
            return JsonConvert.DeserializeObject<ChaptersLRY_APIResponseModel>(result.Content);
            //return  JsonSerializer.Deserialize<ChaptersLRY_APIResponseModel>(result.Content, options);

        }
        public async Task<FictionContentLRY_APIResponseModel?> SearchContent(string chapterId)
        {
            var result = await Send($"https://api.pingcc.cn/fictionContent/search/{chapterId}", HttpMethod.Get, "");
            return JsonConvert.DeserializeObject<FictionContentLRY_APIResponseModel>(result.Content);
            //return JsonSerializer.Deserialize<FictionContentLRY_APIResponseModel>(result.Content, options);
        }
        public async Task<ResponseModel> Send(string url, HttpMethod method, string content, Dictionary<string, string> queryParameter = null)
        {
            var httpRequest = new HttpRequest();
            if (queryParameter != null)
            {
                var quertString = string.Join("/", queryParameter.Values);
                url += quertString;
            }

            var result = await httpRequest.SendAsync(new RequestModel()
            {
                Content = content,
                Method = method,
                Url = url
            });
            if (result.StatusCode != 200)
            {
                throw new Exception($"获取数据失败：{result.Message}");
            }
            return result;
        }

        public LRY_APIRequest()
        {
            ConnectUrl = "https://api.pingcc.cn/fictionContent/search/";
        }

        public LRY_APIRequest(string url)
        {
            ConnectUrl = url;
        }

    }
}
