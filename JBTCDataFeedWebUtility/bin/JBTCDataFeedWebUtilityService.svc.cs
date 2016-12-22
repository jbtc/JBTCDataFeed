using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Serialization;
using Utilities;
using static Utilities.Logging;

namespace JBTCDataFeedWebUtility
{
    /// <summary>
    /// This purpose of this service is to convey configuration 
    /// for the feeds to be recorded 
    /// Feed configuration will be stored in the db
    /// No AdHoc queries are run from this service
    /// in order to add configuration the following flow has to be followed
    /// -- insert user
    /// -- insert TagSubscribers - [OWNER],[EMAIL],[PHONE],[SMSEMAIL]
    /// -- insert TagGroups - [OWNER],[SITE],[ENABLED],[TAGGROUP]
    /// -- insert TagsFeed - [OWNER],[SITE],[TAGGROUP],[TAG],[ACTION]
    /// 
    /// </summary>
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class JBTCDataFeedWebUtilityService
    {
        // response values for client
        private enum WebUtilityReponseCode : int
        {
            OK = 0,
            InvalidJson = 100,                  // Response JSON related Group  
            PleaseRequestKey = 101,
            InvalidKey = 200,                   // Authentication Issue Group
            DuplicateEntryDB = 300,             // DataBase Issue Group
            RequestIsMissingParameter = 400,    // Request Parameter Group 
            InvalidRequestType = 401,           // could not parse request type
            GeneralFailure = 999,               // Kitchen Sink

        }

        private enum ReqyestTypeCode : int
        {
            SiteSpecificDeviceData = 1,
            InsertTagSubscribers = 2,
            InsertTagGroups = 3,
            InsertTagsFeed=4,
            Test = 9999
        }

        private static Dictionary<string, SessionKey> UserKeysByIp = new Dictionary<string, SessionKey>();

        // HINT About Decorations ..
        // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
        // To create an operation that returns XML,
        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
        //     and include the following line in the operation body:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";


        /// <summary>
        /// Handles the work requests.  client has to submit key
        /// Call response is based on enumerator(WebUtilityReponseCode) plus JSONdata.
        /// </summary>
        /// <returns>JSON data</returns>
        [OperationContract]
        [WebGet]
        public string DoWork()
        {
            /*
             * you call it this way
             * http://localhost/JBTCDataFeedWebUtility/JBTCDataFeedWebUtilityService.svc/DoWork?user=123&pwd=234
             http://localhost:49501/JBTCDataFeedWebUtilityService.svc/DoWork?RequestType=1&RequestData=234

            http://localhost:49501/JBTCDataFeedWebUtilityService.svc/DoWork?RequestType=1&RequestData=%7B%22requestData%22%3A%7B%22uid%22%3A%2220161206175606%22%2C%22site%22%3A%22EWR%22+%2C%22user%22%3Anull%2C%22data%22%3A%7B%22emails%22%3A%22tester%40jbt.co+m%3Bsupervisor%40jbt.com%22%2C%22schedule%22%3A%2260%22%2C%22WhereClause%22%3A%22TermC.Zone1.GateC90.GPU.GPUSTATUSBOOLEAN+%3D+1+AND+TermC.Zon+e1.GateC90.GPU.ON+2+%3D+1+AND+TermC.Zone1.GateC90.GPU.ON+2+%3D+1+AND+Ter+mC.Zone1.GateC90.GPU.MODE+%3D+%27Standby%27+AND+%28+TermC.Zone1.GateC90.+GPU.MODE+%3D+%27Standby%27+OR+TermC.Zone1.GateC90.GPU.MODE+%3D+%27AC++Run%27%29%22%7D%7D%7D
             */
            NameValueCollection queryStringCol =
                WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;

            string parameters = string.Empty;
            Dictionary<string, string> pars = new Dictionary<string, string>();
            if (queryStringCol != null && queryStringCol.Count > 0)
            {
                for (int i = 0; i < queryStringCol.Count; i++)
                {
                    pars.Add(queryStringCol.AllKeys[i], queryStringCol[i]);

                }
            }


            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string ip = endpoint.Address;
            ResponseData r = new ResponseData();
            if (UserKeysByIp.ContainsKey(ip))
            {
                /*
                 * structure of request parameters
                 * RequestType = name of the request - index 0
                 * RequestData = JSON input data = index 1
                 */
                if (pars.ContainsKey("RequestType"))
                {
                    if (pars.ContainsKey("RequestData"))
                    {
                        int reqType = 0;
                        if (int.TryParse(pars["RequestType"], out reqType))
                        {
                            r = DoBusiness(reqType, pars["RequestData"]);
                        }
                        else
                        {
                            r.ResponseKey = (int)WebUtilityReponseCode.InvalidRequestType;
                        }
                    }
                    else
                    {
                        r.ResponseKey = (int)WebUtilityReponseCode.RequestIsMissingParameter;
                    }
                }
                else
                {
                    r.ResponseKey = (int)WebUtilityReponseCode.RequestIsMissingParameter;

                }
            }
            else
            {
                r.ResponseKey = (int)WebUtilityReponseCode.PleaseRequestKey;
            }
            // Add your operation implementation here
            string res = new JavaScriptSerializer().Serialize(r);
            return res;
        }

        /// <summary>
        /// Does the business.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private ResponseData DoBusiness(int requesttype, string requestData)
        {
            ResponseData r = new ResponseData();
            switch (requesttype)
            {
                case (int)ReqyestTypeCode.SiteSpecificDeviceData:
                    #region test
                    //request
                    SiteSpecificDeviceDataJsonRequestData ssddjrd =
                        JsonConvert.DeserializeObject<SiteSpecificDeviceDataJsonRequestData>(requestData);
                    // invoke db object to run query based on 
                    // the selector submitted by the client 
                    // "WhereClause":
                    // "TermC.Zone1.GateC90.GPU.GPUSTATUSBOOLEAN = 1 AND 
                    //  TermC.Zone1.GateC90.GPU.ON2   = 1         AND 
                    //  TermC.Zone1.GateC90.GPU.ON2   = 1         AND 
                    //  TermC.Zone1.GateC90.GPU.MODE  = 'Standby' AND 
                    // ( TermC.Zone1.GateC90.GPU.MODE = 'Standby' OR TermC.Zone1.GateC90.GPU.MODE = 'AC Run' ) "
                    // we pull data from dict
                    // time is limited to cyclical bracket implicitly
                    // we evaluate each statement to true or false
                    // 1. get boundaries of logical statements
                    //  what connects a logical statement is the "="
                    //  we avaluate from let to right until we hit  (
                    //  if we hit a "(" we open another stack level
                    //  -> we count opening and closing brackets if the number is not equal we fail call

                    string w = ssddjrd.requestData.data.WhereClause;

                    //response
                    r.JSON = @"{whereclause:'" + ssddjrd.requestData.data.WhereClause + "'}";
                    r.ResponseKey = (int)WebUtilityReponseCode.OK;
                    #endregion
                    break;
                case (int)ReqyestTypeCode.InsertTagSubscribers:
                    //request
                    InsertTagSubscribersJsonRequestData itsjrd =
                        JsonConvert.DeserializeObject<InsertTagSubscribersJsonRequestData>(requestData);
                    {
                        // new row insert
                        string owner = itsjrd.requestData.owner;
                        string phone = itsjrd.requestData.phone;
                        string email = itsjrd.requestData.email;
                        string smsemail = itsjrd.requestData.smsemail;
                        string execQuery = "INSERT INTO TagSubscribers (OWNER,EMAIL,PHONE,SMSEMAIL) Values('" +
                            owner + "','" +
                            email + "','" +
                            phone + "','" +
                            smsemail + "')";
                        string result = RunQuery(execQuery);
                        writeEventLog("Insert of new TagSubscribers record done for " + owner + " with result: " + (result == "NONE" ? "OK" : result), "DoBusiness", null, Utilities.Logging.LogLevel.Info);

                    }
                    //response
                    //r.JSON = @"{whereclause:'" + ssddjrd.requestData.data.WhereClause + "'}";
                    r.ResponseKey = (int)WebUtilityReponseCode.OK;
                    break;
                case (int)ReqyestTypeCode.InsertTagGroups:
                    //request
                    InsertTagGroupsJsonRequestData itgjrd =
                        JsonConvert.DeserializeObject<InsertTagGroupsJsonRequestData>(requestData);
                    {
                        string execQuery = "insert into TagGroups(OWNER,SITE,TAGGROUP,ENABLED) values('" +
                            itgjrd.requestData.owner + "','" +
                            itgjrd.requestData.site + "','" +
                            itgjrd.requestData.taggroup + "','" +
                            itgjrd.requestData.enabled + "')";
                        string result = RunQuery(execQuery);
                        writeEventLog("Insert of new TagGroups record done for " + itgjrd.requestData.owner + " with result: " + (result == "NONE" ? "OK" : result), "DoBusiness", null, Utilities.Logging.LogLevel.Info);
                    }
                    //response
                    //r.JSON = @"{whereclause:'" + ssddjrd.requestData.data.WhereClause + "'}";
                    r.ResponseKey = (int)WebUtilityReponseCode.OK;
                    break;
                case (int)ReqyestTypeCode.InsertTagsFeed:
                    //request
                    InsertTagsFeedJsonRequestData itfjrd =
                        JsonConvert.DeserializeObject<InsertTagsFeedJsonRequestData>(requestData);
                    {
                        string execQuery = "insert into TagsFeed(OWNER,SITE,TAGGROUP,ACTION) values('" +
                            itfjrd.requestData.owner + "','" +
                            itfjrd.requestData.site + "','" +
                            itfjrd.requestData.taggroup + "','" +
                            itfjrd.requestData.action+ "')";
                        string result = RunQuery(execQuery);
                        writeEventLog("Insert of new TagsFeed record done for " + itfjrd.requestData.owner + " with result: " + (result == "NONE" ? "OK" : result), "DoBusiness", null, Utilities.Logging.LogLevel.Info);
                    }
                    //response
                    //r.JSON = @"{whereclause:'" + ssddjrd.requestData.data.WhereClause + "'}";
                    r.ResponseKey = (int)WebUtilityReponseCode.OK;
                    break;
                default:
                    r.ResponseKey = (int)WebUtilityReponseCode.InvalidRequestType;
                    break;
            }
            return r;
        }

        private static bool evaluateEquality(string key, string value)
        {
            return (key == value ? true : false);
        }


        /// <summary>
        /// Does the heartbeat processing.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        public string DoPing()
        {
            /*
             * you call it this way
             * http://localhost/JBTCDataFeedWebUtility/JBTCDataFeedWebUtilityService.svc/DoPing
             */
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string ip = endpoint.Address;
            return "OK.. you are " + ip;
        }

        /// <summary>
        /// Does the authenticate.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        public string DoAuthenticate()
        {
            /*
             * you call it this way
             * http://localhost:49501/JBTCDataFeedWebUtilityService.svc/DoAuthenticate?user=123&pwd=234
             */
            NameValueCollection queryStringCol =
                WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;

            string parameters = string.Empty;
            if (queryStringCol != null && queryStringCol.Count > 0)
            {
                for (int i = 0; i < queryStringCol.Count; i++)
                {
                    parameters += queryStringCol[i] + "###";
                }
            }

            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string ip = endpoint.Address;

            SessionKey s = new SessionKey();
            s.TimeStamp = DateTime.Now.ToString();
            s.Key = Guid.NewGuid().ToString();
            if (UserKeysByIp.ContainsKey(ip))
            {
                UserKeysByIp[ip] = s;
            }
            else
            {
                UserKeysByIp.Add(ip, s);
            }
            if (queryStringCol != null && queryStringCol.Count > 0)
            {
                return ip + " entered " + parameters; // This will returned as an XML.
            }
            else
            {
                string res = new JavaScriptSerializer().Serialize(s);
                return res;

            }
        }

        #region AUX functions
        /// <summary>
        /// write event log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="name"></param>
        /// <param name="thread"></param>
        /// <param name="logLevel"></param>
        private void writeEventLog(string message, string functionName, string thread = "main", LogLevel logLevel = Utilities.Logging.LogLevel.Info)
        {
            string loglevelname = Utilities.Logging.LoglevelToString(logLevel);
            //string testtimestamp = DateTime.Now.ToString();
            string eventmessage = functionName + " #### " + message + " ####";


            WriteLog(message, functionName, thread, logLevel);
            //Utilities.EventLogLogging evtlg = new Utilities.EventLogLogging(evtJBTCDataPort);// TODO: move event log into Utility lib completely
            //evtlg.WriteEntry(eventmessage, logLevel);
            //evtJBTCDataPort.WriteEntry(eventmessage);
        }

        /// <summary>
        /// Runs the query.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <returns></returns>
        private string RunQuery(string queryString)
        {
            string connectionString = ConfigurationManager.AppSettings["dbConnectionString"];
            string res = "NONE";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        res = reader[0].ToString();
                        Logging.WriteLog("runquery result:" + res, "RunQuery", null, Logging.LogLevel.Info);

                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            return res;
        }
        #endregion 


    }
}

//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.ServiceModel;
//using System.ServiceModel.Activation;
//using System.ServiceModel.Channels;
//using System.ServiceModel.Web;
//using System.Text;
//using System.Web.Script.Serialization;

//namespace JBTCDataFeedWebUtility
//{
//    [ServiceContract(Namespace = "")]
//    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
//    public class JBTCDataFeedWebUtilityService
//    {
//        // response values for client
//        private  enum   WebUtilityReponseCode : int
//        {
//            OK = 0,
//            InvalidJson = 100,                  // Response JSON related Group  
//            PleaseRequestKey = 101,
//            InvalidKey = 200,                   // Authentication Issue Group
//            DuplicateEntryDB = 300,             // DataBase Issue Group
//            RequestIsMissingParameter = 400,    // Request Parameter Group 
//            GeneralFailure = 999,               // Kitchen Sink

//        }

//        private Dictionary<string, SessionKey> UserKeysByIp = new Dictionary<string, SessionKey>();

//        // HINT About Decorations ..
//        // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
//        // To create an operation that returns XML,
//        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
//        //     and include the following line in the operation body:
//        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";


//        /// <summary>
//        /// Handles the work requests.  client has to submit key
//        /// Call response is based on enumerator(WebUtilityReponseCode) plus JSONdata.
//        /// </summary>
//        /// <returns>JSON data</returns>
//        [OperationContract][WebGet]
//        public string DoWork()
//        {
//            /*
//             * you call it this way
//             * http://localhost/JBTCDataFeedWebUtility/JBTCDataFeedWebUtilityService.svc/DoWork?user=123&pwd=234
//             */
//            NameValueCollection queryStringCol =
//                WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;

//            string parameters = string.Empty;
//            Dictionary<string, string> pars = new Dictionary<string, string>();
//            if (queryStringCol != null && queryStringCol.Count > 0)
//            {
//                for (int i = 0; i < queryStringCol.Count; i++)
//                {
//                    pars.Add(queryStringCol.AllKeys[i], queryStringCol[i]);

//                }
//            }

//            OperationContext context = OperationContext.Current;
//            MessageProperties prop = context.IncomingMessageProperties;
//            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
//            string ip = endpoint.Address;
//            ResponseData r = new ResponseData();
//            if (UserKeysByIp.ContainsKey(ip))
//            {
//                /*
//                 * structure of request parameters
//                 * RequestType = name of the request - index 0
//                 * RequestData = JSON input data = index 1
//                 */
//                if (pars.ContainsKey("RequestType"))
//                {
//                    if (pars.ContainsKey("RequestData"))
//                    {
//                        switch (pars["RequestType"])
//                        {
//                            case "1":

//                                break;
//                            default:
//                                break;
//                        }
//                        string responsedata = "aha";
//                        r.ResponseKey = (int)WebUtilityReponseCode.OK;
//                        r.JSON = responsedata;
//                    }
//                    else
//                    {
//                        r.ResponseKey = (int)WebUtilityReponseCode.RequestIsMissingParameter;
//                    }
//                }
//                else
//                {
//                    r.ResponseKey = (int)WebUtilityReponseCode.RequestIsMissingParameter;

//                }
//            }
//            else
//            {
//                r.ResponseKey = (int)WebUtilityReponseCode.PleaseRequestKey;
//            }
//            // Add your operation implementation here
//            string res = new JavaScriptSerializer().Serialize(r);
//            return res;
//        }


//        /// <summary>
//        /// Does the heartbeat processing.
//        /// </summary>
//        /// <returns></returns>
//        [OperationContract]
//        [WebGet]
//        public string DoPing()
//        {
//            /*
//             * you call it this way
//             * http://localhost/JBTCDataFeedWebUtility/JBTCDataFeedWebUtilityService.svc/DoPing
//             */
//            OperationContext context = OperationContext.Current;
//            MessageProperties prop = context.IncomingMessageProperties;
//            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
//            string ip = endpoint.Address;
//            return "OK.. you are "+ip;
//        }

//        /// <summary>
//        /// Does the authenticate.
//        /// </summary>
//        /// <param name="username">The username.</param>
//        /// <param name="password">The password.</param>
//        /// <returns></returns>
//        [OperationContract]
//        [WebGet]
//        public string DoAuthenticate()
//        {
//            /*
//             * you call it this way
//             * http://localhost/JBTCDataFeedWebUtility/JBTCDataFeedWebUtilityService.svc/DoAuthenticate?user=123&pwd=234
//             */
//            NameValueCollection queryStringCol = 
//                WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;

//            string parameters = string.Empty;
//            if (queryStringCol != null && queryStringCol.Count > 0)
//            {                
//                for (int i = 0; i < queryStringCol.Count; i++)
//                {
//                    parameters += queryStringCol[i] + "###";
//                }
//            }

//            OperationContext context = OperationContext.Current;
//            MessageProperties prop = context.IncomingMessageProperties;
//            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
//            string ip = endpoint.Address;

//            SessionKey s = new SessionKey();
//            s.TimeStamp = DateTime.Now.ToString();
//            s.Key = Guid.NewGuid().ToString() ;
//            if (UserKeysByIp.ContainsKey(ip))
//            {
//                UserKeysByIp[ip] = s;
//            }
//            else
//            {
//                UserKeysByIp.Add(ip, s);
//            }
//            if (queryStringCol != null && queryStringCol.Count > 0)
//            {
//                return ip + " entered " + parameters; // This will returned as an XML.
//            }
//            else
//            {
//                string res = new JavaScriptSerializer().Serialize(s);
//                return res;

//            }
//        }
//    }
//}
