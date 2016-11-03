using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBTCDataFeed
{
    public class FeedOwners
    {
        public Dictionary<string, string> TweetFeed = new Dictionary<string, string>();
        public Dictionary<string, string> SMS = new Dictionary<string, string>();
        public Dictionary<string, string> EMAIL = new Dictionary<string, string>();
        public string EMAILAddress;
        public string SMSaddress;
    }
}
