using System;
using System.Collections;
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
   
    public partial class frmEditSubscribedUsers : Form
    {
        Dictionary<string, string> owners = new Dictionary<string, string>();
        public frmEditSubscribedUsers()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the frmEditSubscribedUsers control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void frmEditSubscribedUsers_Load(object sender, EventArgs e)
        {
            switchScreenToExistingUserMode();
            string firstrid = getTagSubscribersFromDB();   
            string setRid = firstrid;            
            setTextFields(setRid);
            lblRid.Visible = false;
        }
        
        /// <summary>
        /// Handles the Click event of the cmdNew control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cmdNew_Click(object sender, EventArgs e)
        {            
            switchScreenToNewUserMode();
            txtEmail.Text = "";
            txtNewOwner.Text = "";
            txtPhone.Text = "";
            txtSMSEmail.Text = "";
            lblRid.Text = "";
        }

        /// <summary>
        /// Handles the Click event of the cmdClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cmdClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the cmdSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cmdSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lblRid.Text))
            {
                // new row insert
                string owner = txtNewOwner.Text;
                string phone = txtPhone.Text;
                string email = txtEmail.Text;
                string smsemail = txtSMSEmail.Text;
                string execQuery = "INSERT INTO TagSubscribers (OWNER,EMAIL,PHONE,SMSEMAIL) Values('"+
                    owner+"','"+
                    email + "','" +
                    phone + "','" +
                    smsemail + "')";
                string result = RunQuery(execQuery);
                writeEventLog("Insert of new record done for " + owner + " with result: " + (result == "NONE"?"OK":result), "cmdSave_Click", null, Utilities.Logging.LogLevel.Info);
                
            }
            else
            {
                // update existing row
                string rid = lblRid.Text;
                string owner = cmbOwner.SelectedText.ToString();
                string phone = txtPhone.Text;
                string email = txtEmail.Text;
                string smsemail = txtSMSEmail.Text;

                string execQuery = "UPDATE TagSubscribers "+
                    "SET EMAIL='"+email+"',"+
                    "PHONE='"+phone+"',"+
                    "SMSEMAIL='"+smsemail+"'"+
                    "WHERE RID="+rid;
                string result = RunQuery(execQuery);
                writeEventLog("Update of record done for " + owner + " with result: " + (result == "NONE" ? "OK" : result), "cmdSave_Click", null, Utilities.Logging.LogLevel.Info);
            }

            switchScreenToExistingUserMode();
            string firstrid = getTagSubscribersFromDB();
            string setRid = firstrid;
            setTextFields(setRid);
        }
                
        /// <summary>
        /// Handles the SelectedIndexChanged event of the cmbOwner control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cmbOwner_SelectedIndexChanged(object sender, EventArgs e)
        {
            Item itm = (Item)cmbOwner.SelectedItem;
            string setRid = itm.RID.ToString();
            setTextFields(setRid);
        }

        #region aux functions

        /// <summary>
        /// Sets the text fields.
        /// </summary>
        /// <param name="setRid">The set rid.</param>
        private void setTextFields(string setRid)
        {
            string[] data = owners[setRid].Split('|');
            string setOwner = data[0];
            txtEmail.Text = data[1];
            txtPhone.Text = data[2];
            txtSMSEmail.Text = data[3];
            lblRid.Text = setRid;
        }

        /// <summary>
        /// Gets the tag subscribers from database.
        /// </summary>
        /// <returns></returns>
        private string getTagSubscribersFromDB()
        {
            string firstrid = "";
            // get all rows for owner email phone smsemail
            string execQuery = @"SELECT RID,OWNER,EMAIL,PHONE,SMSEMAIL FROM  TagSubscribers";
            DataSet res = GetDataSet(execQuery);
            if (res.Tables != null)
                if (res.Tables.Count > 0)
                {
                    DataTable dt = new DataTable();
                    dt = res.Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        string rid = dr[0].ToString();
                        if (string.IsNullOrEmpty(firstrid))
                        {
                            firstrid = rid;
                        }
                        string owner = dr[1].ToString();
                        string email = dr[2].ToString();
                        string phone = dr[3].ToString();
                        string smsemail = dr[4].ToString();
                        //
                        owners.Add(rid, owner + "|" + email + "|" + phone + "|" + smsemail);
                    }
                }
            cmbOwner.Items.Clear();// empty combo box
       
            foreach (KeyValuePair<string, string> t in owners)
            {
                cmbOwner.Items.Add(new Item(t.Value.Split('|')[0], Convert.ToInt32(t.Key)));
            }
            cmbOwner.SelectedIndex = 0;
            return firstrid;
        }

        /// <summary>
        /// Switches the screen to new user mode.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void switchScreenToNewUserMode()
        {
            lblNewOwner.Visible = true;
            txtNewOwner.Visible = true;
            lblOwner.Visible = false;
            cmbOwner.Visible = false;
        }

        /// <summary>
        /// Switches the screen to existing user mode.
        /// </summary>
        private void switchScreenToExistingUserMode()
        {
            lblNewOwner.Visible = false;
            txtNewOwner.Visible = false;
            lblOwner.Visible = true;
            cmbOwner.Visible = true;

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
        /// Gets the data set.
        /// </summary>        
        /// <param name="SQL">The SQL.</param>
        /// <returns></returns>
        public DataSet GetDataSet(string SQL)
        {
            string connectionString = ConfigurationManager.AppSettings["dbConnectionString"];
            SqlConnection conn = new SqlConnection(connectionString);
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = SQL;
            da.SelectCommand = cmd;
            DataSet ds = new DataSet();

            conn.Open();
            da.Fill(ds);
            conn.Close();

            return ds;
        }
        #endregion
    }

    // Content item for the combo box
    public class Item
    {
        public string Name;
        public int RID;
        public Item(string name, int _rid)
        {
            Name = name; RID = _rid;
        }
        public override string ToString()
        {
            // Generates the text shown in the combo box
            return Name;
        }
    }
}
