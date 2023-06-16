using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class News
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("like")]
        public bool Like { get; set; }

        [JsonProperty("dislike")]
        public bool Dislike { get; set; }

        //public SolidColorBrush LikeColor
        //{
        //    get
        //    {
        //        if (Like)
        //            return new SolidColorBrush(Colors.BlueViolet);
        //        return new SolidColorBrush(Colors.Gray);
        //    }
        //}

        //public SolidColorBrush DislikeColor
        //{
        //    get
        //    {
        //        if (Dislike)
        //            return new SolidColorBrush(Colors.BlueViolet);
        //        return new SolidColorBrush(Colors.Gray);
        //    }
        //}
    }
}
