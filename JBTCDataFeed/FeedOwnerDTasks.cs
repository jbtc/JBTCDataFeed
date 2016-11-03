using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace JBTCDataFeed
{
    public class FeedOwnerDTasks
    {
        public FeedOwners fo = new FeedOwners();
        public Dictionary<string, string> mTags = new Dictionary<string, string>();
        public List<string> SMSQueue = new List<string>();
        public List<string> EMAILQueue = new List<string>();
        public List<string> TWEETQueue = new List<string>();
        internal string Owner;

        /// <summary>
        /// Starts the monitor.
        /// </summary>
        public void startMonitor()
        {
            #region setup recurring timer for data checks
            int ownerwatchDogTimerVal =
                Convert.ToInt32(ConfigurationManager.AppSettings["ownerwatchDogTimerVal"]);
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = ownerwatchDogTimerVal; // 60000 milli seconds should be default
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
            #endregion
        }

        /// <summary>
        /// Called when [timer].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            #region check owner's tags and update
            foreach (KeyValuePair<string, string> tSet in mTags)
            {
                if (fo.SMS.ContainsKey(tSet.Key))
                {
                    // check if old value != new value
                    string oldval = fo.SMS[tSet.Key];
                    string newVal = tSet.Value;

                    if (oldval != newVal)
                    {
                        string message = buildDataMessage("SMS",tSet.Key, oldval, newVal);                             
                        SMSQueue.Add(message);
                    }
                }

                if (fo.EMAIL.ContainsKey(tSet.Key))
                {
                    // check if old value != new value
                    string oldval = fo.EMAIL[tSet.Key];
                    string newVal = tSet.Value;

                    if (oldval != newVal)
                    {
                        string message = buildDataMessage("EMAIL",tSet.Key, oldval, newVal);
                        EMAILQueue.Add(message);
                    }
                }
                if (fo.TweetFeed.ContainsKey(tSet.Key))
                {
                    // check if old value != new value
                    string oldval = fo.TweetFeed[tSet.Key];
                    string newVal = tSet.Value;

                    if (oldval != newVal)
                    {
                        string message = buildDataMessage("TWEET",tSet.Key, oldval, newVal);
                        TWEETQueue.Add(message);
                    }
                }

            }


            #endregion


        }

        /// <summary>
        /// Builds the data message.
        /// </summary>
        /// <param name="feedType">Type of the feed.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="oldval">The oldval.</param>
        /// <param name="newVal">The new value.</param>
        /// <returns></returns>
        private string buildDataMessage(string feedType, string tag, string oldval, string newVal)
        {
            switch (feedType)
            {
                case "SMS":
                case "EMAIL":
                case "TWEET":
                default:
                    return DateTime.Now.ToString() + " | " + tag + " changed from " + oldval + " to " + newVal;
            }
        }
        
    }
}
