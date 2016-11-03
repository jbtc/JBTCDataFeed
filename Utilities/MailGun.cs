using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Utilities
{
    public class MailGun
    {

        /// <summary>
        /// Send email via mailgun API.
        /// sample use : MailGun.SendWithNoAttachment("testemail from mailgun", "hans.wright@jbtc.com", "<html>Hans just implemented MailGun. This is a test.  Thanks   Hans</html>");
        /// </summary>
        /// <param name="Subject">The subject.</param>
        /// <param name="To">To.</param>
        /// <param name="Body">The body.</param>
        public static void SendWithNoAttachment(string Subject, string To, string Body)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3/");
            client.Authenticator = new HttpBasicAuthenticator("api", "key-1e464b66a35b677ba5698a0b48d73226");//Properties.Settings.Default.APIKEY.ToString());
            RestRequest request = new RestRequest();
            request.Resource = "iops-notifications.com/messages";
            request.AddParameter("from", "YOUR APP <hans.wright@jbtc.com>");
            request.AddParameter("to", To);
            request.AddParameter("subject", Subject);
            request.AddParameter("text", Body);
            request.Method = Method.POST;
            var res = client.Execute(request);
            int g = 0;
        }

    }
}
