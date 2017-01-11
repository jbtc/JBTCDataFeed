using Newtonsoft.Json;
using System.Collections.Generic;

namespace JBTCDataFeedWebUtility
{
    /// <summary>
    /// 
    /// -- insert TagsFeed - [OWNER],[SITE],[TAGGROUP],[TAG],[ACTION]
    /// 
    /// </summary>
    public class InsertTagsFeedJsonRequestData
    {
        /*
         * 
         * {
         *  "requestData":
         *      {
         *          "uid":"20161206175606",
         *          "owner":"HWR" ,
         *          "site":"RDU",
         *          "taggroup":"SMS",
         *          "tag":"sinus",          
         *          "action": ""
         *      }
         *  }  
         *  
         */
        [JsonProperty("requestData")]
        public TFRequestData requestData { get; set; }
    }

    public class TFRequestData
    {
        [JsonProperty("uid")]
        public string uid;
        [JsonProperty("owner")]
        public string owner;
        [JsonProperty("site")]
        public string site;
        [JsonProperty("taggroup")]
        public string taggroup;
        [JsonProperty("tag")]
        public string tag;
        [JsonProperty("action")]
        public string action;
        [JsonProperty("rid")]
        public string rid;
    }
}