using Newtonsoft.Json;
using System.Collections.Generic;

namespace JBTCDataFeedWebUtility
{
    /// <summary>
    /// 
    /// -- insert TagSubscribers - [OWNER],[EMAIL],[PHONE],[SMSEMAIL]
    /// 
    /// </summary>
    public class InsertTagSubscribersJsonRequestData
    {
        /*
        * 
        * {
        *  "requestData":
        *      {
        *          "uid":"20161206175606",
        *          "owner":"HWR" ,
        *          "email":"wright.hans@gmail.com",
        *          "phone":"5022331048",          
        *          "smsemail": "5022331048@txt.voice.google.com"
        *      }
        *  }  
        *  
        */
        [JsonProperty("requestData")]
        public TSRequestData requestData { get; set; }
    }

    public class TSRequestData
    {
        [JsonProperty("uid")]
        public string uid;
        [JsonProperty("owner")]
        public string owner;
        [JsonProperty("email")]
        public string email;
        [JsonProperty("phone")]
        public string phone;
        [JsonProperty("smsemail")]
        public string smsemail;
        [JsonProperty("rid")]
        public string rid;
    }
}