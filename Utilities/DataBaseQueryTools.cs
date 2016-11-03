using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class DataBaseQueryTools
    {
        /// <summary>
        /// Runs the query.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <returns></returns>
        public static List<string> RunQuery(string queryString)
        {
            string connectionString = ConfigurationManager.AppSettings["dbConnectionString"];
            List<string> res = new List<string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        res.Add(reader[0].ToString());
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
        public static DataSet GetDataSet(string SQL)
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
    }
}
