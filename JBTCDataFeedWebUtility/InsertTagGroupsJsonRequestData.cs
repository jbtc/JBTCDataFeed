using Newtonsoft.Json;
using System.Collections.Generic;

namespace JBTCDataFeedWebUtility
{
    /// <summary>
    /// 
    /// -- insert TagGroups - [OWNER],[SITE],[TAGGROUP],[ENABLED]
    /// 
    /// </summary>
    public class InsertTagGroupsJsonRequestData
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
            *          "enabled":"TRUE"
            *      }
            *  }  
            *  
            */
        [JsonProperty("requestData")]
        public TGRequestData requestData { get; set; }
    }

    public class TGRequestData
    {
        [JsonProperty("uid")]
        public string uid;
        [JsonProperty("owner")]
        public string owner;
        [JsonProperty("site")]
        public string site;
        [JsonProperty("taggroup")]
        public string taggroup;
        [JsonProperty("enabled")]
        public string enabled;        
    }
}