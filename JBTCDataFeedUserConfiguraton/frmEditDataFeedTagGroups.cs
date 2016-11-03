using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;
using static Utilities.Logging;

namespace JBTCDataFeedUserConfiguraton
{
    public partial class frmEditDataFeedTagGroups : Form
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["dbConnectionString"]);
        SqlCommand cmd;
        SqlDataAdapter adapt;
        //ID variable used in Updating and Deleting Record  
        int ID = 0;


        public frmEditDataFeedTagGroups()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the frmEditDataFeedTagGroups control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void frmEditDataFeedTagGroups_Load(object sender, EventArgs e)
        {
            DisplayData();
        }

        /// <summary>
        /// Handles the Click event of the cmdInsert control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cmdInsert_Click(object sender, EventArgs e)
        {
            if (txtOwner.Text != "" && txtSite.Text != "" && txtTagGroup.Text != "")
            {
                insertData(txtOwner.Text, txtSite.Text, txtTagGroup.Text, txtEnabled.Text);
                DisplayData();
                ClearData();
            }
            else
            {
                MessageBox.Show("Please Provide All Mandatory Details!");
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdUpdate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            if (txtOwner.Text != "" && txtSite.Text != ""  && txtTagGroup.Text != "")
            {
                updateData(ID, txtSite.Text, txtTagGroup.Text, txtEnabled.Text);
                DisplayData();
                ClearData();
            }
            else
            {
                MessageBox.Show("Please Select Record to Update");
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdDelete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if (ID != 0)
            {
                deleteData(ID);
                DisplayData();
                ClearData();
            }
            else
            {
                MessageBox.Show("Please Select Record to Delete");
            }
        }

        /// <summary>
        /// Handles the RowHeaderMouseClick event of the dataGridView1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridViewCellMouseEventArgs"/> instance containing the event data.</param>
        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!string.IsNullOrEmpty(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString()))
            {
                ID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                txtOwner.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                txtSite.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtTagGroup.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                txtEnabled.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
            }
            else
            {
                MessageBox.Show("No Data In Row.");
            }
        }


        /// <summary>
        /// Handles the Click event of the cmdLoadCsv control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cmdLoadCsv_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Load Tag Group List";
            openFileDialog1.Filter = "CSV|*.CSV";
            bool faultFlag = false;

            if (openFileDialog1.ShowDialog() != DialogResult.Cancel)
            {

                string fileName = openFileDialog1.FileName;
                List<String> data = File.ReadAllLines(fileName).Distinct().ToList();
                foreach (string set in data)
                {
                    //OWNER,SITE,TAGGROUP,Tag
                    string[] fields = set.Split(',');
                    if (fields.Length == 4)
                    {
                        string owner = fields[0];
                        string site = fields[1];
                        string taggroup = fields[2];
                        string enabled = (fields[3].ToUpper().TrimEnd() == "TRUE"?"TRUE":"FALSE");                
                        insertData(owner, site, taggroup, enabled);
                    }
                    else
                    {
                        faultFlag = true;
                        writeEventLog("Invalid CSV data row. Data:" + set, "cmdLoadCsv_Click", "frmEditDataFeedTagGroups", Utilities.Logging.LogLevel.Warning);
                    }

                }

                if (faultFlag == true)
                {
                    MessageBox.Show("CSV file had errors. Mandatory fields are OWNER,SITE,TAGGROUP,ENABLED." +
                        "  Please check log file");
                }
                else
                {
                    MessageBox.Show(fileName + " loaded");
                    DisplayData();
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdExportDataToCSV control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cmdExportDataToCSV_Click(object sender, EventArgs e)
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string filename = System.IO.Path.GetTempPath() + @"TagsFeedGroupCSV." + unixTimestamp.ToString() + ".csv";

            saveFileDialog1.Title = "Save TagFeed CSV";
            saveFileDialog1.FileName = filename;
            if (saveFileDialog1.ShowDialog() != DialogResult.Cancel)
            {
                filename = saveFileDialog1.FileName;
                con.Open();
                DataTable dt = new DataTable();
                adapt = new SqlDataAdapter("SELECT OWNER,SITE,TAGGROUP,ENABLED FROM TagGroups", con);
                adapt.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    string data = dr[0].ToString() + "," + dr[1].ToString() + "," + dr[2].ToString() + "," + dr[3].ToString() + Environment.NewLine;
                    File.AppendAllText(filename, data);
                }
                con.Close();
            }
        }



        #region aux functions


        /// <summary>
        /// Inserts the data.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="site">The site.</param>
        /// <param name="taggroup">The taggroup.</param>
        /// <param name="enabled">The enabled.</param>
        private void insertData(string owner, string site, string taggroup, string enabled)
        {
            try
            {
                cmd = new SqlCommand("insert into TagGroups(OWNER,SITE,TAGGROUP,ENABLED)" +
                                    " values(@owner,@site,@taggroup,@enabled)", con);
                con.Open();
                cmd.Parameters.AddWithValue("@owner", owner.ToUpper().TrimEnd());
                cmd.Parameters.AddWithValue("@site", site.ToUpper().TrimEnd());
                cmd.Parameters.AddWithValue("@taggroup", taggroup.ToUpper().TrimEnd());
                cmd.Parameters.AddWithValue("@enabled", (enabled.ToUpper().TrimEnd() == "TRUE"?"TRUE":"FALSE"));
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception esd)
            {
                writeEventLog("Insert failed. Exception:" + esd.Message, "insertData", "frmEditDataFeedTags", Utilities.Logging.LogLevel.Exception);
            }
        }

        /// <summary>
        /// Updates the data.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="site">The site.</param>
        /// <param name="taggroup">The taggroup.</param>
        /// <param name="enabled">The enabled.</param>
        private void updateData(int id, string site, string taggroup, string enabled)
        {
            try
            {
                cmd = new SqlCommand("update TagGroups set SITE=@site,TAGGROUP=@taggroup," +
                        " ENABLED=@enabled where RID = @RID", con);
                con.Open();
                cmd.Parameters.AddWithValue("@RID", id);
                cmd.Parameters.AddWithValue("@site", site.ToUpper().TrimEnd());
                cmd.Parameters.AddWithValue("@taggroup", taggroup.ToUpper().TrimEnd());
                cmd.Parameters.AddWithValue("@enabled", (enabled.ToUpper().TrimEnd() == "TRUE" ? "TRUE" : "FALSE"));
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception esd)
            {
                writeEventLog("Update failed. Exception:" + esd.Message, "updateData", "frmEditDataFeedTags", Utilities.Logging.LogLevel.Exception);
            }
        }

        /// <summary>
        /// Deletes the data.
        /// </summary>
        /// <param name="id">The identifier.</param>
        private void deleteData(int id)
        {
            try
            {
                cmd = new SqlCommand("delete from TagGroups where RID=@RID", con);
                con.Open();
                cmd.Parameters.AddWithValue("@RID", id);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception esd)
            {
                writeEventLog("Delete failed. Exception:" + esd.Message, "deleteData", "frmEditDataFeedTags", Utilities.Logging.LogLevel.Exception);
            }
        }

        /// <summary>
        /// Truncates the data.
        /// </summary>
        private void truncateData()
        {
            try
            {
                cmd = new SqlCommand("TRUNCATE TABLE TagGroups", con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception esd)
            {
                writeEventLog("Delete failed. Exception:" + esd.Message, "deleteData", "frmEditDataFeedTags", Utilities.Logging.LogLevel.Exception);
            }
        }


        //Display Data in DataGridView  
        /// <summary>
        /// Displays the data.
        /// </summary>
        private void DisplayData()
        {
            con.Open();
            DataTable dt = new DataTable();
            adapt = new SqlDataAdapter("SELECT RID,OWNER,SITE,TAGGROUP,ENABLED FROM TagGroups", con);
            adapt.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();
        }

        /// <summary>
        /// Clears the data.
        /// </summary>
        private void ClearData()
        {
            txtOwner.Text = "";
            txtSite.Text = "";            
            txtTagGroup.Text = "";
            txtEnabled.Text = "";
            ID = 0;
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


        #endregion



    }
}
