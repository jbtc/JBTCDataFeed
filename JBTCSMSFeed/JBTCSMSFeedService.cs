﻿
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
using System.Data.SqlClient;

namespace JBTCSMSFeed
{
    
    public partial class JBTCSMSFeedService : ServiceBase
    {
        
        private Dictionary<string, string> SMSEmailsByOwner = new Dictionary<string, string>();
        private bool useMailGun;


        #region service status and event log
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

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
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="JBTCSMSFeedService"/> class.
        /// </summary>
        public JBTCSMSFeedService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            writeEventLog("Starting", "OnStart", null, Utilities.Logging.LogLevel.Info);
            useMailGun = (ConfigurationManager.AppSettings["useMailGun"] == "TRUE" ? true : false);

            // get sms emails
            string execQuery = "exec JBTCDataFeedGetSMSEmails";
            string result = RunQuery(execQuery);
            writeEventLog("exec JBTCDataFeedGetSMSEmails Done", "OnStart", null, Utilities.Logging.LogLevel.Info);
            // result sample ..            HWR#8322293293@messaging.sprintpcs.com
            List<string> smss = new List<string>(result.Split('|'));
            foreach (string set in smss)
            {
                string[] resset = set.Split('#');
                writeEventLog("resset length:"+resset.Length.ToString(), "OnStart", null, Utilities.Logging.LogLevel.Info);
                SMSEmailsByOwner.Add(resset[0], resset[1]);
            }

            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            

            // Set up a timer to trigger every minute.
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000; // 60 seconds
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();

            

            DoBusiness();

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            writeEventLog("Done", "OnStart", null, Utilities.Logging.LogLevel.Info);
        }

        
        /// <summary>
        /// Does the business.
        /// </summary>
        private void DoBusiness()
        {
            //do biz
            writeEventLog("DoBusiness", "DoBusiness", null, Utilities.Logging.LogLevel.Info);

        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            writeEventLog("Stopping", "onStop", null, Utilities.Logging.LogLevel.Info);
        }

        /// <summary>
        /// Called when [timer].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnTimer(object sender, EventArgs e)
        {
            writeEventLog("Timer Expired", "OnTimer", null, Utilities.Logging.LogLevel.Info);

            // check database
            string execQuery = "exec JBTCDataFeedCheckForSMSs";
            string result = RunQuery(execQuery);
            // result sample ..            2#HWR#THIS IS ANOTHER SMS|3#HWR#SMS FOR TWO ROWS TEST
            List <string> smss = new List<string> (result.Split('|'));
            // use result set to write emails
            foreach (string set in smss)
            {

                string[] resultSMSSet = set.Split('#');
                string owner = resultSMSSet[1].Trim().ToUpper();
                string RID = resultSMSSet[0].Trim();
                if (SMSEmailsByOwner.ContainsKey(owner))
                {
                    
                    string ownerSMSEmailAddress = SMSEmailsByOwner[owner];
                    string sms = resultSMSSet[2].TrimEnd();

                    // sms without attachments
                    string fromAddress = "wright.hans@ymail.com";
                    string toAddress = ownerSMSEmailAddress;
                    string emailSubject = "JBTCDataFeedServiceSMS";
                    string emailBody = sms;
                    string smtServerString = "smtp.mail.yahoo.com";
                    int smtpServerPort = 465;
                    string emailServerUserName = @"wright.hans@ymail.com";
                    string emailServerUserPassword = @"superSecretPassword123ThatNeverWillBeCrackedWithinThisOrTheNextCenturyUnlessYouAreARussianOrChineseHackerButInAnyOtherCaseItJustWillTakeSoooMuchLongerNothingIsSafeJustWHenYouGetOutOfBoundsWithAServerWouldTakeThisPassword";
                    if (useMailGun == true)
                    {
                        MailGun.SendWithNoAttachment(emailSubject, toAddress, emailBody);
                    }
                    else
                    {
                        //if (false == Utilities.Email.SendEmailWithAttachment(
                        //        null,
                        //        fromAddress,
                        //        toAddress,
                        //        emailSubject,
                        //        emailBody,
                        //        smtServerString,
                        //        smtpServerPort,
                        //        emailServerUserName,
                        //        emailServerUserPassword))
                        //{
                        //    writeEventLog("Email failed", "OnTimer", null, Utilities.Logging.LogLevel.Info);
                        //}
                        //else
                    }
                    {
                        // check database
                        string execQuerySMSDone = "exec JBTCDataFeedSMSDone '" + RID + "'";
                        string smsresult = RunQuery(execQuerySMSDone);
                    }
                }
            }
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

    }
}
