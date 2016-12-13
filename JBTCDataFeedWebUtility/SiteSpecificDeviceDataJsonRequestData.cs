﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace JBTCDataFeedWebUtility
{
    public class SiteSpecificDeviceDataJsonRequestData
    {

        /*
        * 
        * {
        *  "requestData":
        *      {
        *          "uid":"20161206175606",
        *          "site":"EWR" ,
        *          "user":null,
        *          "data":
        *              {
        *                  "emails":"tester@jbt.co m;supervisor@jbt.com",
        *                  "schedule":"60",
        *                  "WhereClause":"TermC.Zone1.GateC90.GPU.GPUSTATUSBOOLEAN = 1 AND TermC.Zon e1.GateC90.GPU.ON 2 = 1 AND TermC.Zone1.GateC90.GPU.ON 2 = 1 AND Ter mC.Zone1.GateC90.GPU.MODE = 'Standby' AND ( TermC.Zone1.GateC90. GPU.MODE = 'Standby' OR TermC.Zone1.GateC90.GPU.MODE = 'AC  Run' ) "
        *              }
        *      }
        *  }  
        *  
        *  %7B%22requestData%22%3A%7B%22uid%22%3A%2220161206175606%22%2C%22site%22%3A%22EWR%22+%2C%22user%22%3Anull%2C%22data%22%3A%7B%22emails%22%3A%22tester%40jbt.co+m%3Bsupervisor%40jbt.com%22%2C%22schedule%22%3A%2260%22%2C%22WhereClause%22%3A%22TermC.Zone1.GateC90.GPU.GPUSTATUSBOOLEAN+%3D+1+AND+TermC.Zon+e1.GateC90.GPU.ON+2+%3D+1+AND+TermC.Zone1.GateC90.GPU.ON+2+%3D+1+AND+Ter+mC.Zone1.GateC90.GPU.MODE+%3D+%27Standby%27+AND+%28+TermC.Zone1.GateC90.+GPU.MODE+%3D+%27Standby%27+OR+TermC.Zone1.GateC90.GPU.MODE+%3D+%27AC++Run%27%29%22%7D%7D%7D
        */
        [JsonProperty("requestData")]
        public RequestData requestData { get; set; }
    }

    public class RequestData
    {
        [JsonProperty("uid")]
        public string uid;
        [JsonProperty("site")]
        public string site;
        [JsonProperty("user")]
        public string user;
        [JsonProperty("data")]
        public SiteSpecificDeviceDataJsonRequestDataDATA data;
    }
    public class SiteSpecificDeviceDataJsonRequestDataDATA
    {
        [JsonProperty("emails")]
        public string emails;
        [JsonProperty("schedule")]
        public string schedule;
        [JsonProperty("WhereClause")]
        public string WhereClause;
    }
}