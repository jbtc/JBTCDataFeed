using Newtonsoft.Json;

namespace JBTCDataFeedWebUtility
{
    internal class GetFeedDataRequestData
    {
        [JsonProperty("requestData")]
        public ReqData requestData;
    }

    internal class ReqData
    {
        [JsonProperty("owner")]
        public string owner;
        [JsonProperty("site")]
        public string site;
        [JsonProperty("taggroup")]
        public string taggroup;
    }
}