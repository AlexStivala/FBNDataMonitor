using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDFInterface
{
    using System.ComponentModel;
    public enum QueryTypes : int
    {
        [Description("Quotes")]
        Quotes = 0,

        [Description("Portfolio Mgr")]
        Portfolio_Mgr = 1,

        [Description("Fundamentals")]
        Fundamentals = 2,

        [Description("Dynamic Quotes")]
        Dynamic_Quotes = 3,

        [Description("Charts")]
        Charts = 4,

        [Description("Change Since")]
        ChangeSince = 5,

        [Description("Pulse")]
        Pulse = 6,

    }
    public enum XMLTypes
    {
        [Description("Unknown")]
        Unknown = 0,

        [Description("XMLCharts")]
        XMLCharts = 1,

        [Description("marketPages")]
        marketPages = 2,

        [Description("bpPages")]
        bpPages = 3,

        [Description("marketStatistics")]
        marketStatistics = 4,

    }

    public enum RequestTypes
    {
        [Description("Quotes")]
        Quotes = 0,

        [Description("Charts")]
        Charts = 1,

        [Description("Change Since")]
        ChangeSince = 2,

        [Description("Pulse")]
        Pulse = 3,

        [Description("Winners and losers")]
        Winners = 4,

    }



    public class TDFconstants
    {
        // ITF constants
        public const byte SYNC = 0x02;
        public const ushort PROT_ID = 0x0300;  // 0x03 L.E.
        public const byte DATA_OFFSET = 0x0c; //             12
        public const byte MSG_TERMINATOR = 0x00;

        // ITF Message Types
        public const byte LOGON_REQUEST = 0x4c; // 'L'       76  > Send
        public const byte LOGOFF_REQUEST = 0x43; // 'C'      67  > Send
        public const byte DATA_REQUEST = 0x51; // 'Q'        81  > Send
        public const byte LOGON_RESPONSE = 0x49; // 'I'      73
        public const byte LOGOFF_RESPONSE = 0x58; // 'X'     88
        public const byte DATA_RESPONSE = 0X5a; // 'Z'       90
        public const byte DYNAMIC_UPDATE = 0X55; //'U'       85
        public const byte DYNAMIC_CONTROL = 0X59; //'Y'      89
        public const byte KEEP_ALIVE_REQUEST = 0X4b; //'K'   75
        
        // Response Type Data Response
        public const byte SUCCCESSFUL_LOGON_LOGOFF = 0x52; // 'R'  82 MESSAGE TYPE = 0x49  or 0x58
        public const byte ERROR_LOGON_LOGOFF = 0x45; //'E'      69  MESSAGE TYPE = 0x58
        public const ushort OPEN_FID_RESPONSE = 0x17; //        23  MESSAGE TYPE = 0X5a
        public const ushort CATALOGER_RESPONSE = 0x19; //       25  MESSAGE TYPE = 0X5a
        public const ushort SUBSCRIPTION_RESPONSE = 0x1b; //    27  MESSAGE TYPE = 0X5a
        public const ushort UNSUBSCRIPTION_RESPONSE = 0x1c; //  28  MESSAGE TYPE = 0X5a
        public const ushort XML_RESPONSE = 0x28; //             40  MESSAGE TYPE = 0X5a
        public const ushort XML_CHART_RESPONSE = 0x35; //       53  MESSAGE TYPE = 0X5a
        public const ushort SERVER_CONTROL_MESSAGE = 0x53; //'S' 83   MESSAGE TYPE = 0x59
        public const ushort EXCHANGE_CONTROL_MESSAGE = 0x45; //'E' 69   MESSAGE TYPE = 0x59
        public const byte KEEP_ALIVE_RESPONSE = 0x6b; // 'k' 107

    }

    public class TDFGlobals
    {
        public static List<symbolData> symbols = new List<symbolData>();
        public static List<symbolData> LiveUpdates = new List<symbolData>();
        public static field_Info[] field_Info_Table = new field_Info[0x10000];
        public static List<string> starredFields = new List<string>();
        public static List<fin_Data> financialResults = new List<fin_Data>();
        public static Int32[,] CatalogData = new int[150, 60];
        public static Int16 numCat { get; set; }

        public static bool showAllFields { get; set; }
        public static List<string> Dow30symbols = new List<string>();
        public DateTime serverReset;
        public static TimeSpan marketOpen = new TimeSpan(9, 30, 00); //9:30 am
        public static TimeSpan marketClose = new TimeSpan(16, 00, 0); //4:00 pm 
        public static bool marketOpenStatus;
        public static bool eflag = false;
        public static string dbConnSymbols;
        public static string dbConnMarket;



    }

}
