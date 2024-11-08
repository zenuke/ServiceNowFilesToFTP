using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceNowFilesToFTP
{
    class Program
    {
        static void Main(string[] args)
        {

            List<string> createCSVs = createCSVFiles();


            //Get Files from the Server Path where the created date is today or modified date is today

            //for each file, upload it.

        }

        public static List<string> createCSVFiles()
        {

            //For each table data from SQL, create a CSV file and put in the path.            
            List<string> Tables = new List<string>() { "GreenHouse_Users","GreenHouse_Custom_Field","GreenHouse_Custom_Field_Options","GreenHouse_Departments","GreenHouse_Offices","vw_GreenHouse_JobOffer_Data","vw_greenhouse_joboffer_Customfield_data", "vw_GreenHouse_Candidate_Custom_Field_Data" };

            foreach (string t in Tables)
            {

                DataTable dt = clSQL.getDataTable(t);

                if(dt.Rows.Count!=0)
                {
                    string fileContents = ToCsv(dt);

                    System.IO.File.WriteAllText(@"c:\temp\" + t + ".csv", fileContents);


                }

                
            }

            

            return new List<string>();
        }

        public static string ToCsv(DataTable dataTable)
        {
            StringBuilder sbData = new StringBuilder();

            // Only return Null if there is no structure.
            if (dataTable.Columns.Count == 0)
                return null;

            foreach (var col in dataTable.Columns)
            {
                if (col == null)
                    sbData.Append(",");
                else
                    sbData.Append("\"" + col.ToString().Replace("\"", "\"\"") + "\",");
            }

            sbData.Replace(",", System.Environment.NewLine, sbData.Length - 1, 1);

            foreach (DataRow dr in dataTable.Rows)
            {
                foreach (var column in dr.ItemArray)
                {
                    if (column == null)
                        sbData.Append(",");
                    else
                        sbData.Append("\"" + column.ToString().Replace("\"", "\"\"") + "\",");
                }
                sbData.Replace(",", System.Environment.NewLine, sbData.Length - 1, 1);
            }

            return sbData.ToString();
        }
    }
}
