using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceNowFilesToFTP
{
    class clSQL
    {      

        public static DataTable getDataTable(string table)
        {
            DataTable dt = new DataTable();

            try
            {
                //table = getTablePrefixName(table, prefix);

                string sqlConn = ConfigurationSettings.AppSettings["DefaultConn"];

                using (SqlConnection connection = new SqlConnection(sqlConn))
                {
                    SqlCommand command = new SqlCommand("Select * from " + table, connection);
                    command.Connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dt);
                    command.Connection.Close();
                }
            }
            catch (Exception ee)
            {
                dt = new DataTable();
            }

            return dt;


        }     
    }
}
