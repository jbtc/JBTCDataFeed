using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;
using static Utilities.Logging;

namespace JBTCDataFeedUserConfiguraton
{
    public partial class frmJBTCDataFeedConfiguration : Form
    {

        Dictionary<string, string> EmailAddressesByOwner = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="frmJBTCDataFeedConfiguration"/> class.
        /// </summary>
        public frmJBTCDataFeedConfiguration()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the FormClosing event of the frmJBTCDataFeedConfiguration control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FormClosingEventArgs"/> instance containing the event data.</param>
        private void frmJBTCDataFeedConfiguration_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        /// <summary>
        /// Handles the Load event of the frmJBTCDataFeedConfiguration control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void frmJBTCDataFeedConfiguration_Load(object sender, EventArgs e)
        {

            // connect to data base
            writeEventLog("Starting", "frmJBTCDataFeedConfiguration_Load", null, Utilities.Logging.LogLevel.Info);

            // get sms emails
            string execQuery = "exec JBTCDataFeedGetEmailAddressses";
            string result = RunQuery(execQuery);
            writeEventLog("exec JBTCDataFeedGetEmailAddressses Done", "OnStart", null, Utilities.Logging.LogLevel.Info);
            // result sample ..            HWR#Hans.Wright@jbtc.com
            List<string> smss = new List<string>(result.Split('|'));
            foreach (string set in smss)
            {
                string[] resset = set.Split('#');
                writeEventLog("resset length:" + resset.Length.ToString(), "OnStart", null, Utilities.Logging.LogLevel.Info);
                EmailAddressesByOwner.Add(resset[0], resset[1]);
            }

           



            // Set up a timer to trigger every minute.
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["watchDogTimerVal"]);// 60000; // 60 seconds
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();



            DoBusiness();

            
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
        /// Called when [timer].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnTimer(object sender, EventArgs e)
        {
            writeEventLog("Timer Expired", "OnTimer", null, Utilities.Logging.LogLevel.Info);
            
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

        /// <summary>
        /// Handles the Click event of the cmdEditSubscribedUsers control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cmdEditSubscribedUsers_Click(object sender, EventArgs e)
        {
            frmEditUsers eu = new frmEditUsers();
            var dialogResult = eu.ShowDialog();
            eu.Dispose();
        }

        /// <summary>
        /// Handles the Click event of the cmdEditDataFeedTags control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cmdEditDataFeedTags_Click(object sender, EventArgs e)
        {
            frmEditDataFeedTags edft = new frmEditDataFeedTags();
            var dialogResult = edft.ShowDialog();
            edft.Dispose();
        }

        /// <summary>
        /// Handles the Click event of the cmdEditDataFeedTagGroups control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cmdEditDataFeedTagGroups_Click(object sender, EventArgs e)
        {
            frmEditDataFeedTagGroups edftg = new frmEditDataFeedTagGroups();
            var dialogresult = edftg.ShowDialog();
            edftg.Dispose();
        }
    }
}
