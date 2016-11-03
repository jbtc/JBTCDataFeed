using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Runtime.InteropServices;
using static Utilities.Logging;
using System.Configuration;
using Utilities;
using System.Threading;
using static Utilities.DataBaseQueryTools;
using JBTCDataFeed;

namespace JBTCDataPort
{
    public partial class JBTCDataFeedService : ServiceBase
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);


        public static int sleepTimeBetweenOPCTagWritesInSecs { get; private set; }
        private static Dictionary<string, OPCTagHandler> opcHandlers = new Dictionary<string, OPCTagHandler>();
        private static List<Thread> allThreads = new List<Thread>();
        private string OPCServerName;
        private List<OPCTagHandler> OPCClients = new List<OPCTagHandler>();
        private Utilities.DataBase dbSet;
        private Dictionary<string, string> tags = new Dictionary<string, string>();
        private Dictionary<string, FeedOwners> owners = new Dictionary<string, FeedOwners>();
        private List<FeedOwnerDTasks> feedOwnerDTasks = new List<FeedOwnerDTasks>();



        /// <summary>
        /// serv status eum
        /// </summary>
        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        /// <summary>
        /// iterop interf.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public long dwServiceType;
            public ServiceState dwCurrentState;
            public long dwControlsAccepted;
            public long dwWin32ExitCode;
            public long dwServiceSpecificExitCode;
            public long dwCheckPoint;
            public long dwWaitHint;
        };



        /// <summary>
        /// Initializes a new instance of the <see cref="JBTCDataFeedService"/> class.
        /// </summary>
        public JBTCDataFeedService()
        {
            InitializeComponent();
            #region using event log - disabled
            // event log update
            //evtJBTCDataFeed = new System.Diagnostics.EventLog();
            //if (!System.Diagnostics.EventLog.SourceExists("evtJBTCDataFeedSource"))
            //{
            //    System.Diagnostics.EventLog.CreateEventSource(
            //        "evtJBTCDataFeedSource", "evtJBTCDataFeedLog");
            //}
            //evtJBTCDataFeed.Source = "evtJBTCDataFeedSource";
            //evtJBTCDataFeed.Log = "evtJBTCDataFeedLog";
            #endregion

        }
        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            writeEventLog("Starting", "OnStart", null, Utilities.Logging.LogLevel.Info);

            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            #region using event log - disabled
            //evtJBTCDataFeed.WriteEntry("JBTCDataFeed starting");
            #endregion

            dbSet = new DataBase();


            sleepTimeBetweenOPCTagWritesInSecs = 
                Convert.ToInt32(ConfigurationManager.AppSettings["sleepTimeBetweenOPCTagWritesInSecs"]);
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = sleepTimeBetweenOPCTagWritesInSecs; // 60000 milli seconds should be default
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
            

            DoBusiness();

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            writeEventLog("Done", "OnStart", null, Utilities.Logging.LogLevel.Info);
        }



        /// <summary>
        /// Called when [timer].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            //evtJBTCDataFeed.WriteEntry("JBTCDataFeed recurring timer expired");
            writeEventLog("Timer Expired", "OnTimer", null, Utilities.Logging.LogLevel.Info);
            
            #region update tags 
            foreach (OPCTagHandler set in OPCClients)
            {

                #region logging to db and adding to tags value dict

                #region table definition tweet feed
                /*
                 * 
                    CREATE TABLE [dbo].[TweetFeed](
                        [RID] [bigint] IDENTITY(1,1) NOT NULL,
                        [OWNER] [varchar](50) NULL,
                        [TAG] [varchar](500) NULL,
                        [DATATYPE] [varchar](50) NULL,
                        [VALUE] [varchar](500) NULL,
                        [TIMESTAMP] [varchar](50) NULL
    )
                 */
                #endregion
                // lock read list
                set._jbtopc.canAddTagstoReadQueue = false;
                //
                if (set._jbtopc.TagUpdates.Count > 0)
                {
                    foreach (string ss in set._jbtopc.TagUpdates)
                    {
                        // tag value quality timestamp
                        string[] valset = ss.Split('#');
                        
                        #region update tags dictionary
                        string tag = valset[0];
                        string tagVal = valset[1];

                        if (tags.ContainsKey(tag))
                        {
                            tags[tag] = tagVal;
                        }
                        #endregion

                        string insertQuery = "INSERT INTO TWEETFEED (OWNER,TAG,DATATYPE,VALUE,TIMESTAMP) Values ('TEST','" +
                            tag + "','STRING','" +
                            tagVal + "','" +
                            //valset[2] + "','" +// QUALITY -- if needed
                            valset[3] +
                             "');";
                        dbSet.RunQuery(insertQuery);
                    }

                }
                set._jbtopc.TagUpdates.Clear();
                // unlock read list
                set._jbtopc.canAddTagstoReadQueue = true;
                #endregion

            }
            foreach (FeedOwnerDTasks fodt in feedOwnerDTasks )
            {
                // check for messages
                string owner = fodt.Owner;
                //string email = owners[owner].EMAILAddress;
                //string sms = owners[owner].SMSaddress;
                foreach (string message in fodt.EMAILQueue)
                {
                    // insert into db
                    List<string> set = 
                        DataBaseQueryTools.RunQuery("exec JBTCDataFeedInsertEmails '"+owner+"','"+message+"'");                    
                }
                foreach (string message in fodt.SMSQueue)
                {
                    // insert into db
                    List<string> set =
                        DataBaseQueryTools.RunQuery("exec JBTCDataFeedInsertSMSs '" + owner + "','" + message + "'");
                }
                foreach (string message in fodt.TWEETQueue)
                {
                    // insert into db
                    List<string> set =
                        DataBaseQueryTools.RunQuery("exec JBTCDataFeedInsertTWEETs '" + owner + "','" + message + "'");
                }
            }
            #endregion
            
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            //evtJBTCDataFeed.WriteEntry("JBTCDataFeed stopping");
            writeEventLog("Stopping", "onStop", null, Utilities.Logging.LogLevel.Info);
            foreach (OPCTagHandler set in OPCClients)
            {
                set.terminateThread = true;
            }
        }

        /// <summary>
        /// Does the business.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void DoBusiness()
        {
            OPCServerName = ConfigurationManager.AppSettings["OPCServerName"];

            /*
             * rough overall process
             * load tag groups 
             * data sample
             *      RID	OWNER	SITE	TAGGROUP	ENABLED
                    1	HWR	RDU	TWEETFEED	TRUE
                    2	HWR	RDU	SMS	TRUE
                    3	HWR	RDU	EMAIL	TRUE
             * run threads by owner
             * child threads by site + tagGroup
             * load all distinct tags in dict with value as value of dict and tag as key
             * on timer distribute
             */

            // build collection for the OPC thread to work off and get data updates for
            getTagsFromDB();

            // build owners collection 
            getTagGroups();

            // start one thread for each owner (with includes all types of feeds per owner)
            startOwnerThreads();

            // use configured tags to collect data from opc
            getTagsFromOPC();
        }

        /// <summary>
        /// Starts the owner threads.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void startOwnerThreads()
        {
            foreach (KeyValuePair<string, FeedOwners> set in owners)
            {
                FeedOwnerDTasks fodt = new FeedOwnerDTasks();
                fodt.mTags = new Dictionary<string, string>();
                fodt.mTags = tags;
                fodt.fo = new FeedOwners();
                fodt.fo = set.Value;
                fodt.Owner = set.Key;

                // create thread
                Thread feedOwnerWorkerThread = new Thread(fodt.startMonitor);
                feedOwnerDTasks.Add(fodt);
                writeEventLog("startOwnerThreads for "+set.Key, "DoBusiness", null, LogLevel.Info);
                // Start the worker thread.
                feedOwnerWorkerThread.Start();                

                // Loop until worker thread activates. 
                while (!feedOwnerWorkerThread.IsAlive) ;

                writeEventLog("startOwnerThreads for " + set.Key + " done", "DoBusiness", null, LogLevel.Info);
            }
        }

        /// <summary>
        /// Gets the tags from database.
        /// </summary>
        private void getTagsFromDB()
        {


            DataSet ds = DataBaseQueryTools.GetDataSet("exec JBTCDataFeedGetTags");// only enabled rows are pulled
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    tags.Add(dr[0].ToString(),"");
                }
            }
        }

        /// <summary>
        /// Gets the tag groups.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void getTagGroups()
        {
            /* 
             * get data from tag groups to establish who has feeds for what 
             * table:   TagGroups
             * data sample:
             *              RID	OWNER	SITE	TAGGROUP	ENABLED
             *              1	HWR	    RDU	    TWEETFEED	TRUE
             *              2	HWR	    RDU	    SMS	        TRUE
             *              3	HWR	    RDU	    EMAIL	    TRUE
             */
            #region build owners dict
            {
                DataSet ds = DataBaseQueryTools.GetDataSet("exec JBTCDataFeedGetTagGroups");// only enabled rows are pulled
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        FeedOwners fo = new FeedOwners();
                        string ownerName = dr[1].ToString();
                        if (!owners.ContainsKey(ownerName))
                        {
                            // add owner to owners dict.  this will contain the current data for all feeds
                            owners.Add(ownerName, fo);
                        }
                    }
                }
            }
            #endregion

            #region add tags to owners data collection
            foreach (KeyValuePair<string, FeedOwners> set in owners)
            {
                string ownerName = set.Key;
                FeedOwners fo = set.Value;
                DataSet feedOwnerDataset = DataBaseQueryTools.GetDataSet("exec JBTCDataFeedGetTagsByOwner '" + ownerName + "'");// only enabled rows are pulled
                /*
                 * sample data 
                 * 
                 * TAG	    TAGGROUP
                 * String1	TWEETFEED
                 * String 1	SMS
                 * 
                 */
                if (feedOwnerDataset.Tables.Count > 0)
                {
                    foreach (DataRow dr in feedOwnerDataset.Tables[0].Rows)
                    {
                        string taggroup = dr[1].ToString();
                        string tag = dr[0].ToString();
                        switch (taggroup)
                        {
                            case "SMS":
                                if (!fo.SMS.ContainsKey(tag))
                                {
                                    fo.SMS.Add(tag, "");
                                }
                                break;
                            case "EMAIL":
                                if (!fo.EMAIL.ContainsKey(tag))
                                {
                                    fo.EMAIL.Add(tag, "");
                                }                                
                                break;
                            case "TWEETFEED":
                                if (!fo.TweetFeed.ContainsKey(tag))
                                {
                                    fo.TweetFeed.Add(tag, "");
                                }
                                break;
                            default:
                                break;
                        }
                    }

                }
            }
            #endregion


        }

        /// <summary>
        /// Gets the tags from opc.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        private void getTagsFromOPC()
        {
            //string ErrorString = "";
            string sysname = "mainSys";
            List<string> tagNames = new List<string>();
            foreach (KeyValuePair<string, string> set in tags)
            {
                tagNames.Add(set.Key);
            }
            OPCTagHandler oth = new OPCTagHandler(sysname, tagNames, OPCServerName, sleepTimeBetweenOPCTagWritesInSecs);
            oth.loglevels = OPCServerName = ConfigurationManager.AppSettings["logLevels"];

            opcHandlers.Add("main", oth);
            // create thread                               
            Thread opcWorkerThread = new Thread(oth.start);

            writeEventLog("OPCConn start...done", "DoBusiness", null, LogLevel.Info);
            // Start the worker thread.
            opcWorkerThread.Start();

            writeEventLog("Starting opc worker thread for " + sysname, "DoBusiness", null, LogLevel.Info);

            // Loop until worker thread activates. 
            while (!opcWorkerThread.IsAlive) ;

            writeEventLog("Starting opc worker thread...done", "DoBusiness", null, LogLevel.Info);

            // add opc thread to thread collection in order to terminate at the end
            allThreads.Add(opcWorkerThread);
            OPCClients.Add(oth);
            writeEventLog(sysname + " done", "DoBusiness", null, LogLevel.Info);

        }


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



    }
}
