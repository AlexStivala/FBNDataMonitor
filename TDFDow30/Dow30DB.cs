using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;
using TDFInterface;
using log4net;


namespace Dow30Database
{
    public class Dow30DB
    {

        public class Dow30symbolData
        {
            public int SymbolType { get; set; }
            public string SubscribeSymbol { get; set; }
            public string DisplaySymbol { get; set; }
            public string DisplayName { get; set; }
            public float Last { get; set; }
            public float Change { get; set; }
            public float PercentChange { get; set; }
            public DateTime Updated { get; set; }
            
        }

        public class Russel3000symbolData
        {
            public string Symbol { get; set; }
            public string Name { get; set; }
            public float Last { get; set; }
            public float Change { get; set; }
            public float PercentChange { get; set; }
            public float Open { get; set; }
            public float High { get; set; }
            public float Low { get; set; }
            public Int64 Volume { get; set; }
            public DateTime Updated { get; set; }
            public bool Dow { get; set; }
            public bool Nasdaq100 { get; set; }
            public bool SP { get; set; }
            public int Sector { get; set; }
            public float lastActivity { get; set; }
            public float lastActivityNetChg { get; set; }
            public float lastActivityPcntChg { get; set; }
            public Int64 lastActivityVol { get; set; }
            public float annHi { get; set; }
            public float annLo { get; set; }
            public bool NewHi { get; set; }
            public bool NewLo { get; set; }


        }


        #region Logger instantiation - uses reflection to get module name
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Logging & status setup
        // This method used to implement IAppender interface from log4net; to support custom appends to status strip
        public void DoAppend(log4net.Core.LoggingEvent loggingEvent)
        {
            // Set text on status bar only if logging level is DEBUG or ERROR
            if ((loggingEvent.Level.Name == "ERROR") | (loggingEvent.Level.Name == "DEBUG"))
            {
                //toolStripStatusLabel.BackColor = System.Drawing.Color.Red;
                //toolStripStatusLabel.Text = String.Format("Error Logging Message: {0}: {1}", loggingEvent.Level.Name, loggingEvent.MessageObject.ToString());
            }
            else
            {
                //toolStripStatusLabel.BackColor = System.Drawing.Color.SpringGreen;
                //toolStripStatusLabel.Text = String.Format("Status Logging Message: {0}: {1}", loggingEvent.Level.Name, loggingEvent.MessageObject.ToString());
            }
        }
        #endregion

        public static DataTable GetDBData(string cmdStr, string dbConnection)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Instantiate the connection
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    // Create the command and set its properties
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                        {
                            cmd.CommandText = cmdStr;
                            //cmd.Parameters.Add("@StackID", SqlDbType.Float).Value = stackID;
                            sqlDataAdapter.SelectCommand = cmd;
                            sqlDataAdapter.SelectCommand.Connection = connection;
                            sqlDataAdapter.SelectCommand.CommandType = CommandType.Text;

                            // Fill the datatable from adapter
                            sqlDataAdapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                log.Error("GetDBData Exception occurred: " + ex.Message);
            }

            return dataTable;
        }

        public static int SQLExec(string cmdStr, string dbConnection)
        {
            int numRowsAffected = -2;
            try
            {
                // Instantiate the connection
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    // Create the command and set its properties
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        connection.Open();
                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                        {
                            cmd.CommandText = cmdStr;
                            sqlDataAdapter.SelectCommand = cmd;
                            sqlDataAdapter.SelectCommand.Connection = connection;
                            sqlDataAdapter.SelectCommand.CommandType = CommandType.Text;
                            numRowsAffected = sqlDataAdapter.SelectCommand.ExecuteNonQuery();
                        }
                        connection.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                log.Error("GetDBData Exception occurred: " + ex.Message);
                //log.Debug("GetDBData Exception occurred", ex);
                numRowsAffected = -1;

            }
            return numRowsAffected;
        }


        public static BindingList<Dow30symbolData> GetSymbolDataCollection(string cmdStr, string dbConnection)
        {
            DataTable dataTable;

            // Clear out the current collection
            //candidateData.Clear();
            BindingList<Dow30symbolData> Dow30 = new BindingList<Dow30symbolData>();


            try
            {
                dataTable = GetDBData(cmdStr, dbConnection);

                foreach (DataRow row in dataTable.Rows)
                {
                    var sd = new Dow30symbolData()
                    {
                        SymbolType = Convert.ToInt32(row["SymbolType"] ?? ""),
                        SubscribeSymbol = row["SubscribeSymbol"].ToString() ?? "",
                        DisplaySymbol = row["DisplaySymbol"].ToString() ?? "",
                        DisplayName = row["DisplayName"].ToString() ?? "",
                        Last = Convert.ToSingle(row["Last"] ?? ""),
                        Change = Convert.ToSingle(row["Change"] ?? ""),
                        PercentChange = Convert.ToSingle(row["PercentChange"] ?? ""),
                        //Updated = Convert.ToDateTime(row["Updated"] ?? ""),                       
                    };
                    Dow30.Add(sd);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error");
                // Log error
                log.Error("GetCandidateDataCollection Exception occurred: " + ex.Message);
                //log.Debug("GetCandidateDataCollection Exception occurred", ex);
            }
            // Return 
            return Dow30;
        }

        public static List<Russel3000symbolData> GetRussel3000SymbolDataCollection(string cmdStr, string dbConnection)
        {
            DataTable dataTable;

            // Clear out the current collection
            //candidateData.Clear();
            List<Russel3000symbolData> Russel3000 = new List<Russel3000symbolData>();


            try
            {
                dataTable = GetDBData(cmdStr, dbConnection);
                int n = 0;

                foreach (DataRow row in dataTable.Rows)
                {
                    var sd = new Russel3000symbolData()
                    {
                        Symbol = row["Symbol"].ToString().Trim() ?? "",
                        Name = row["Name"].ToString() ?? "",
                        Last = Convert.ToSingle(row["Last"] ?? 0),
                        Change = Convert.ToSingle(row["Change"] ?? 0),
                        PercentChange = Convert.ToSingle(row["PercentChange"] ?? 0),
                        Open = Convert.ToSingle(row["Open"] ?? 0),
                        High = Convert.ToSingle(row["High"] ?? 0),
                        Low = Convert.ToSingle(row["Low"] ?? 0),
                        Volume = Convert.ToInt64(row["Volume"] ?? 0),
                        Updated = Convert.ToDateTime(row["Updated"] ?? ""),
                        Dow = Convert.ToBoolean(row["Dow"] ?? false),
                        Nasdaq100= Convert.ToBoolean(row["Nasdaq100"] ?? false),
                        SP = Convert.ToBoolean(row["S & P"] ?? false),
                        Sector = Convert.ToInt32(row["Sector"] ?? 0),
                        lastActivity = Convert.ToSingle(row["lastActivity"] ?? 0),
                        lastActivityNetChg = Convert.ToSingle(row["lastActivityNetChg"] ?? 0),
                        lastActivityPcntChg = Convert.ToSingle(row["lastActivityPcntChg"] ?? 0),
                        lastActivityVol = Convert.ToInt64(row["lastActivityVol"] ?? 0),
                        annHi = Convert.ToSingle(row["annHi"] ?? 0),
                        annLo = Convert.ToSingle(row["annLo"] ?? 0),
                        NewHi = Convert.ToBoolean(row["NewHi"] ?? false),
                        NewLo = Convert.ToBoolean(row["NewLo"] ?? false)

                    };
                    Russel3000.Add(sd);
                    n++;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting Russel300 DB data. - {ex}");
                // Log error
                log.Error("GetRussel3000SymbolDataCollection Exception occurred: " + ex.Message);
            }
            // Return 
            return Russel3000;
        }


        public static List<MarketModel.MarketHolidays> GetHolidays(string cmdStr, string dbConnection)
        {
            DataTable dataTable;

            // Clear out the current collection
            //candidateData.Clear();
            List<MarketModel.MarketHolidays> holidays = new List<MarketModel.MarketHolidays>();


            try
            {
                dataTable = GetDBData(cmdStr, dbConnection);

                foreach (DataRow row in dataTable.Rows)
                {
                    var hol = new MarketModel.MarketHolidays()
                    {
                        holiday = row["Holiday"].ToString() ?? "",
                        holiDate = Convert.ToDateTime(row["holiDate"] ?? ""),

                    };
                    holidays.Add(hol);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error");
                // Log error
                log.Error("GetHolidays Exception occurred: " + ex.Message);
            }
            // Return 
            return holidays;
        }

    }
}
