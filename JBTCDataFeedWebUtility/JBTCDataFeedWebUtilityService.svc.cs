using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Serialization;

namespace JBTCDataFeedWebUtility
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class JBTCDataFeedWebUtilityService
    {
        // response values for client
        private  enum   WebUtilityReponseCode : int
        {
            OK = 0,
            InvalidJson = 100,                  // Response JSON related Group  
            PleaseRequestKey = 101,
            InvalidKey = 200,                   // Authentication Issue Group
            DuplicateEntryDB = 300,             // DataBase Issue Group
            RequestIsMissingParameter = 400,    // Request Parameter Group 
            GeneralFailure = 999,               // Kitchen Sink
            
        }

        private Dictionary<string, SessionKey> UserKeysByIp = new Dictionary<string, SessionKey>();

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
        [OperationContract][WebGet]
        public string DoWork()
        {
            /*
             * you call it this way
             * http://localhost/JBTCDataFeedWebUtility/JBTCDataFeedWebUtilityService.svc/DoWork?user=123&pwd=234
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
                        switch (pars["RequestType"])
                        {
                            case "1":
                                
                                break;
                            default:
                                break;
                        }
                        string responsedata = "aha";
                        r.ResponseKey = (int)WebUtilityReponseCode.OK;
                        r.JSON = responsedata;
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
            return "OK.. you are "+ip;
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
             * http://localhost/JBTCDataFeedWebUtility/JBTCDataFeedWebUtilityService.svc/DoAuthenticate?user=123&pwd=234
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
            s.Key = Guid.NewGuid().ToString() ;
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
    }
}
