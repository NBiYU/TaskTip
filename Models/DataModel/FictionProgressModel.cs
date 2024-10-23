using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TaskTip.Models.DataModel
{
    public class FictionProgressModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string? Description { get; set; }
        public string LastUpdateTime { get; set; }
        public string CoverImageSource { get; set; }
        public int LastChapterReadIndex { get; set; }
        public int LastContentReadIndex { get; set; }

    }
}
