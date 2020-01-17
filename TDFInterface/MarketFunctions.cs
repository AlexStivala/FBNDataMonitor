using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data;
using System.Data.SqlClient;

namespace TDFInterface
{
    public class MarketFunctions
    {
        public static List<MarketModel.MarketHolidays> marketHolidays = new List<MarketModel.MarketHolidays>();
        public static List<MarketModel.MarketPulsePages> MarketPulse = new List<MarketModel.MarketPulsePages>();
        public static List<MarketModel.BusinessPulsePages> BusinessPulse = new List<MarketModel.BusinessPulsePages>();


        // Checks if date is weekend or holiday
        public static bool IsMarketHoliday(DateTime dateCheck)
        {
            
            bool todayIsHoliday = false;
            bool weekday;

            // check if weekend
            if (dateCheck.DayOfWeek != DayOfWeek.Saturday && dateCheck.DayOfWeek != DayOfWeek.Sunday)
                weekday = true;
            else
            {
                weekday = false;
                todayIsHoliday = true;
            }

            // its a weekday - check if holiday
            if (weekday)
            {
                for (int i = 0; i < marketHolidays.Count; i++)
                {
                    if (marketHolidays[i].holiDate == dateCheck.Date)
                        todayIsHoliday = true;
                }
            }

            return todayIsHoliday;

        }

        public static bool IsMarketOpen()
        {

            bool todayIsHoliday = IsMarketHoliday(DateTime.Now);
            bool isMarketOpen;

            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            if (!todayIsHoliday && currentTime > TDFGlobals.marketOpen && currentTime < TDFGlobals.marketClose)
                isMarketOpen = true;
            else
                isMarketOpen = false;

            return isMarketOpen;

        }

        public static DateTime GetLastMarketDate(DateTime checkDate)
        {
            bool done = false;
            bool isHoliday = false;
            isHoliday = MarketFunctions.IsMarketHoliday(checkDate);

            if (isHoliday)
            {
                // walk yesterday date back until last market date found
                while (!done)
                {
                    isHoliday = MarketFunctions.IsMarketHoliday(checkDate);
                    if (isHoliday)
                        checkDate = checkDate.AddDays(-1);
                    else
                        done = true;
                }
            }

            return checkDate;

        }

        public static List<MarketModel.MarketHolidays> GetMarketHolidays()
        {
            string dbConnStr = "Data Source=10.232.77.71;Initial Catalog=X20Financial_TDF;Persist Security Info=True;User ID=X2ouser;Password=C0mpl1cat3d@1";
            //SqlConnection dbConn = new SqlConnection(dbConnStr);
            SqlConnection dbConn = new SqlConnection(dbConnStr);
            dbConn.Open();

            SqlCommand cmd = new SqlCommand($"SELECT * FROM MarketHolidays", dbConn);
            cmd.CommandType = CommandType.Text;

            SqlDataReader sqlData = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(sqlData);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                MarketModel.MarketHolidays mh = new MarketModel.MarketHolidays();
                DataRow row;

                row = dt.Rows[i];
                mh.holiday = row["Holiday"].ToString().Trim();
                mh.holiDate = Convert.ToDateTime(row["holiDate"]);
                
                marketHolidays.Add(mh);

            }

            dbConn.Close();
            sqlData.Close();

            return marketHolidays;
        }

        public static MarketModel.CompanyInfo GetSymbolInfo(string checkSymbol)
        {

            MarketModel.CompanyInfo companyInfo = new MarketModel.CompanyInfo();

            var sym = GetTickerSymbol(checkSymbol);
            
            //string dbConnStr = "Data Source=SQL-dev;Initial Catalog=TDF_Symbols_new;Persist Security Info=True;User ID=sa;Password=Engineer@1";
            string dbConnStr = "Data Source=FBN-SQL-PRI;Initial Catalog=TDF_Symbols_new;Persist Security Info=True;User ID=sa;Password=Engineer@1";
            SqlConnection dbConn = new SqlConnection(dbConnStr);
            dbConn.Open();

            string quot = "\"";
            //SqlCommand cmd = new SqlCommand($"p_GetCompanyInfo {quot}{sym.symbol}{quot}", dbConn);
            SqlCommand cmd = new SqlCommand($"p_GetSymbolInfo {quot}{sym.symbol}{quot}", dbConn);
            cmd.CommandType = CommandType.Text;

            SqlDataReader sqlData = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(sqlData);

            

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row;

                    row = dt.Rows[i];
                    companyInfo.Search_Symbol = row["Search_Symbol"].ToString().Trim();
                    companyInfo.Ticker_Symbol1 = row["Ticker_Symbol1"].ToString().Trim();
                    companyInfo.Instrument_Type = Convert.ToInt32(row["Instrument_Type"]);
                    companyInfo.Instrument_Type_Mnemonic = row["Instrument_Type_Mnemonic"].ToString().Trim();
                    companyInfo.Trading_Exchange = row["Trading_Exchange"].ToString().Trim();
                    companyInfo.Currency = row["Currency"].ToString().Trim();
                    companyInfo.Session = row["Session"].ToString().Trim();
                    companyInfo.Company_Name_Long = row["Company_Name_Long"].ToString().Trim();
                    companyInfo.Company_Name_Short = row["Company_Name_Short"].ToString().Trim();
                    companyInfo.symbol_Valid = true;
                    sym.securityType = Convert.ToInt32(row["sectyType"]);

                }
            }
            else
            {
                companyInfo.Search_Symbol = checkSymbol;
                companyInfo.symbol_Valid = false;

            }
            dbConn.Close();
            sqlData.Close();

            companyInfo.symbol = sym;

            return companyInfo;
        }
        public static string GetCompanyName(string checkSymbol)
        {

            var sym = GetTickerSymbol(checkSymbol);

            //string dbConnStr = "Data Source=SQL-dev;Initial Catalog=TDF_Symbols_new;Persist Security Info=True;User ID=sa;Password=Engineer@1";
            string dbConnStr = "Data Source=FBN-SQL-PRI;Initial Catalog=TDF_Symbols_new;Persist Security Info=True;User ID=sa;Password=Engineer@1";
            SqlConnection dbConn = new SqlConnection(dbConnStr);
            dbConn.Open();

            string quot = "\"";
            //SqlCommand cmd = new SqlCommand($"p_GetCompanyInfo {quot}{sym.symbol}{quot}", dbConn);
            SqlCommand cmd = new SqlCommand($"p_GetSymbolInfoEx {quot}{sym.symbol}{quot}", dbConn);
            cmd.CommandType = CommandType.Text;

            SqlDataReader sqlData = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(sqlData);

            MarketModel.CompanyInfo companyInfo = new MarketModel.CompanyInfo();

            dbConn.Close();
            sqlData.Close();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row;

                    row = dt.Rows[i];
                    companyInfo.Search_Symbol = row["Search_Symbol"].ToString().Trim();
                    companyInfo.Ticker_Symbol1 = row["Ticker_Symbol1"].ToString().Trim();
                    companyInfo.Instrument_Type = Convert.ToInt32(row["Instrument_Type"]);
                    companyInfo.Instrument_Type_Mnemonic = row["Instrument_Type_Mnemonic"].ToString().Trim();
                    companyInfo.Trading_Exchange = row["Trading_Exchange"].ToString().Trim();
                    companyInfo.Currency = row["Currency"].ToString().Trim();
                    companyInfo.Session = row["Session"].ToString().Trim();
                    companyInfo.Company_Name_Long = row["Company_Name_Long"].ToString().Trim();
                    companyInfo.Company_Name_Short = row["Company_Name_Short"].ToString().Trim();
                    companyInfo.symbol_Valid = true;
                    companyInfo.securityType = Convert.ToInt32(row["Security_Type"]);
                    //sym.securityType = Convert.ToInt32(row["sectyType"]);


                }
                companyInfo.symbol = sym;
                return companyInfo.Company_Name_Short;
            }
            else
            {
                return "";
            }
            
            
        }
        public static bool SymbolValid(string checkSymbol)
        {

            var sym = GetTickerSymbol(checkSymbol);

            //string dbConnStr = "Data Source=SQL-dev;Initial Catalog=TDF_Symbols_new;Persist Security Info=True;User ID=sa;Password=Engineer@1";
            string dbConnStr = "Data Source=FBN-SQL-PRI;Initial Catalog=TDF_Symbols_new;Persist Security Info=True;User ID=sa;Password=Engineer@1";
            SqlConnection dbConn = new SqlConnection(dbConnStr);
            dbConn.Open();

            string quot = "\"";
            SqlCommand cmd = new SqlCommand($"p_GetCompanyInfo {quot}{sym.symbol}{quot}", dbConn);
            cmd.CommandType = CommandType.Text;

            SqlDataReader sqlData = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(sqlData);

            MarketModel.CompanyInfo companyInfo = new MarketModel.CompanyInfo();

            dbConn.Close();
            sqlData.Close();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row;

                    row = dt.Rows[i];
                    companyInfo.Search_Symbol = row["Search_Symbol"].ToString().Trim();
                    companyInfo.Ticker_Symbol1 = row["Ticker_Symbol1"].ToString().Trim();
                    companyInfo.Instrument_Type = Convert.ToInt32(row["Instrument_Type"]);
                    companyInfo.Instrument_Type_Mnemonic = row["Instrument_Type_Mnemonic"].ToString().Trim();
                    companyInfo.Trading_Exchange = row["Trading_Exchange"].ToString().Trim();
                    companyInfo.Currency = row["Currency"].ToString().Trim();
                    companyInfo.Session = row["Session"].ToString().Trim();
                    companyInfo.Company_Name_Long = row["Company_Name_Long"].ToString().Trim();
                    companyInfo.Company_Name_Short = row["Company_Name_Short"].ToString().Trim();
                    companyInfo.symbol_Valid = true;

                }
            }
            else
            {
                companyInfo.symbol_Valid = false; ;
            }

            return companyInfo.symbol_Valid;

        }

        public static MarketModel.SymbolDef GetTickerSymbol(string s)
        {

            
            // symbol[-Exch][,Curr][,Sess][,Variant]

            int pos = s.IndexOf("-");
            string[] strSeparator = new string[] { "," };
            string[] symbolStrings;

            MarketModel.SymbolDef sym = new MarketModel.SymbolDef();

            symbolStrings = s.Split(strSeparator, StringSplitOptions.None);

            sym.fullyQualified = false;
            if (symbolStrings.Length > 0)
            {
                sym.symbolFull = s;
                sym.tickerSymbol = symbolStrings[0];

                if (pos < 0)
                {
                    sym.symbol = symbolStrings[0];
                    sym.symbolEx = string.Empty;
                    sym.exchange = string.Empty;
                }
                else
                {
                    sym.symbol = s.Substring(0, pos);
                    sym.symbolEx = symbolStrings[0];
                    sym.exchange = s.Substring(pos);
                }

                if (symbolStrings.Length > 1)
                    sym.currency = symbolStrings[1];

                if (symbolStrings.Length > 2)
                    sym.session = symbolStrings[2];

                if (symbolStrings.Length > 3)
                {
                    sym.variant = symbolStrings[3];
                    sym.fullyQualified = true;
                }

            }
            else
            {
                sym.symbolFull = string.Empty;
                if (pos < 0)
                {
                    sym.symbol = s;
                    sym.symbolEx = string.Empty;
                    sym.exchange = string.Empty;
                }
                else
                {
                    sym.symbol = s.Substring(0, pos);
                    sym.symbolEx = s;
                    sym.exchange = s.Substring(pos);
                }

            }

            return sym;
            
        }
        public static List<MarketModel.BusinessPulsePages> GetBusinessPulsePages(string tableName)
        {
            //string dbConnStr = "Data Source=SQL-dev;Initial Catalog=TDF_Symbols_new;Persist Security Info=True;User ID=sa;Password=Engineer@1";
            string dbConnStr = "Data Source=FBN-SQL-PRI;Initial Catalog=TDF_Symbols_new;Persist Security Info=True;User ID=sa;Password=Engineer@1";
            SqlConnection dbConn = new SqlConnection(dbConnStr);
            dbConn.Open();

            SqlCommand cmd = new SqlCommand($"SELECT * FROM {tableName}", dbConn);
            cmd.CommandType = CommandType.Text;

            SqlDataReader sqlData = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(sqlData);

            dbConn.Close();
            sqlData.Close();

            List<MarketModel.BusinessPulsePages> pages = new List<MarketModel.BusinessPulsePages>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    MarketModel.BusinessPulsePages pulse = new MarketModel.BusinessPulsePages();
                    DataRow row;

                    row = dt.Rows[i];
                    pulse.pageCode = row["pageCode"].ToString().Trim();
                    pulse.pageDescription = row["pageDescription"].ToString().Trim();
                    pages.Add(pulse);
                    
                }
                
            }

            return pages;
        }
        public static List<MarketModel.MarketPulsePages> GetMarketPulsePages(string tableName)
        {
            //string dbConnStr = "Data Source=SQL-dev;Initial Catalog=TDF_Symbols_new;Persist Security Info=True;User ID=sa;Password=Engineer@1";
            string dbConnStr = "Data Source=FBN-SQL-PRI;Initial Catalog=TDF_Symbols_new;Persist Security Info=True;User ID=sa;Password=Engineer@1";
            SqlConnection dbConn = new SqlConnection(dbConnStr);
            dbConn.Open();

            SqlCommand cmd = new SqlCommand($"SELECT * FROM {tableName}", dbConn);
            cmd.CommandType = CommandType.Text;

            SqlDataReader sqlData = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(sqlData);

            dbConn.Close();
            sqlData.Close();

            List<MarketModel.MarketPulsePages> pages = new List<MarketModel.MarketPulsePages>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    MarketModel.MarketPulsePages pulse = new MarketModel.MarketPulsePages();
                    DataRow row;

                    row = dt.Rows[i];
                    pulse.pageCode = row["pageCode"].ToString().Trim();
                    pulse.pageDescription = row["pageDescription"].ToString().Trim();
                    pulse.exchange = row["exchange"].ToString().Trim();
                    pages.Add(pulse);

                }

            }

            return pages;
        }

        public static List<MarketModel.marketSort> GetSP500WinnersLosers(bool winners)
        {
            //string dbConnStr = "Data Source=SQL-dev;Initial Catalog=TDF_Symbols_new;Persist Security Info=True;User ID=sa;Password=Engineer@1";
            string dbConnStr = "Data Source=FBN-SQL-PRI;Initial Catalog=TDF_Symbols_new;Persist Security Info=True;User ID=sa;Password=Engineer@1";

            SqlConnection dbConn = new SqlConnection(dbConnStr);
            dbConn.Open();

            string win = "DESC";
            if (winners == false)
                win = "ASC";

            SqlCommand cmd = new SqlCommand($"SELECT TOP 5 * FROM SP500 TOP5 ORDER BY PercentChange {win}", dbConn);
            cmd.CommandType = CommandType.Text;

            SqlDataReader sqlData = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(sqlData);

            dbConn.Close();
            sqlData.Close();

            List<MarketModel.marketSort> SP500 = new List<MarketModel.marketSort>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    MarketModel.marketSort ms = new MarketModel.marketSort();
                    DataRow row;

                    row = dt.Rows[i];
                    ms.symbol = row["SubscribeSymbol"].ToString().Trim();
                    ms.trdPrc = Convert.ToSingle(row["Last"]);
                    ms.chng = Convert.ToSingle(row["Change"]);
                    ms.pcntChg = Convert.ToSingle(row["PercentChange"]);
                    SP500.Add(ms);

                }

            }

            return SP500;
        }
        
        public static MarketModel.ServerReset GetServerResetSched(int serverId)
        {

            try
            {
                //string dbConnStr = "Data Source=SQL-dev;Initial Catalog=TDF_Symbols_new;Persist Security Info=True;User ID=sa;Password=Engineer@1";
                string dbConnStr = "Data Source=FBN-SQL-PRI;Initial Catalog=TDF_Symbols_new;Persist Security Info=True;User ID=sa;Password=Engineer@1";

                SqlConnection dbConn = new SqlConnection(dbConnStr);
                dbConn.Open();

                string queryStr = $"SELECT * FROM ThompsonServerReset WHERE ServerId = '{serverId}'";
                SqlCommand cmd = new SqlCommand(queryStr, dbConn);
                cmd.CommandType = CommandType.Text;

                SqlDataReader sqlData = cmd.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(sqlData);

                dbConn.Close();
                sqlData.Close();

                MarketModel.ServerReset sr = new MarketModel.ServerReset();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow row;
                        row = dt.Rows[i];

                        sr.ServerId = Convert.ToInt32(row["ServerId"]);
                        sr.IPAddress = row["IPAddress"].ToString().Trim();
                        sr.weekNo = Convert.ToInt32(row["Week"]);
                        sr.UserId = row["UserId"].ToString().Trim();
                        sr.resetTime = Convert.ToDateTime(row["resetTime"]);
                        sr.resetDay = Convert.ToInt32(row["resetDay"]);
                    }
                }
                return sr;

            }
            catch (Exception ex)
            {
                MarketModel.ServerReset sr = new MarketModel.ServerReset();
                string s = $"Error getting server reset info - {ex}";
                return sr;

            }

        }

        
    }
}
