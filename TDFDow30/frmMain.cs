using System;
using System.Collections.Generic;
using log4net.Appender;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using AsyncClientSocket;
using TDFInterface;
using System.Diagnostics;
using System.IO;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.InteropServices;

namespace TDFDow30
{
    public delegate void ConnectLite(bool on);

    public partial class frmMain : Form, IAppender
    {
        #region Globals
        DateTime referenceTime = DateTime.MaxValue;
        DateTime dataReceivedTime = DateTime.MaxValue;
        DateTime lastKA;
        DateTime lastDCEmail;
        public XmlDocument xmlResponse = new XmlDocument();
        public static List<Dow30Database.Dow30DB.Russel3000symbolData> Russel3000Data = new List<Dow30Database.Dow30DB.Russel3000symbolData>();
        public static List<Dow30Database.Dow30DB.Russel3000symbolData> R3000UpdateData = new List<Dow30Database.Dow30DB.Russel3000symbolData>();
        public static List<symbolData> addedSymbols = new List<symbolData>();

        Stopwatch stopWatch = new Stopwatch();
        TimeSpan ts;
        public TimeSpan tsMax = TimeSpan.Zero;
        public bool updateFlag = false;
        public bool symbolError = false;
        public int nSkip = 0;
        public static DateTime lastDataReceived = DateTime.Now;

        public ConnectLite connectLite;

        itf_Header stdHeadr = new itf_Header()
        {
            sync = TDFconstants.SYNC,
            msgType = TDFconstants.LOGON_REQUEST,
            protId = TDFconstants.PROT_ID,
            seqId = 0,
            sessionId = 0xffff,
            msgSize = 0,
            dataOffset = TDFconstants.DATA_OFFSET
        };

        // Default Login Info
        public string IPAddress = "";
        public string Port = "";
        public string UserName = "";
        public string PW = "";

        public string logResp = "";
        public ushort session_ID = 0xffff;
        public string cmdResp = "";


        public Int32[,] CatalogData = new int[150, 60];
        public string msgStr = "";
        public string XMLStr = "";

        public string quot = "\"";
        public int rCnt = 0;
        public Int64 bytesReceived = 0;
        public bool moreData = false;
        public bool traversal = true;
        public string unsubscribeSymbol = "";
        public int numLog = 0;

        public itf_Parser_Return_Message tmpMessage = new itf_Parser_Return_Message();
        public static List<byte> TRdata = new List<byte>();
        public List<string> catStr = new List<string>();
        public List<itf_Parser_Return_Message> recMessages = new List<itf_Parser_Return_Message>();

        public bool showCatalog = false;
        public bool showFIT = false;
        public bool pageDataFlag = false;
        public string FITstr = "";

        public Chart_Data ch = new Chart_Data();
        public Chart_Info chartInfo = new Chart_Info();
        public List<Chart_Data> charts = new List<Chart_Data>();
        public int nchart = 0;
        public string statusStr = "";
        public pageData marketPage = new pageData();
        public Stopwatch sw;
        public string dbConn = "";
        public string dbTableName = "";
        public string dbChartTableName = "";
        public List<string> symbolListStr = new List<string>();
        public DateTime connectTime;
        public DateTime disconnectTime;
        public DateTime refTime;
        public bool resetting = false;
        public bool resetComplete = false;
        public bool loggedIn = false;
        public bool dynamic = false;
        public List<string> messages = new List<string>();
        public string zipperFilePath;
        public bool debugMode = false;
        public bool timerFlag = false;
        public bool resetFlag = false;
        public bool zipperFlag = false;
        public string spName = "";
        public DateTime timerEmailSent = DateTime.Now.AddDays(-1);
        public DateTime zipperEmailSent = DateTime.Now.AddDays(-1);
        public bool marketIsOpen = false;
        public float dowValue;
        public float nasdaqValue;
        public float spxValue;
        public Int16 chartCnt = 0;
        public Int16 chartInterval = 1;
        public bool updateZipperFile = false;
        public bool updateChartData = false;
        public string spUpdateChart = "";
        public static bool dataReset = false;
        public static byte mt = 0;
        public static ushort msgSize = 0;
        public static int dataLeft = 0;
        public static bool errFlag = false;

        public TimeSpan marketOpen = new TimeSpan(9, 29, 58); //9:30 am
        public TimeSpan marketClose = new TimeSpan(16, 10, 0); //4:06 pm  somtimes data is updated a bit after market close
        public TimeSpan oneMinute = new TimeSpan(0, 1, 1); // 1 min and 1 sec
        public TimeSpan tenMinutes = new TimeSpan(0, 10, 1); // 10 min and 1 sec
        public TimeSpan tenSeconds = new TimeSpan(0, 0, 10); // 10 sec

        public static int nSymGood = 0;
        public static int nSymBad = 0;
        public UInt64 tCnt = 0;
        public int nSymPerSec = 0;
        public int oldCnt = 0;
        public int nTick = 0;
        public int nTickTot = 0;
        public int nSec = 0;
        public int nTickPerMin = 0;
        public int nSymPerMin = 0;
        public int nDup = 0;
        public DateTime day = DateTime.Today;
        public bool todayIsHoliday = false;
        public bool weekday = true;
        public DateTime nextServerReset;
        public DateTime nextDailyReset;
        int ServerID = 0;
        public static DateTime lastEmail = DateTime.Now.AddMinutes(-15);

        public static fin_Data fd = new fin_Data();




        public class X20ChartData
        {
            public float dow { get; set; }
            public float nasdaq { get; set; }
            public float spx { get; set; }
        }
        public X20ChartData X20_chartData = new X20ChartData();


        public class XMLUpdateEventArgs : EventArgs
        {
            public string XML { get; set; }
        }

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);
        private const int WM_SETREDRAW = 11;

        private void SuspendView(DataGridView view)
        {
            SendMessage(view.Handle, WM_SETREDRAW, false, 0);
        }

        private void ResumeView(DataGridView view)
        {
            SendMessage(view.Handle, WM_SETREDRAW, true, 0);
            view.Refresh();
        }

        

    public List<MarketModel.MarketHolidays> marketHolidays = new List<MarketModel.MarketHolidays>();

        
        #endregion


        #region Collection, bindilist & variable definitions

        /// <summary>
        /// Define classes for collections and logic
        /// </summary>

        // Declare TCP client sockets for Thomson Reuters communications
        public AsyncClientSocket.ClientSocket TRClientSocket;
        bool TRConnected = false;


        #endregion

        #region Logger instantiation - uses reflection to get module name
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Logging & status setup
        // This method used to implement IAppender interface from log4net; to support custom appends to status strip
        public void DoAppend(log4net.Core.LoggingEvent loggingEvent)
        {
            // Set text on status bar only if logging level is DEBUG or ERROR
            if (loggingEvent.Level.Name == "ERROR")
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

        // Handler to clear status bar message and reset color
        private void resetStatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //toolStripStatusLabel.BackColor = System.Drawing.Color.SpringGreen;
            //toolStripStatusLabel.Text = "Status Logging Message: Statusbar reset @" + DateTime.Now.ToString();
        }
        #endregion

        public frmMain()
        {
            InitializeComponent();
        }

        public event EventHandler<SymbolUpdateEventArgs> SymbolDataUpdated;
        //public event EventHandler<ChartLiveUpdateEventArgs> ChartDataUpdated;
        //public event EventHandler<XMLUpdateEventArgs> XMLDataUpdated;
        //public event EventHandler<ChartClosedEventArgs> ChartClosed;

        protected virtual void OnSymbolDataUpdated(SymbolUpdateEventArgs e)
        {

            EventHandler<SymbolUpdateEventArgs> evntH = SymbolDataUpdated;
            if (evntH != null)
                evntH(this, e);


            //SymbolDataUpdated?.Invoke(this, e);
        }
        /*
        protected virtual void OnChartDataUpdated(ChartLiveUpdateEventArgs e)
        {

            EventHandler<ChartLiveUpdateEventArgs> evntH = ChartDataUpdated;
            if (evntH != null)
                evntH(this, e);

            //ChartDataUpdated?.Invoke(this, e);
        }

        
        protected virtual void OnXMLDataUpdated(XMLUpdateEventArgs e)
        {

            EventHandler<XMLUpdateEventArgs> evntH = XMLDataUpdated;
            if (evntH != null)
                evntH(this, e);

        }
        */


        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                // Read in config settings
                // Display host name and IP address
                string hostIpAddress = HostIPNameFunctions.GetLocalIPAddress();
                string hostName = HostIPNameFunctions.GetHostName(hostIpAddress);
                lblIpAddress.Text = hostIpAddress;
                lblHostName.Text = hostName;
                string server;
                string User;
                string PW;

                IPAddress = Properties.Settings.Default.TDF_IPAddress;
                Port = Properties.Settings.Default.TDF_Port;
                UserName = Properties.Settings.Default.TDF_UserName;
                PW = Properties.Settings.Default.TDF_PW;

                dbConn = Properties.Settings.Default.dbConn;
                dbTableName = Properties.Settings.Default.dbTableName;
                dbChartTableName = Properties.Settings.Default.chartTableName;
                spName = Properties.Settings.Default.spUpdate;
                spUpdateChart = Properties.Settings.Default.spUpdateChart;
                dynamic = Properties.Settings.Default.Dynamic;
                zipperFilePath = Properties.Settings.Default.ZipperFilePath;
                debugMode = Properties.Settings.Default.DebugMode;
                updateZipperFile = Properties.Settings.Default.updateZipperFile;
                updateChartData = Properties.Settings.Default.updateChartData;
                ServerID = Properties.Settings.Default.TDFServer_ID;

                label3.Text = dbTableName;


                string primaryServer = Properties.Settings.Default.PrimaryServer;
                string backupServer = Properties.Settings.Default.BackupServer;
                string UserPri = Properties.Settings.Default.SQLUserNamePri;
                string PwPri = Properties.Settings.Default.SQLPWPri;
                string UserBk = Properties.Settings.Default.SQLUserNameBk;
                string PwBk = Properties.Settings.Default.SQLPWBk;
                bool useBackupServer = Properties.Settings.Default.UseBackupServer;

                if (useBackupServer)
                {
                    server = backupServer;
                    User = UserBk;
                    PW = PwBk;
                    log.Info($"Using backup SQL server {server}");
                }
                else
                {
                    server = primaryServer;
                    User = UserPri;
                    PW = PwPri;
                    log.Info($"Using primary SQLserver {server}");
                }

                var symBuilder = new SqlConnectionStringBuilder();
                symBuilder.UserID = User;
                symBuilder.Password = PW;
                symBuilder.DataSource = server;
                symBuilder.InitialCatalog = Properties.Settings.Default.SymbolsDB;
                symBuilder.PersistSecurityInfo = true;
                TDFGlobals.dbConnSymbols = symBuilder.ConnectionString;
                log.Info($"Symbols Database {symBuilder.InitialCatalog}");
                //log.Info($"SymbolsDBConnectionString {dbConnSymbols}");

                var mktBuilder = new SqlConnectionStringBuilder();
                mktBuilder.UserID = User;
                mktBuilder.Password = PW;
                mktBuilder.DataSource = server;
                mktBuilder.InitialCatalog = Properties.Settings.Default.MarketDataDB;
                mktBuilder.PersistSecurityInfo = true;
                TDFGlobals.dbConnMarket = mktBuilder.ConnectionString;
                log.Info($"Market Database {mktBuilder.InitialCatalog}");
                //log.Info($"MarketDBConnectionString {dbConnMarket}");




                DateTime timeNow = DateTime.Now;
                if (timeNow.DayOfWeek != DayOfWeek.Saturday && timeNow.DayOfWeek != DayOfWeek.Sunday)
                    weekday = true;
                else
                    weekday = false;


                for (int i = 0; i < marketHolidays.Count; i++)
                {
                    if (marketHolidays[i].holiDate == DateTime.Today)
                        todayIsHoliday = true;
                }


                IPTextBox.Text = IPAddress;
                PortTextBox.Text = Port;
                UserTextBox.Text = UserName;
                PWTextBox.Text = PW;
                ServerTextBox.Text = ServerID.ToString();

                MarketModel.ServerReset sr = MarketFunctions.GetServerResetSched(ServerID);

                /*
                int weekNo = sr.weekNo;
                DateTime now = DateTime.Now;
                DayOfWeek resetDay = (DayOfWeek)sr.resetDay;
                DateTime resetTime = sr.resetTime;

                int srDate = FindDay(now.Year, now.Month, resetDay, weekNo);
                nextServerReset = new DateTime(now.Year, now.Month, srDate, resetTime.Hour, resetTime.Minute, resetTime.Second);
                
                if (now > nextServerReset)
                    nextServerReset = nextServerReset.AddDays(28);

                nextServerReset = nextServerReset.AddMinutes(-2);
                */

                nextServerReset = GetNextServerResetTime(sr);


                IPAddress = sr.IPAddress;
                UserName = sr.UserId;

                /*
                //disconnectTime = DateTime.Today + Properties.Settings.Default.Reset_Connection;
                disconnectTime = new DateTime(now.Year, now.Month, now.Day, resetTime.Hour, resetTime.Minute, resetTime.Second);
                if (DateTime.Now > disconnectTime)
                    disconnectTime = disconnectTime.AddDays(1);
                refTime = DateTime.Today.AddHours(1);
                */

                nextDailyReset = GetNextDailyResetTime(sr);


                TDFGlobals.showAllFields = false;

                for (int i = 0; i < 150; i++)
                {
                    for (int j = 0; j < 60; j++)
                    {
                        CatalogData[i, j] = 0;
                    }
                }

                // fields requested in hotboards
                TDFGlobals.starredFields.Add("trdPrc"); // 0
                TDFGlobals.starredFields.Add("netChg"); // 1
                TDFGlobals.starredFields.Add("ycls"); //2
                TDFGlobals.starredFields.Add("pcntChg"); //3
                TDFGlobals.starredFields.Add("hi"); // 4
                TDFGlobals.starredFields.Add("lo"); // 5
                TDFGlobals.starredFields.Add("opn"); // 6
                TDFGlobals.starredFields.Add("cumVol"); // 7
                TDFGlobals.starredFields.Add("lastActivity"); // 8
                TDFGlobals.starredFields.Add("lastActivityNetChg"); // 9
                TDFGlobals.starredFields.Add("lastActivityPcntChg"); // 10
                TDFGlobals.starredFields.Add("lastActivityVol"); // 11
                TDFGlobals.starredFields.Add("annHi"); // 12
                TDFGlobals.starredFields.Add("annLo");// 13
                TDFGlobals.starredFields.Add("isiErrCode"); // 14
                TDFGlobals.starredFields.Add("errMsg"); // 15
                TDFGlobals.starredFields.Add("issuerName"); // 16

                //XMLDataUpdated += new EventHandler<XMLUpdateEventArgs>(DisplayXMLData);
                //SymbolDataUpdated += new EventHandler<SymbolUpdateEventArgs>(SymbolDataUpdated);
                //ChartDataUpdated += new EventHandler<ChartLiveUpdateEventArgs>(ChartDataUpdated);
                //ChartClosed += new EventHandler<ChartClosedEventArgs>(ChartClosed);

                string cmd = $"SELECT * FROM MarketHolidays";
                //marketHolidays = Dow30Database.Dow30DB.GetHolidays(cmd, dbConn);
                marketHolidays = Dow30Database.Dow30DB.GetHolidays(cmd, TDFGlobals.dbConnMarket);
                
                marketIsOpen = MarketOpenStatus();
                if (marketIsOpen)
                    dataReset = false;

                // Set version number
                var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                this.Text = String.Format("Data Monitor SP1500 Application  Version {0}", version);

                // Log application start
                log.Debug($"\r\n\r\n*********** Starting Data Monitor SP1500 v{version} **********\r\n");


                TDFProcessingFunctions TDFproc = new TDFProcessingFunctions();
                TDFproc.sendBuf += new SendBuf(TRSendCommand);
                chartCnt = (Int16)(chartInterval - 3);
                ConnectToTDF();

                TODTimer.Enabled = true;

            }
            catch (Exception ex)
            {
                // Log error
                log.Error("frmMain Exception occurred during main form load: " + ex.Message);
                //log.Debug("frmMain Exception occurred during main form load", ex);
            }
            
        }



        private void ConnectButton_Click(object sender, EventArgs e)
        {
            ConnectToTDF();
        }

        public void ConnectToTDF()
        {
            // Build Logon Message
            string queryStr = "LOGON USERNAME" + "=\"" + UserTextBox.Text + "\" PASSWORD=\"" +
                PWTextBox.Text + "\"";

            // Instantiate and setup the client sockets
            // Establish the remote endpoints for the sockets
            System.Net.IPAddress TRIpAddress = System.Net.IPAddress.Parse(IPAddress);
            TRClientSocket = new ClientSocket(TRIpAddress, Convert.ToInt32(Port));

            // Initialize event handlers for the sockets
            TRClientSocket.DataReceived += TRDataReceived;
            TRClientSocket.ConnectionStatusChanged += TRConnectionStatusChanged;

            // Connect to the TRClientSocket; call-backs for connection status will indicate status of client sockets
            TRClientSocket.AutoReconnect = true;
            TRClientSocket.Connect();


            int n = 0;
            bool done = false;

            while (!TRConnected)
            {
                while (n < 2000 && !done)
                {
                    System.Threading.Thread.Sleep(10);
                    if (TRConnected == true)
                    {
                        if (this.InvokeRequired && connectLite == null)
                            this.Invoke(new ConnectLite(connectLED), false);
                        else
                        {
                            pictureBox2.Visible = true;
                        }
                        done = true;
                    }
                    n++;

                }

                if (!done)
                {
                    n = 0;
                    DialogResult dr = MessageBox.Show("Error: Did not connect.", "Error",
                        MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    if (dr == DialogResult.Cancel)
                        this.Close();
                }
            }

            ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
            byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, queryStr, TDFconstants.LOGON_REQUEST, 0);
            TRSendCommand(outputbuf);

            n = 0;
            done = false;
            //while (logResp.Length == 0)
            while (loggedIn == false)
            {
                while (n < 2000 && !done)
                {
                    System.Threading.Thread.Sleep(10);
                    if (logResp.Length > 5)
                    {
                        //lblLogResp.Text = logResp;
                        done = true;
                    }
                    n++;
                }

                if (!done)
                {
                    n = 0;
                    DialogResult dr = MessageBox.Show("Error: Not logged on.", "Error",
                        MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    if (dr == DialogResult.Cancel)
                        this.Close();
                }

            }

            TDFProcessingFunctions TDFproc = new TDFProcessingFunctions();
            TDFproc.sendBuf += new SendBuf(TRSendCommand);
            //lblLogResp.Text = logResp;
            TDFproc.GetCataloger();
            //label7.Text = "Num Catalogs: " + numCat.ToString();
            
            TDFproc.GetFieldInfoTable();
            System.Threading.Thread.Sleep(300);
            Int32 cnt = 0;

            for (int i = 0; i < TDFGlobals.field_Info_Table.Length; i++)
            {
                if (TDFGlobals.field_Info_Table[i].fieldId > 0)
                    cnt++;

            }

            label6.Text = "Number of Fields: " + cnt.ToString();
            label7.Text = "Num Catalogs: " + TDFGlobals.numCat.ToString();
            
            GetSymbols();
        }

        public void GetSymbols()
        {
            log.Debug("Initializing symbols...");
            // get data from db table to get symbol list
            string cmd = $"SELECT * FROM {dbTableName}";
            Russel3000Data.Clear();
            R3000UpdateData.Clear();
            //Russel3000Data = Dow30Database.Dow30DB.GetRussel3000SymbolDataCollection(cmd, dbConn);
            Russel3000Data = Dow30Database.Dow30DB.GetRussel3000SymbolDataCollection(cmd, TDFGlobals.dbConnSymbols);

            foreach (Dow30Database.Dow30DB.Russel3000symbolData R3000 in Russel3000Data)
                R3000UpdateData.Add(R3000);

            System.Threading.Thread.Sleep(100);

            // create symbol list and set up symbols collection
            bool first = true;
            string symListStr = "";
            uint ui = 0;
            int nSymbols = 0;
            int tot = 0;
            int totSymbols = Russel3000Data.Count;
            int n = 0;

            foreach (Dow30Database.Dow30DB.Russel3000symbolData sd in Russel3000Data)
            {
                symbolData sd1 = new symbolData();
                sd1.company_Name = MarketFunctions.GetCompanyName(sd.Symbol);
                if (sd1.company_Name != "")
                {

                    if (dynamic)
                        sd1.queryType = (int)QueryTypes.Dynamic_Quotes;
                    else
                    {
                        sd1.queryType = (int)QueryTypes.Portfolio_Mgr;
                        if (first == false)
                            symListStr += ", " + sd.Symbol;
                        else
                        {
                            symListStr = sd.Symbol;
                            first = false;
                        }

                        nSymbols++;
                        tot++;
                        if (nSymbols == 50 || tot == totSymbols)
                        {
                            symbolListStr.Add(symListStr);
                            nSymbols = 0;
                            first = true;
                        }
                    }

                    sd1.queryStr = "";
                    sd1.symbol = sd.Symbol;
                    sd1.seqId = 5;
                    sd1.updated = sd.Updated;
                    sd1.trdPrc = sd.Last;
                    sd1.netChg = sd.Change;
                    sd1.pcntChg = sd.PercentChange;
                    sd1.opn = sd.Open;
                    sd1.hi = sd.High;
                    sd1.lo = sd.Low;
                    sd1.lastActivity = sd.lastActivity;
                    sd1.lastActivityNetChg = sd.lastActivityNetChg;
                    sd1.lastActivityPcntChg = sd.lastActivityPcntChg;
                    sd1.lastActivityVol = sd.lastActivityVol;
                    sd1.annHi = sd.annHi;
                    sd1.annLo = sd.annLo;

                    TDFGlobals.symbols.Add(sd1);
                    n++;

                    if (!TDFGlobals.eflag)
                    {
                        UpdateDBCompanyName(sd1);
                        TDFGlobals.eflag = false;
                    }

                }
                else
                {
                    symbolData sym1 = GetNewSymbol(sd.Symbol);
                    string deleteCmd = $"DELETE FROM SP1500 WHERE Symbol = '{sd.Symbol}'";
                    //int nd = Dow30Database.Dow30DB.SQLExec(deleteCmd, dbConn);
                    int nd = Dow30Database.Dow30DB.SQLExec(deleteCmd, TDFGlobals.dbConnSymbols);
                    string msg = $" >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>  Symbol {sd.Symbol} {sd.Name} deleted from SP1500 database.";
                    if (sym1.symbol != "error")
                        msg += $"\r\n Replaced by {sym1.symbol}  {sym1.issuerName}";
                    if (nd > 0)
                        log.Info(msg);

                    SendEmail(msg);
                }

            }

            // re do after updating names in DB

            Russel3000Data.Clear();
            R3000UpdateData.Clear();
            //Russel3000Data = Dow30Database.Dow30DB.GetRussel3000SymbolDataCollection(cmd, dbConn);
            Russel3000Data = Dow30Database.Dow30DB.GetRussel3000SymbolDataCollection(cmd, TDFGlobals.dbConnSymbols);

            foreach (Dow30Database.Dow30DB.Russel3000symbolData R3000 in Russel3000Data)
                R3000UpdateData.Add(R3000);

            if (dynamic)
            {
                log.Debug("Issuing dynamic subscription queries...");
                foreach (Dow30Database.Dow30DB.Russel3000symbolData sd in Russel3000Data)
                {
                    ui++;
                    IssueDynamicSubscriptionQuery(sd.Symbol, ui);
                    Thread.Sleep(2);
                }
                log.Debug("Dynamic subscription queries complete ...");
            }
            // start data collection
            timer1.Enabled = true;
        }

        public symbolData GetNewSymbol(string symbol)
        {
            string fieldList = "issuerName";
            string quot = "\"";
            string query = $"SELECT {fieldList} FROM QUOTES WHERE usrSymbol= {quot}{symbol}{quot}";
            
            symbolData sd1 = new symbolData();
            sd1.symbol = "error";
            sd1.issuerName = "";
            sd1.seqId = 5;
            sd1.isiErrCode = 0;
            sd1.errMsg = "";

            if (TRConnected)
            {
                ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
                byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, query, TDFconstants.DATA_REQUEST, 5);

                TRSendCommand(outputbuf);
                Thread.Sleep(2);
            }

            bool done = false;
            string s = "";
            string sym = "";
            int cnt = 0;
            int frNum = 1;
            Thread.Sleep(100);

            while (!done)
            {
                if (TDFGlobals.financialResults.Count >= frNum)
                {
                    for (int i = 0; i < TDFGlobals.financialResults.Count; i++)
                    {
                        sym = TDFGlobals.financialResults[i].symbol;
                        if (TDFGlobals.financialResults[i].fieldName == "issuerName")
                        {
                            sd1.symbol = sym;
                            sd1.issuerName = TDFGlobals.financialResults[i].sData;
                        }
                        else if (TDFGlobals.financialResults[i].fieldName == "errMsg" || TDFGlobals.financialResults[i].fieldName == "isiErrCode")
                        {
                            sd1.symbol = "error";
                            sd1.issuerName = "";
                        }

                    }
                    TDFGlobals.financialResults.Clear();
                    done = true;
                }
                else
                {
                    Thread.Sleep(10);
                    cnt++;
                    if (cnt > 20)
                        done = true;
                }
            }

            return sd1;

        }

        public void IssueDynamicSubscriptionQuery(string symbolStr, uint seq)
        {
            
            try
            {
                string fieldList = "trdPrc, netChg, pcntChg, opn, hi, lo, cumVol, lastActivity, lastActivityNetChg, lastActivityPcntChg, lastActivityVol, annHi, annLo";
                string query = $"SELECT {fieldList} FROM DYNAMIC_QUOTES WHERE usrSymbol= {quot}{symbolStr}{quot}";
                if (TRConnected)
                {
                    ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
                    byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, query, TDFconstants.DATA_REQUEST, seq);

                    TRSendCommand(outputbuf);
                }
            }
            catch (Exception ex)
            {
                log.Error($"IssueDynamicSubscriptionQuery error - {ex}");
            }
        }
        
        #region Socket Handlers
        // Handler for data received back from TRclientsocket
        private void TRDataReceived(ClientSocket sender, byte[] data)
        {

            TDFDataReceived(sender, data);
        }

        private void TDFDataReceived(ClientSocket sender, byte[] data)
        {
            try
            {
                // receive the data and determine the type
                //int bufLen = sender.bufLen;
                int bufLen = data.Length;
                rCnt++;
                bytesReceived += bufLen;
                byte[] rData = new byte[bufLen];
                Array.Copy(data, 0, rData, 0, bufLen);
                TRdata.AddRange(rData);
                //TRdata.AddRange(data);
                bool waitForData = false;
                bool dynamicFlag = false;
                int len = 0;
                mt = 0;
                msgSize = 0;

                dataLeft = TRdata.Count;
                dataReceivedTime = DateTime.Now;

                TDFProcessingFunctions TDFproc = new TDFProcessingFunctions();

                while (dataLeft >= 23 && waitForData == false)
                {
                    ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
                    itf_Parser_Return_Message TRmessage = new itf_Parser_Return_Message();
                    itf_Parser_Update_Message TRupdateMessage = new itf_Parser_Update_Message();
                    itf_Control_Message TRControlMessage = new itf_Control_Message();

                    mt = itfHeaderAccess.GetMsgType(TRdata.ToArray());
                    msgSize = itfHeaderAccess.GetMsgSize(TRdata.ToArray());

                    if (msgSize <= dataLeft)
                    {
                        if (mt == TDFconstants.DYNAMIC_UPDATE)
                        {
                            try
                            {
                                TRupdateMessage = itfHeaderAccess.ParseItfUpdateMessage(TRdata.ToArray());
                                if (msgSize <= TRupdateMessage.totalMessageSize)
                                    TDFproc.ProcessFinancialUpdateData(TRupdateMessage);
                                //Task.Run(() => TDFproc.ProcessFinancialUpdateData(TRupdateMessage));
                                if (msgSize + 1 >= TRdata.Count)
                                    len = TRdata.Count;
                                else
                                    len = msgSize + 1;
                                //TRdata.RemoveRange(0, msgSize + 1);
                                TRdata.RemoveRange(0, len);
                                dataLeft = TRdata.Count;
                                dynamicFlag = true;
                                WatchdogTimer.Enabled = false;
                            }
                            catch (Exception ex)
                            {
                                log.Error($"Dynamic Update error: {ex}");
                            }
                        }
                        else if (mt == TDFconstants.DYNAMIC_CONTROL)
                        {

                            try
                            {
                                TRControlMessage = itfHeaderAccess.ParseItfControlMessage(TRdata.ToArray());
                                //log.Info($"Control Message Code: {TRControlMessage.control_Message_Header.messageCode}   Control Message: {TRControlMessage.Message}");
                                string msg = $"CONTROL MESSAGE CODE RECEIVED: {TRControlMessage.control_Message_Header.messageCode}   CONTROL MESSAGE: {TRControlMessage.Message}";
                                log.Info(msg);

                                if (DateTime.Now > lastDCEmail.AddHours(1))
                                {
                                    Task.Run(() => SendEmail(msg));
                                    lastDCEmail = DateTime.Now;
                                }

                                if (msgSize + 1 >= TRdata.Count)
                                    len = TRdata.Count;
                                else
                                    len = msgSize + 1;
                                TRdata.RemoveRange(0, len);
                                dataLeft = TRdata.Count;
                                //TRdata.RemoveRange(0, TRmessage.itf_Header.msgSize + 1);
                                //dataLeft = TRdata.Count;
                            }
                            catch (Exception ex)
                            {
                                log.Error($"Dynamic Control Error: {ex}");
                            }
                        }
                        else if (mt == TDFconstants.LOGOFF_RESPONSE)
                        {
                            TRmessage = itfHeaderAccess.ParseItfMessage(TRdata.ToArray());
                            logResp = System.Text.Encoding.Default.GetString(TRmessage.Message.ToArray());
                            messages.Add("Logoff at " + DateTime.Now.ToString());
                            log.Info(logResp);
                            TRdata.Clear();
                            dataLeft = TRdata.Count;
                            loggedIn = false;

                            switch (TRmessage.data_Header.respType)
                            {
                                case TDFconstants.SUCCCESSFUL_LOGON_LOGOFF:
                                    log.Info("Logoff " + logResp);
                                    break;

                                case TDFconstants.ERROR_LOGON_LOGOFF:
                                    log.Info("Logoff Error " + logResp);
                                    break;
                            }
                        }
                        else if (mt == TDFconstants.LOGON_RESPONSE)
                        {
                            TRmessage = itfHeaderAccess.ParseItfMessage(TRdata.ToArray());
                            logResp = System.Text.Encoding.Default.GetString(TRmessage.Message.ToArray());
                            messages.Add("Logon at " + DateTime.Now.ToString());
                            TRdata.Clear();
                            dataLeft = TRdata.Count;
                            
                            switch (TRmessage.data_Header.respType)
                            {
                                case TDFconstants.SUCCCESSFUL_LOGON_LOGOFF:
                                    // get and save session ID
                                    stdHeadr.sessionId = TRmessage.itf_Header.sessionId;
                                    log.Info("Logon at " + DateTime.Now.ToString());
                                    log.Info(logResp);
                                    loggedIn = true;
                                    break;

                                case TDFconstants.ERROR_LOGON_LOGOFF:
                                    log.Info("Logon Error " + logResp);
                                    loggedIn = false;
                                    break;
                            }

                        }
                        else if (mt == TDFconstants.KEEP_ALIVE_REQUEST)
                        {
                            try
                            {
                                string ka = System.Text.Encoding.Default.GetString(TRmessage.Message.ToArray());
                                lastKA = DateTime.Now;
                                statusStr = "Keep Alive at " + DateTime.Now.ToString() + " 1 " + ka;
                                messages.Add(statusStr);
                                log.Info(statusStr);

                                ProcessKeepAliveRequest(TRmessage);
                                if (TRdata.Count >= msgSize + 1)
                                    TRdata.RemoveRange(0, msgSize + 1);
                                else
                                    TRdata.RemoveRange(0, TRdata.Count);

                                dataLeft = TRdata.Count;
                            }
                            catch (Exception ex)
                            {
                                log.Error($"KEEP ALIVE REQUEST error: {ex}");
                            }
                        }
                        else if (mt == TDFconstants.DATA_RESPONSE)
                        {
                            try
                            {
                                TRmessage = itfHeaderAccess.ParseItfMessage(TRdata.ToArray());
                                switch(TRmessage.data_Header.respType)
                                {

                                    case TDFconstants.CATALOGER_RESPONSE:
                                        catStr = TDFproc.ProcessCataloger(TRmessage.Message.ToArray());
                                        TRdata.Clear();
                                        dataLeft = TRdata.Count;
                                        break;

                                    case TDFconstants.OPEN_FID_RESPONSE:
                                        if (TRmessage.itf_Header.seqId == 98)
                                        {
                                            TDFproc.ProcessFieldInfoTable(TRmessage);
                                            TRdata.RemoveRange(0, TRmessage.itf_Header.msgSize + 1);
                                            dataLeft = TRdata.Count;
                                        }
                                        else
                                        {
                                            TDFproc.ProcessFinancialData(TRmessage);
                                            TRdata.RemoveRange(0, TRmessage.itf_Header.msgSize + 1);
                                            dataLeft = TRdata.Count;
                                            WatchdogTimer.Enabled = false;
                                        }
                                        break;

                                    case TDFconstants.SUBSCRIPTION_RESPONSE:
                                        TDFproc.ProcessFinancialData(TRmessage);
                                        TRdata.RemoveRange(0, TRmessage.itf_Header.msgSize + 1);
                                        dataLeft = TRdata.Count;
                                        break;

                                    case TDFconstants.UNSUBSCRIPTION_RESPONSE:
                                        TRdata.RemoveRange(0, msgSize + 1);
                                        dataLeft = TRdata.Count;
                                        break;

                                    case TDFconstants.XML_RESPONSE:
                                        XMLStr = System.Text.Encoding.Default.GetString(TRmessage.Message.ToArray());
                                        MemoryStream ms = new MemoryStream(TRmessage.Message.ToArray());
                                        xmlResponse.Load(ms);
                                        break;

                                    case TDFconstants.XML_CHART_RESPONSE:
                                        XMLStr += System.Text.Encoding.Default.GetString(TRmessage.Message.ToArray());
                                        PreProXML xmlData = TDFproc.GetXmlType(TRmessage);

                                        switch (xmlData.xmlCode)
                                        {
                                            case XMLTypes.XMLCharts:
                                                Chart_Data chart1Data = new Chart_Data();
                                                chart1Data = TDFproc.ProcessXMLChartData(xmlData);
                                                charts.Add(chart1Data);
                                                sw.Stop();
                                                break;

                                            case XMLTypes.marketPages:
                                                marketPage = TDFproc.ProcessMarketPages(xmlData);
                                                pageDataFlag = true;
                                                break;

                                            case XMLTypes.bpPages:
                                                marketPage = TDFproc.ProcessBusinessPages(xmlData);
                                                pageDataFlag = true;
                                                break;
                                        }

                                        TRdata.RemoveRange(0, TRmessage.itf_Header.msgSize + 1);
                                        dataLeft = TRdata.Count;
                                        break;

                                    case TDFconstants.KEEP_ALIVE_REQUEST:
                                        string ka = System.Text.Encoding.Default.GetString(TRmessage.Message.ToArray());
                                        statusStr = "Keep Alive at " + DateTime.Now.ToString() + " 2 " + ka;
                                        TDFProcessingFunctions TDFproc5 = new TDFProcessingFunctions();
                                        TDFproc5.ProcessKeepAliveRequest(TRmessage);
                                        TRdata.RemoveRange(0, TRmessage.itf_Header.msgSize);
                                        dataLeft = TRdata.Count;
                                        statusStr = "Keep Alive at " + DateTime.Now.ToString();
                                        messages.Add(statusStr);
                                        break;

                                    default:
                                        statusStr = "Message type " + TRmessage.itf_Header.msgType.ToString() +
                                            "  Message Response " + TRmessage.data_Header.respType.ToString() + "  " + DateTime.Now.ToString();

                                        log.Error(statusStr);

                                        TRdata.RemoveRange(0, msgSize + 1);
                                        dataLeft = TRdata.Count;
                                        break;

                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error($"DATA_RESPONSE error: {ex}");
                            }
                        }
                        else
                        {
                            if (TRdata[0] == 2)
                                TRmessage = itfHeaderAccess.ParseItfMessage(TRdata.ToArray());
                            else
                            {
                                log.Error("--- Sync byte not found!");
                                /*
                                TRdata.Clear();
                                dataLeft = TRdata.Count;

                                log.Debug("--- Receive buffer cleared!");
                                */

                                int n = 0;
                                while (TRdata[0] != 2 && TRdata.Count > 0)
                                {
                                    TRdata.RemoveRange(0, 1);
                                    n++;
                                }
                                log.Debug($"--- {n} Bytes removed!");


                                //if (TRdata.Count > 0)
                                //TRmessage = itfHeaderAccess.ParseItfMessage(TRdata.ToArray());
                                //return;
                            }
                        }
                    }
                    else
                        waitForData = true;

                }

                if (dynamicFlag)
                {
                    dynamicFlag = false;
                    //Task.Run(() => UpdateDynamicSymbols());
                    //UpdateDynamicSymbols();
                }
                
            }
            catch (Exception ex)
            {
                log.Error($"TRDataReceived error - {ex}");
            }
        }




        // Handler for source & destination MSE connection status change
        public void TRConnectionStatusChanged(ClientSocket sender, ClientSocket.ConnectionStatus status)
        {
            // Set status
            if (status == ClientSocket.ConnectionStatus.Connected)
            {
                TRConnected = true;
            }
            else
            {
                TRConnected = false;
            }
            if (debugMode)
                messages.Add("status: " + status.ToString());

            // Send to log - DEBUG ONLY
            log.Debug("TR Connection Status: " + status.ToString());
        }

        // Send a command to TDF
        public void TRSendCommand(byte[] outbuf)

        {
            try
            {
                // Send the data; terminiate with CRLF
                TRClientSocket.Send(outbuf);
            }
            catch (Exception ex)
            {
                // Log error
                log.Error("Error occurred while trying to send data to TR client port: " + ex.Message);
            }
        }
        #endregion;
        
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        
        public void GetDow30Data()
        {
            stopWatch.Start();
            string fieldList = "trdPrc, netChg, pcntChg, open, high, low, cumVol";
            for (int i = 0; i < symbolListStr.Count; i++)
            {
                string query = $"SELECT {fieldList} FROM PORTFOLIO_MGR WHERE usrSymbol IN ({symbolListStr[i]})";
                if (TRConnected)
                {
                    ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
                    byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, query, TDFconstants.DATA_REQUEST, 5);

                    TRSendCommand(outputbuf);
                    Thread.Sleep(10);
                }
            }
            WatchdogTimer.Enabled = true;

        }

        public void GetYesterdaysClose()
        {
            string s = "";
            string sym = "";
            string oldSym = "";

            timer1.Enabled = false;
            Thread.Sleep(50);
            string query = "SELECT ycls FROM PORTFOLIO_MGR WHERE usrSymbol IN (.DJIA,.NCOMP,.SPX)";
            if (TRConnected)
            {
                ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
                byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, query, TDFconstants.DATA_REQUEST, 5);

                TRSendCommand(outputbuf);
                //WatchdogTimer.Enabled = true;
            }
            Thread.Sleep(50);

            if (TDFGlobals.financialResults.Count > 0)
            {
                for (int i = 0; i < TDFGlobals.financialResults.Count; i++)
                {

                    int symbolIndex = TDFProcessingFunctions.GetSymbolIndx(TDFGlobals.financialResults[i].symbol);
                    sym = TDFGlobals.financialResults[i].symbol;
                    if (sym != oldSym && sym != null)
                    {
                        s = TDFGlobals.financialResults[i].symbolFull;
                        oldSym = sym;
                    }


                    if (TDFGlobals.financialResults.Count > 0)
                        TDFProcessingFunctions.SetSymbolData(TDFGlobals.financialResults, i, symbolIndex);

                }
                TDFGlobals.financialResults.Clear();

                UpdateAllSymbols(true);

                timer1.Enabled = true;


            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                string s = "";
                string sym = "";
                string oldSym = "";

                if (dynamic == false)
                {
                    GetDow30Data();
                    Thread.Sleep(50);

                    if (TDFGlobals.financialResults.Count > 0)
                    {
                        for (int i = 0; i < TDFGlobals.financialResults.Count; i++)
                        {

                            int symbolIndex = TDFProcessingFunctions.GetSymbolIndx(TDFGlobals.financialResults[i].symbol);
                            sym = TDFGlobals.financialResults[i].symbol;
                            if (sym != oldSym && sym != null)
                            {
                                s = TDFGlobals.financialResults[i].symbolFull;
                                oldSym = sym;
                            }

                            if (TDFGlobals.financialResults.Count > 0)
                                TDFProcessingFunctions.SetSymbolData(TDFGlobals.financialResults, i, symbolIndex);

                        }
                        TDFGlobals.financialResults.Clear();

                        UpdateAllSymbols(false);

                        string connection = $"SELECT * FROM {dbTableName}";
                        //Russel3000Data = Dow30Database.Dow30DB.GetRussel3000SymbolDataCollection(connection, dbConn);
                        Russel3000Data = Dow30Database.Dow30DB.GetRussel3000SymbolDataCollection(connection, TDFGlobals.dbConnSymbols);
                        //label1.Text = $"Elapsed: {ts.TotalMilliseconds.ToString()} msec";
                    }
                }
                else
                {
                    if (!updateFlag)
                    {
                        updateFlag = true;
                        Task.Run(() => UpdateAll());
                    }
                    else
                    {
                        nSkip++;
                    }
                    
                    label8.Text = $"Processing Time: {ts.TotalMilliseconds.ToString()} msec";
                    label13.Text = $"Processing Max: {tsMax.TotalMilliseconds.ToString()} msec";
                    label14.Text = $"Number of skips: {nSkip}";

                    tCnt++;
                    if (tCnt % 5 == 0)
                    {
                        //DataTable dt = new DataTable();
                        //dt = ListToDataTable<Dow30Database.Dow30DB.Russel3000symbolData>(Russel3000Data);
                        //UpdateRussel3000Table(dt);

                        if (dataReset == false && marketIsOpen == false)
                        {
                            int nZeros = 0;
                            for (int i = 0; i < Russel3000Data.Count; i++)
                            {
                                if (Russel3000Data[i].Change == 0)
                                    nZeros++;
                            }

                            if (nZeros > (Russel3000Data.Count - 100) && dataReset == false)
                            {
                                log.Debug($"Data reset at: {DateTime.Now}");
                                DataResetLabel.Text = $"Data reset at: {DateTime.Now}";
                                dataReset = true;
                            }
                        }


                    }
                    label2.Text = $"Total Symbols: {nSymGood}";
                    label9.Text = $"Bad Symbols: {nSymBad}";
                    nTick++;
                }
            }
            catch (Exception ex)
            {
                log.Error($"Timer1 error: {ex}");
            }

        }
        public void UpdateAll()
        {
            updateFlag = true;
            stopWatch.Start();
            
            R3000UpdateData.Clear();
            if (TDFGlobals.financialResults.Count > 0)
                UpdateDynamicSymbols();

            DataTable dt = new DataTable();
            dt = ListToDataTable<Dow30Database.Dow30DB.Russel3000symbolData>(R3000UpdateData);

            //Task.Run(() => UpdateRussel3000Table(dt));
            Task.Run(() => UpdateSP1500Table(dt));

            stopWatch.Stop();
            ts = stopWatch.Elapsed;
            stopWatch.Reset();
            if (ts > tsMax)
                tsMax = ts;

            updateFlag = false;

        }

        public static DataTable ListToDataTable<T>(List<T> list)
        {
            DataTable dt = new DataTable();

            //var pInfo = typeof(T).GetProperty("transSrc").GetCustomAttribute<MaxLengthAttribute>();
            //var maxLen = pInfo.Length;

            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                var col = new DataColumn(info.Name, info.PropertyType);
                try
                {
                    /*
                    var pInfo = typeof(T).GetProperty(info.Name).GetCustomAttribute<MaxLengthAttribute>();
                    if (pInfo != null)
                    {
                        var maxLen = pInfo.Length;
                        col.MaxLength = maxLen;
                    }
                    */
                    dt.Columns.Add(col);
                }
                catch (Exception ex)
                {
                    var err = ex.Message;

                }

            }
            foreach (T t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(T).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null);
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        public void UpdateAllSymbols(bool ycls)
        {
            int nZeros = 0;
            for (int i = 0; i < TDFGlobals.symbols.Count; i++)
            {
                symbolData sd = new symbolData();
                sd = TDFGlobals.symbols[i];
                if (marketIsOpen == false && sd.netChg == 0.0 && dataReset == false)
                    nZeros++;

                if (Russel3000Data[i].Change != sd.netChg && sd.trdPrc != 0)
                {
                    UpdateDB(sd);
                    
                }
                if (ycls)
                {
                    if (sd.symbol == ".DJIA")
                        dowValue = sd.ycls;
                    if (sd.symbol == ".NCOMP")
                        nasdaqValue = sd.ycls;
                    if (sd.symbol == ".SPX")
                        spxValue = sd.ycls;

                }
                else
                {
                    if (sd.symbol == ".DJIA")
                        dowValue = sd.trdPrc;
                    if (sd.symbol == ".NCOMP")
                        nasdaqValue = sd.trdPrc;
                    if (sd.symbol == ".SPX")
                        spxValue = sd.trdPrc;

                }

            }

            if (nZeros > TDFGlobals.symbols.Count - 10 && dataReset == false)
            {
                log.Debug($"Data reset at: {DateTime.Now}");
                DataResetLabel.Text = $"Data reset at: {DateTime.Now}";
                dataReset = true;
            }
        }

        public void UpdateDynamicSymbols()
        {
            int eCode = 0;
            string sym = "";
            string oldSym = "";
            int symbolIndex = -1;
            bool updateNewSymbol = false;
            List<int> updateIndx = new List<int>();

            try
            {
                
                int n = TDFGlobals.financialResults.Count;
                if (n > 0)
                {
                    for (int i = 0; i < n; i++)
                    {
                        eCode = 1;
                        if (i <= TDFGlobals.financialResults.Count - 1)
                        {
                            fin_Data fd = new fin_Data();
                            fd = TDFGlobals.financialResults[i];
                            sym = TDFGlobals.financialResults[i].symbol;
                            eCode = 22;
                            if (sym != oldSym && sym != null)
                            {
                                // new symbol
                                // Update previous symbol except first one or last symbol was an error
                                eCode = 23;
                                if (i > 0)
                                {
                                    // update previous symbol
                                    if (symbolIndex >= 0 && symbolError == false)
                                        UpdateSymbol(symbolIndex);
                                    else
                                    {
                                        log.Error($"Previous symbol not updated.  Symbol : {oldSym}");
                                    }
                                }

                                eCode = 2;
                                symbolIndex = TDFProcessingFunctions.GetSymbolIndx(TDFGlobals.financialResults[i].symbol);
                                eCode = 24;
                                if (symbolIndex >= 0)
                                {
                                    symbolError = false;
                                    TDFGlobals.symbols[symbolIndex].updated = DateTime.Now;
                                    eCode = 25;

                                }
                                else
                                {
                                    //fin_Data fd = new fin_Data();
                                    eCode = 26;

                                    if (sym.Length > 0)
                                        fixSymbols(TDFGlobals.financialResults[i]);
                                    eCode = 27;

                                    log.Error($"symbolIndex < 0  Symbol : {sym}  fieldName : {fd.fieldName}");

                                }
                                oldSym = sym;
                                eCode = 3;

                            }

                            eCode = 28;

                            if (symbolIndex >= 0)
                            {
                                eCode = 4;
                                TDFProcessingFunctions.SetSymbolData(TDFGlobals.financialResults, i, symbolIndex);
                            }
                            else if (sym != null && sym.Length > 0)
                            {
                                eCode = 5;
                                log.Error($"symbolIndex < 0  Symbol : {sym}  fieldName : {TDFGlobals.financialResults[i].fieldName}");
                                //fd = TDFGlobals.financialResults[i];
                                fixSymbols(TDFGlobals.financialResults[i]);
                                eCode = 6;

                            }

                            if (TDFGlobals.financialResults[i].fieldName == "issuerName")
                            {

                                eCode = 7;

                                string newSym = TDFGlobals.financialResults[i].symbol;
                                int newSymbolIndex = TDFProcessingFunctions.GetSymbolIndx(newSym);
                                if (newSymbolIndex >= 0)
                                {
                                    if (TDFGlobals.symbols[newSymbolIndex].issuerName != null && TDFGlobals.symbols[newSymbolIndex].issuerName.Length > 0)
                                    {
                                        eCode = 17;
                                        updateNewSymbol = true;
                                        Russel3000Data[symbolIndex].Name = TDFGlobals.symbols[symbolIndex].issuerName;
                                        log.Info($"symbolIndex < 0  Symbol : {sym}  fieldName : {fd.fieldName}  Name : {TDFGlobals.symbols[i].issuerName}");
                                        updateIndx.Add(symbolIndex);
                                        eCode = 18;

                                    }
                                }
                                else
                                {
                                    eCode = 19;
                                    log.Info($"newSymbolIndex < 0  Symbol : {newSym}  Name : {TDFGlobals.financialResults[i].fieldName}");

                                }
                            }
                        }
                    }
                }

                // Do last symbol
                eCode = 8;

                if (symbolIndex >= 0)
                {
                    eCode = 9;

                    UpdateSymbol(symbolIndex);
                    eCode = 10;

                }
                else
                {
                    eCode = 11;

                    log.Error($"symbolIndex < 0  Symbol : {sym}");
                    if (sym.Length > 0)
                        fixSymbols(TDFGlobals.financialResults[n - 1]);
                    eCode = 12;

                }

                if (updateNewSymbol)
                    for (int i = 0; i < updateIndx.Count; i++)
                        UpdateDBNewSymbol(TDFGlobals.symbols[updateIndx[i]]);
                eCode = 13;

                TDFGlobals.financialResults.RemoveRange(0, n);
                eCode = 14;

            }
            catch (Exception ex)
            {

                bool emailSent = false;
                string msg = $"UpdateDynamicSymbols: eCode: {eCode}   sym: {sym} {symbolIndex}  oldSym: {oldSym}   {ex.Message}    {ex.Source}    {ex.StackTrace}";
                if (DateTime.Now > lastEmail.AddMinutes(60))
                {
                    SendEmail(msg);
                    emailSent = true;
                }
                log.Error(msg);
                TRdata.Clear();
                
                log.Debug($"Starting error reset procedure.....");
                resetting = true;
                //timer1.Enabled = false;
                if (dynamic)
                {
                    UnsubscribeAll();
                    log.Debug("Unsubscribe complete.");
                }
                DisconnectFromTDF();
                Thread.Sleep(50);
                TDFGlobals.symbols.Clear();
                TDFGlobals.financialResults.Clear();
                dataLeft = 0;
                TRdata.Clear();
                log.Debug($"Reconnecting now .....");

                //log.Debug("Starting reconnect...");
                TDFGlobals.eflag = true;
                ConnectToTDF();
                Thread.Sleep(2000);
                resetting = false;
                if (TRConnected == true)
                {
                    resetComplete = true;
                    timer1.Enabled = true;
                    log.Debug("Reset complete.");
                }
                else
                {
                    resetFlag = true;
                    msg = "[" + DateTime.Now + "] DataMonitor reset error. Failed to reconnect after timed disconnect.";
                    if (DateTime.Now > lastEmail.AddMinutes(15))
                    {
                        SendEmail(msg);
                        emailSent = true;
                    }
                    log.Debug("DataMonitor reset error. Failed to reconnect after timed disconnect.");
                }
                if (emailSent)
                    lastEmail = DateTime.Now;
                timer1.Enabled = true;
                WatchdogTimer.Enabled = true;
                TDFGlobals.eflag = false;
            }

        }

        public  void UpdateSymbol(int symIndx)
        {
            try
            {
                string s;
                
                if (symIndx >= 0)
                {
                    var R3000 = Russel3000Data[symIndx];
                    string sym = TDFGlobals.symbols[symIndx].symbol;
                    if (sym == R3000.Symbol)
                    {
                        Dow30Database.Dow30DB.Russel3000symbolData rsd = new Dow30Database.Dow30DB.Russel3000symbolData();
                        symbolData sd = new symbolData();
                        rsd = R3000;
                        sd = TDFGlobals.symbols[symIndx];

                        if (R3000.NewHi == false)
                        {
                            if (TDFGlobals.symbols[symIndx].hi > R3000.annHi || TDFGlobals.symbols[symIndx].trdPrc > R3000.annHi ||
                                TDFGlobals.symbols[symIndx].annHi > R3000.annHi)
                            {
                                R3000.NewHi = true;
                                log.Debug($"New Hi {sd.symbol} Symbol annHi: {sd.annHi}  trdPrc: {sd.trdPrc}  Hi: {sd.hi}  Old annHi: {rsd.annHi}");
                            }
                        }

                        if (TDFGlobals.symbols[symIndx].lo != 0 && R3000.NewLo == false)
                        {
                            if (TDFGlobals.symbols[symIndx].lo < R3000.annLo || TDFGlobals.symbols[symIndx].trdPrc < R3000.annLo ||
                                TDFGlobals.symbols[symIndx].annLo < R3000.annLo)
                            {
                                R3000.NewLo = true;
                                log.Debug($"New Lo {sd.symbol} Symbol annLo: {sd.annLo}  trdPrc: {sd.trdPrc}  Lo: {sd.lo}  Old annLo: {rsd.annLo}");
                            }
                        }
                        else
                            s = "Zero detected";

                        R3000.Last = TDFGlobals.symbols[symIndx].trdPrc;
                        R3000.Change = TDFGlobals.symbols[symIndx].netChg;
                        R3000.PercentChange = TDFGlobals.symbols[symIndx].pcntChg;
                        R3000.Open = TDFGlobals.symbols[symIndx].opn;
                        R3000.High = TDFGlobals.symbols[symIndx].hi;
                        R3000.Low = TDFGlobals.symbols[symIndx].lo;
                        R3000.Volume = TDFGlobals.symbols[symIndx].cumVol;
                        R3000.Updated = DateTime.Now;


                        R3000.lastActivity = TDFGlobals.symbols[symIndx].lastActivity;
                        R3000.lastActivityNetChg = TDFGlobals.symbols[symIndx].lastActivityNetChg;
                        R3000.lastActivityPcntChg = TDFGlobals.symbols[symIndx].lastActivityPcntChg;
                        R3000.lastActivityVol = TDFGlobals.symbols[symIndx].lastActivityVol;
                        R3000.annHi = TDFGlobals.symbols[symIndx].annHi;
                        R3000.annLo = TDFGlobals.symbols[symIndx].annLo;


                        DataGridViewRow row = new DataGridViewRow();
                        int indx = R3000UpdateData.FindIndex(x => x.Symbol == R3000.Symbol);
                        if (indx == -1)
                        {
                            R3000UpdateData.Add(R3000);
                            //row = symbolDataGrid.Rows[R3000UpdateData.Count - 1]; 

                        }
                        else
                        {
                            //R3000UpdateData.RemoveAt(indx);
                            //R3000UpdateData.Add(R3000);
                            nDup++;
                            R3000UpdateData[indx].Last = R3000.Last;
                            R3000UpdateData[indx].Change = R3000.Change;
                            R3000UpdateData[indx].PercentChange = R3000.PercentChange;
                            R3000UpdateData[indx].Open = R3000.Open;
                            R3000UpdateData[indx].High = R3000.High;
                            R3000UpdateData[indx].Low = R3000.Low;
                            R3000UpdateData[indx].Volume = R3000.Volume;
                            R3000UpdateData[indx].Updated = DateTime.Now;

                            R3000UpdateData[indx].lastActivity = R3000.lastActivity;
                            R3000UpdateData[indx].lastActivityNetChg = R3000.lastActivityNetChg;
                            R3000UpdateData[indx].lastActivityPcntChg = R3000.lastActivityPcntChg;
                            R3000UpdateData[indx].lastActivityVol = R3000.lastActivityVol;
                            R3000UpdateData[indx].annHi = R3000.annHi;
                            R3000UpdateData[indx].annLo = R3000.annLo;
                        }
                        //UpdateDB(TDFGlobals.symbols[symIndx]);

                        Russel3000Data[symIndx] = R3000; 
                        lastDataReceived = DateTime.Now;
                        nSymGood++;
                    }
                    else
                        log.Error("Symbols don't match");
                }
                else
                    nSymBad++;
            }
            catch (Exception ex)
            {
                log.Error($"UpdateSymbol: {ex}");
            }

        }

        public void UpdateRussel3000Table(DataTable dt)
        {
            string spName = "sp_UpdateTableRussel3000";
            string cmdStr = $"{spName} ";

            //Save out the top-level metadata
            try
            {
                // Instantiate the connection
                using (SqlConnection connection = new SqlConnection(TDFGlobals.dbConnSymbols))
                {
                    connection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            SqlTransaction transaction;
                            // Start a local transaction.
                            transaction = connection.BeginTransaction("Update Russel3000 Table");

                            // Must assign both transaction object and connection 
                            // to Command object for a pending local transaction
                            cmd.Connection = connection;
                            cmd.Transaction = transaction;

                            try
                            {
                                //Specify base command
                                cmd.CommandText = cmdStr;

                                cmd.Parameters.Add("@tblR3000", SqlDbType.Structured).Value = dt;

                                sqlDataAdapter.SelectCommand = cmd;
                                sqlDataAdapter.SelectCommand.Connection = connection;
                                sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                                // Execute stored proc 
                                sqlDataAdapter.SelectCommand.ExecuteNonQuery();

                                //Attempt to commit the transaction
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                log.Error("UpdateRussel3000Table: " + ex.Message);
                                
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error("UpdateRussel3000Table: " + ex.Message);
            }

        }

        public void UpdateSP1500Table(DataTable dt)
        {
            string spName = "sp_UpdateTableSP1500";
            string cmdStr = $"{spName} ";

            //Save out the top-level metadata
            try
            {
                // Instantiate the connection
                using (SqlConnection connection = new SqlConnection(TDFGlobals.dbConnSymbols))
                {
                    connection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            SqlTransaction transaction;
                            // Start a local transaction.
                            transaction = connection.BeginTransaction("Update SP1500 Table");

                            // Must assign both transaction object and connection 
                            // to Command object for a pending local transaction
                            cmd.Connection = connection;
                            cmd.Transaction = transaction;

                            try
                            {
                                //Specify base command
                                cmd.CommandText = cmdStr;

                                cmd.Parameters.Add("@tblSP1500", SqlDbType.Structured).Value = dt;

                                sqlDataAdapter.SelectCommand = cmd;
                                sqlDataAdapter.SelectCommand.Connection = connection;
                                sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                                // Execute stored proc 
                                sqlDataAdapter.SelectCommand.ExecuteNonQuery();

                                //Attempt to commit the transaction
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                log.Error("UpdateSP1500Table: " + ex.Message);

                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error("UpdateSP1500Table: " + ex.Message);
            }

        }

        public void UpdateDB(symbolData sd)
        {
            //spName = "sp_Insert_Russel3000_Data";
            spName = "sp_Insert_SP1500_Data";

            string cmdStr = $"{spName} @Symbol, @Name, @Last, @Change, @PercentChange, @Open, @High, @Low, @Volume, @Updated, @Dow, @Nasdaq100, @SP, @Sector";
            cmdStr += ", @lastActivity, @lastActivityNetChg, @lastActivityPcntChg, @lastActivityVol, @annHi, @annLo, @NewHi, @NewLo";

            //Save out the top-level metadata
            try
            {
                // Instantiate the connection
                using (SqlConnection connection = new SqlConnection(TDFGlobals.dbConnSymbols))
                {
                    connection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            SqlTransaction transaction;
                            // Start a local transaction.
                            transaction = connection.BeginTransaction("Update Russel 3000 Data");

                            // Must assign both transaction object and connection 
                            // to Command object for a pending local transaction
                            cmd.Connection = connection;
                            cmd.Transaction = transaction;

                            try
                            {
                                //Specify base command
                                cmd.CommandText = cmdStr;

                                cmd.Parameters.Add("@Symbol", SqlDbType.Text).Value = sd.symbol;
                                cmd.Parameters.Add("@Name", SqlDbType.Text).Value = sd.issuerName;
                                cmd.Parameters.Add("@Last", SqlDbType.Float).Value = sd.trdPrc;
                                cmd.Parameters.Add("@Change", SqlDbType.Float).Value = sd.netChg;
                                cmd.Parameters.Add("@PercentChange", SqlDbType.Float).Value = sd.pcntChg;
                                cmd.Parameters.Add("@Open", SqlDbType.Float).Value = sd.opn;
                                cmd.Parameters.Add("@High", SqlDbType.Float).Value = sd.hi;
                                cmd.Parameters.Add("@Low", SqlDbType.Float).Value = sd.lo;
                                cmd.Parameters.Add("@Volume", SqlDbType.BigInt).Value = sd.cumVol;
                                cmd.Parameters.Add("@Updated", SqlDbType.DateTime).Value = DateTime.Now;

                                cmd.Parameters.Add("@Dow", SqlDbType.Bit).Value = false;
                                cmd.Parameters.Add("@Nasdaq100", SqlDbType.Bit).Value = false;
                                cmd.Parameters.Add("@SP", SqlDbType.Bit).Value = false;
                                cmd.Parameters.Add("@Sector", SqlDbType.Int).Value = 0;

                                cmd.Parameters.Add("@lastActivity", SqlDbType.Float).Value = sd.lastActivity;
                                cmd.Parameters.Add("@lastActivityNetChg", SqlDbType.Float).Value = sd.lastActivityNetChg;
                                cmd.Parameters.Add("@lastActivityPcntChg", SqlDbType.Float).Value = sd.lastActivityPcntChg;
                                cmd.Parameters.Add("@lastActivityVol", SqlDbType.BigInt).Value = sd.lastActivityVol;
                                cmd.Parameters.Add("@annHi", SqlDbType.Float).Value = sd.annHi;
                                cmd.Parameters.Add("@annLo", SqlDbType.Float).Value = sd.annLo;

                                cmd.Parameters.Add("@NewHi", SqlDbType.Bit).Value = false;
                                cmd.Parameters.Add("@NewLo", SqlDbType.Bit).Value = false;

                                sqlDataAdapter.SelectCommand = cmd;
                                sqlDataAdapter.SelectCommand.Connection = connection;
                                sqlDataAdapter.SelectCommand.CommandType = CommandType.Text;

                                // Execute stored proc to store top-level metadata
                                sqlDataAdapter.SelectCommand.ExecuteNonQuery();

                                //Attempt to commit the transaction
                                transaction.Commit();

                                log.Info($"Stored proc: {cmdStr}");
                                for (int i = 0; i < cmd.Parameters.Count; i++)
                                    log.Info($"cmd params {i}. {cmd.Parameters[i].ParameterName} : {cmd.Parameters[i].Value}");

                                log.Info($"Symbol {sd.symbol} has beed added to the DB");

                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                log.Error("UpdateData- SQL Command Exception occurred: " + ex.Message);
                                
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error("UpdateData- SQL Connection Exception occurred: " + ex.Message);
                
            }
            
        }
        public void UpdateDBCompanyName(symbolData sd)
        {
            spName = "sp_UpdateCompanyNameSP1500";

            string cmdStr = $"{spName} @Search_Symbol, @Company_Name_Short";
            
            //Save out the top-level metadata
            try
            {
                // Instantiate the connection
                using (SqlConnection connection = new SqlConnection(TDFGlobals.dbConnSymbols))
                {
                    connection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            SqlTransaction transaction;
                            // Start a local transaction.
                            transaction = connection.BeginTransaction("Update SP1500 CompanyName");

                            // Must assign both transaction object and connection 
                            // to Command object for a pending local transaction
                            cmd.Connection = connection;
                            cmd.Transaction = transaction;

                            try
                            {
                                //Specify base command
                                cmd.CommandText = cmdStr;

                                cmd.Parameters.Add("@Search_Symbol", SqlDbType.Text).Value = sd.symbol;
                                cmd.Parameters.Add("@Company_Name_Short", SqlDbType.Text).Value = sd.company_Name;
                                
                                sqlDataAdapter.SelectCommand = cmd;
                                sqlDataAdapter.SelectCommand.Connection = connection;
                                sqlDataAdapter.SelectCommand.CommandType = CommandType.Text;

                                // Execute stored proc to store top-level metadata
                                sqlDataAdapter.SelectCommand.ExecuteNonQuery();

                                //Attempt to commit the transaction
                                transaction.Commit();

                                //log.Info($"Stored proc: {cmdStr}");
                                //for (int i = 0; i < cmd.Parameters.Count; i++)
                                    //log.Info($"cmd params {i}. {cmd.Parameters[i].ParameterName} : {cmd.Parameters[i].Value}");

                                //log.Info($"Symbol {sd.symbol} has beed added to the DB");

                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                log.Error("UpdateData- SQL Command Exception occurred: " + ex.Message);

                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error("UpdateData- SQL Connection Exception occurred: " + ex.Message);

            }

        }

        public void UpdateChartData(X20ChartData cd)
        {
            //string cmdStr = "sp_Insert_ChartData @Updated, @Dow, @NASDAQ, @SP";
            string cmdStr = spUpdateChart +  " @Updated, @Dow, @NASDAQ, @SP";

            //Save out the top-level metadata
            try
            {
                // Instantiate the connection
                using (SqlConnection connection = new SqlConnection(TDFGlobals.dbConnSymbols))
                {
                    connection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            SqlTransaction transaction;
                            // Start a local transaction.
                            transaction = connection.BeginTransaction("Update X20 Chart Data");

                            // Must assign both transaction object and connection 
                            // to Command object for a pending local transaction
                            cmd.Connection = connection;
                            cmd.Transaction = transaction;

                            try
                            {

                                Int64 UnixTimeInMSecUtc = GetCurrentUnixTimestampMillis();
                                string UnixTimeInMSecUtcStr = GetCurrentUnixTimestampMillisLocalTime().ToString();
                                //Specify base command
                                cmd.CommandText = cmdStr;

                                //cmd.Parameters.Add("@Updated", SqlDbType.BigInt).Value = UnixTimeInMSecUtc;
                                cmd.Parameters.Add("@Updated", SqlDbType.VarChar).Value = UnixTimeInMSecUtcStr;
                                cmd.Parameters.Add("@Dow", SqlDbType.Float).Value = cd.dow;
                                cmd.Parameters.Add("@NASDAQ", SqlDbType.Float).Value = cd.nasdaq;
                                cmd.Parameters.Add("@SP", SqlDbType.Float).Value = cd.spx;
                                
                                sqlDataAdapter.SelectCommand = cmd;
                                sqlDataAdapter.SelectCommand.Connection = connection;
                                sqlDataAdapter.SelectCommand.CommandType = CommandType.Text;

                                // Execute stored proc to store top-level metadata
                                sqlDataAdapter.SelectCommand.ExecuteNonQuery();

                                //Attempt to commit the transaction
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                log.Error("Update X20 Chart Data- SQL Command Exception occurred: " + ex.Message);
                                log.Debug("Update X20 Chart Data- SQL Command Exception occurred", ex);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error("UpdateData- SQL Connection Exception occurred: " + ex.Message);

            }

        }


        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long GetCurrentUnixTimestampMillis()
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
        }
        public static long GetCurrentUnixTimestampMillisLocalTime()
        {
            return (long)(DateTime.Now - UnixEpoch).TotalMilliseconds;
        }

        public void SendEmail(string msg)
        {
            //MailMessage mail = new MailMessage("TDFDow30App@foxnews.com", "242 -GFX Engineering <GFXEngineering@FOXNEWS.COM>");
            MailMessage mail = new MailMessage("DataMonitor@foxnews.com", "alex.stivala@foxnews.com");

            SmtpClient mailClient = new SmtpClient();
            mailClient.Port = 25;
            mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            mailClient.UseDefaultCredentials = true;
            mailClient.Host = "10.232.16.121";
            mail.Subject = "DataMonitor Alert";
            //mail.Subject = "TDFDow30 Test Email";
            //mail.Body = "[" + DateTime.Now + "] " + Environment.NewLine + "The data monitor application has encountered a error" + Environment.NewLine + e.ToString();
            //mail.Body = "[" + DateTime.Now + "] " + Environment.NewLine + "This is a test message!" + Environment.NewLine;
            //mail.Body = "This is a greeting from the TDFDow30 application - just saying hello." + Environment.NewLine +
            //"No need to worry, no Fatal Exception Errors, no Warnings, just running smooooooth." + Environment.NewLine +
            //"Almost could be a Corona commercial.";
            mail.Body = msg;
            mailClient.Send(mail);
        }

        
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dynamic)
            {
                UnsubscribeAll();
                log.Debug("Unsubscribe complete");
            }
            Logoff();
            Thread.Sleep(100);
            log.Debug("*****  TDFRussell3000 Closed *****");
        }
        private void Logoff()
        {
            // Build Logon Message
            string queryStr = "LOGOFF";

            ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
            byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, queryStr, TDFconstants.LOGOFF_REQUEST, 0);
            TRSendCommand(outputbuf);
        }
        public void DisconnectFromTDF()
        {
            Logoff();
            Thread.Sleep(50);
            TRClientSocket.Disconnect();
            if (this.InvokeRequired && connectLite == null)
                this.Invoke(new ConnectLite(connectLED), false);
            else
            {
                pictureBox2.Visible = false;
            }
            Thread.Sleep(50);
            
        }

        public void connectLED(bool on)
        {
            if (on)
                pictureBox2.Visible = true;
            else
                pictureBox2.Visible = false;
        }


        private void TODTimer_Tick(object sender, EventArgs e)
        {
            timeOfDayLabel.Text = DateTime.Now.ToString("MMM d, yyyy -- h:mm:ss tt");
            nSec++;
            nSymPerSec = nSymGood - oldCnt;
            nSymPerMin += nSymPerSec;

            //nTickTot += nTick;
            //if (nSec % 60 == 0)
            //nTickPerMin = nTickTot / (nSec / 60);

            TDFGlobals.marketOpenStatus = MarketFunctions.IsMarketOpen();


            /*
            TimeSpan dataCheckTime = new TimeSpan();

            if (TDFGlobals.marketOpenStatus)
                dataCheckTime = tenSeconds;
            else
                dataCheckTime = tenMinutes;

            if (DateTime.Now - lastDataReceived > dataCheckTime)
            {
                TRdata.Clear();
                ServerReset(false);
                if (TDFGlobals.marketOpenStatus)
                {
                    ReconnectTimer.Interval = 100;
                    ReconnectTimer.Enabled = false;
                    ReconnectTimer.Enabled = true;
                    log.Debug($"Reconnecting now.....");
                }
            }
            */

            nTickTot += nTick;
            if (nSec % 60 == 0)
            {
                nTickPerMin = nTickTot;
                nTickTot = 0;
                label1.Text = $"Symbols/Min: {nSymPerMin}";
                nSymPerMin = 1;
            }


            label5.Text = $"Symbols/Sec: {nSymPerSec}";
            label10.Text = $"Updates/Sec: {nTick}";
            label11.Text = $"Updates/Min: {nTickPerMin}";
            label12.Text = $"Repeats/Update: {nDup}";
            oldCnt = nSymGood;
            nTick = 0;
            nDup = 0;


            if (TRConnected)
                pictureBox2.Visible = true;
            else
                pictureBox2.Visible = false;

            lblLogResp.Text = logResp;

            messages.Clear();

            DateTime today = DateTime.Today;
            if (day != today)
            {
                day = today;
                if (today.DayOfWeek != DayOfWeek.Saturday && today.DayOfWeek != DayOfWeek.Sunday)
                    weekday = true;
                else
                    weekday = false;

                for (int i = 0; i < marketHolidays.Count; i++)
                {
                    if (marketHolidays[i].holiDate == today)
                        todayIsHoliday = true;
                }

            }

            
            //TimeSpan marketOpen = new TimeSpan(9, 29,58); //9:30 am
            //TimeSpan marketClose = new TimeSpan(16, 10, 0); //4:06 pm  somtimes data is updated a bit after market close
            TimeSpan currentTime = DateTime.Now.TimeOfDay;

            if (currentTime > marketOpen && currentTime < marketClose && weekday == true && todayIsHoliday == false)
            {

                if (marketIsOpen == false)
                {
                    // market just opened
                    if (updateChartData)
                    {
                        string cmd = $"DELETE FROM " + dbChartTableName;
                        int numRows;
                        numRows = Dow30Database.Dow30DB.SQLExec(cmd, dbConn);
                        string s = $"Deleted {numRows} DB chart records.";
                        //listBox1.Items.Add(s);
                        log.Debug(s);
                        Thread.Sleep(50);
                        GetYesterdaysClose();
                        chartCnt = (short)(chartInterval - 2);

                        if (chartCnt == chartInterval)
                        {
                            chartCnt = 0;
                            X20_chartData.dow = dowValue;
                            X20_chartData.nasdaq = nasdaqValue;
                            X20_chartData.spx = spxValue;
                            UpdateChartData(X20_chartData);
                        }

                    }
                    dataReset = false;
                    tsMax = TimeSpan.Zero;
                    nSkip = 0;
                    nSymGood = 0;
                    ClearHiLo();
                }
                marketIsOpen = true;
                chartCnt++;

            }
            else
                marketIsOpen = false;

            //if (timerFlag == true && DateTime.Now > refTime)
                //timerFlag = false;

            if (timerFlag == true && DateTime.Now > timerEmailSent.AddDays(1))
                timerFlag = false;

            // if server is scheduled for reset - Get next time for both resets
            if (DateTime.Now > nextServerReset)
            {
                MarketModel.ServerReset sr = MarketFunctions.GetServerResetSched(ServerID);
                nextServerReset = GetNextServerResetTime(sr);
                nextDailyReset = GetNextDailyResetTime(sr);
                ServerReset(true);
            }

            if (DateTime.Now > nextDailyReset)
            {
                MarketModel.ServerReset sr = MarketFunctions.GetServerResetSched(ServerID);
                nextDailyReset = GetNextDailyResetTime(sr);
                ServerReset(false);
            }


            /*
            if (resetFlag == true && DateTime.Now > refTime)
                resetFlag = false;


            if (resetComplete == true && DateTime.Now > refTime)
                resetComplete = false;


            if (resetComplete == false && resetting == false && DateTime.Now > disconnectTime)
            {
                ClearHiLo();
                ResetTDFConnection(true);
                InitTimer.Enabled = true;
                log.Debug("TOD timer reset.");
                
            }
            */

            if (zipperFlag == true && DateTime.Now > zipperEmailSent.AddDays(1))
                zipperFlag = false;
        }

        public void ClearHiLo()
        {
            int hiCnt = 0;
            int loCnt = 0;

            for (int i = 0; i < Russel3000Data.Count; i ++)
            {
                if (Russel3000Data[i].NewHi)
                {
                    Russel3000Data[i].NewHi = false;
                    hiCnt++;
                }
                if (Russel3000Data[i].NewLo)
                {
                    Russel3000Data[i].NewLo = false;
                    loCnt++;
                }
            }
            log.Debug($"{hiCnt} NewHi's and {loCnt} Lo's reset");
        }

        public void ResetTDFConnection(bool resetTime)
        {
            log.Debug("Resetting TDF Connection");
            resetting = true;
            timer1.Enabled = false;
            if (dynamic)
            {
                UnsubscribeAll();
                log.Debug("Unsubscribe complete.");
            }
            DisconnectFromTDF();
            Thread.Sleep(5000);
            TDFGlobals.symbols.Clear();
            TDFGlobals.financialResults.Clear();
            ConnectToTDF();
            resetting = false;
            Thread.Sleep(1000);
            if (TRConnected == true)
            {
                resetComplete = true;
                if (resetTime)
                    disconnectTime = disconnectTime.AddDays(1);
                refTime = refTime.AddDays(1);
                timer1.Enabled = true;
                log.Debug("Reset complete");
            }
            else
            {
                if (resetFlag == false)
                {
                    resetFlag = true;
                    string msg = "[" + DateTime.Now + "] DataMonitor reset error. Failed to reconnect after timed disconnect.";
                    SendEmail(msg);
                    log.Debug("DataMonitor reset error. Failed to reconnect after timed disconnect.");
                }
            }
        }


        public void ServerReset(bool isServerReset)
        {

            // Daily reset Values
            int resetMinutes = 2;
            string resetType = "Daily";

            if (isServerReset)
            {
                //Server reset values
                resetMinutes = 60;
                resetType = "Server";
            }

            log.Debug($"Starting {resetType} Reset procedure.....");
            resetting = true;
            timer1.Enabled = false;
            if (dynamic)
            {
                UnsubscribeAll();
                log.Debug("Unsubscribe complete.");
            }
            DisconnectFromTDF();
            TDFGlobals.symbols.Clear();
            TDFGlobals.financialResults.Clear();
            
            ReconnectTimer.Interval = 60000 * resetMinutes;
            ReconnectTimer.Enabled = false;
            ReconnectTimer.Enabled = true;
            log.Debug($"Reconnecting in {resetMinutes} minutes.....");
            
        }




        public void UnsubscribeAll()
        {
            ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
            string sym;
            string queryStr;
            log.Debug("Unsubscribing...");

            foreach (symbolData sd in TDFGlobals.symbols)
            {
                sym = sd.symbolFull;
                queryStr = $"DELETE FROM SUBSCRIPTION_TABLE WHERE channelName = DYNAMIC_QUOTES AND usrSymbol = {quot}{sym}{quot}";
                byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, queryStr, TDFconstants.DATA_REQUEST, 1);
                TRSendCommand(outputbuf);
                //listBox1.Items.Add(queryStr);
                Thread.Sleep(2);
                
            }
        }
        
    

        private void gbTime_Enter(object sender, EventArgs e)
        {

        }

        
        private void button6_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
            if (timer1.Enabled)
                button6.Text = "Pause";
            else
                button6.Text = "GO";
        }
        public void UpdateZipperDataFile1()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(zipperFilePath + "ZipperDataFile.xml");
            XmlNodeList symbolNodes = xmlDoc.SelectNodes("//SYMBOLS/SYMBOL");

            int i = 0;

            foreach (XmlNode sym in symbolNodes)
            {
                
                sym.Attributes["value"].Value = TDFGlobals.symbols[i].trdPrc.ToString();

                if (TDFGlobals.symbols[i].netChg > 0)
                {
                    sym.Attributes["value"].Value = TDFGlobals.symbols[i].trdPrc.ToString();
                    sym.Attributes["change"].Value = TDFGlobals.symbols[i].netChg.ToString();
                    sym.Attributes["arrow"].Value = "up.jpg";
                }
                else if (TDFGlobals.symbols[i].netChg < 0)
                {
                    sym.Attributes["value"].Value = TDFGlobals.symbols[i].trdPrc.ToString();
                    float absChange = Math.Abs(TDFGlobals.symbols[i].netChg);
                    sym.Attributes["change"].Value = absChange.ToString();
                    sym.Attributes["arrow"].Value = "down.jpg";
                }
                else if (TDFGlobals.symbols[i].netChg == 0)
                {
                    sym.Attributes["value"].Value = TDFGlobals.symbols[i].trdPrc.ToString();
                    sym.Attributes["change"].Value = "UNCH";
                }
                i++;
                
            }
            xmlDoc.Save(zipperFilePath + "ZipperDataFile.xml");
            
        }


        public void UpdateZipperDataFile()
        {

            try
            {
                XmlWriter xmlWriter = XmlWriter.Create(zipperFilePath + "ZipperDataFile.xml");
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("SYMBOLS");
                double value;
                string str;

                for (int i = 0; i < 3; i++)
                {

                    xmlWriter.WriteStartElement("SYMBOL");
                    value = TDFGlobals.symbols[i].trdPrc;
                    str = TDFGlobals.symbols[i].trdPrc.ToString("#####.##");
                    //xmlWriter.WriteAttributeString("name", TDFGlobals.symbols[i].name.ToString());
                    //xmlWriter.WriteAttributeString("value", TDFGlobals.symbols[i].trdPrc.ToString("#####.##"));

                    //xmlWriter.WriteAttributeString("name", TDFGlobals.symbols[i].name.ToString());
                    xmlWriter.WriteAttributeString("name", TDFGlobals.symbols[i].company_Name.ToString());
                    xmlWriter.WriteAttributeString("value", str);
                    if (TDFGlobals.symbols[i].netChg > 0)
                    {
                        value = TDFGlobals.symbols[i].netChg;
                        str = TDFGlobals.symbols[i].netChg.ToString("#####.##");
                        //xmlWriter.WriteAttributeString("change", TDFGlobals.symbols[i].netChg.ToString("#####.##"));

                        xmlWriter.WriteAttributeString("change", str);
                        xmlWriter.WriteAttributeString("arrow", "up.jpg");
                    }
                    else if (TDFGlobals.symbols[i].netChg < 0)
                    {
                        float absChange = Math.Abs(TDFGlobals.symbols[i].netChg);
                        str = absChange.ToString("#####.##");
                        //xmlWriter.WriteAttributeString("change", absChange.ToString("#####.##"));
                        xmlWriter.WriteAttributeString("change", str);
                        xmlWriter.WriteAttributeString("arrow", "down.jpg");
                    }
                    else if (TDFGlobals.symbols[i].netChg == 0)
                    {
                        xmlWriter.WriteAttributeString("change", "UNCH");
                    }

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndDocument();
                xmlWriter.Close();

            }
            catch
            {
                if (zipperFlag == false)
                {
                    zipperFlag = true;
                    string msg = "[" + DateTime.Now + "] DataMonitor write error. Error writing to Zipper Data File.";
                    SendEmail(msg);
                    zipperEmailSent = DateTime.Now;
                    log.Debug("DataMonitor write error. Error writing to Zipper Data File.");
                }

            }
            
        }

        
        private void WatchdogTimer_Tick(object sender, EventArgs e)
        {
            if (timerFlag == false)
            {
                timerFlag = true;
                string msg = "[" + DateTime.Now + "] DataMonitor response error. Data requested with no response.";
                log.Error("WatchdogTimer -" + msg);
                SendEmail(msg);
                timerEmailSent = DateTime.Now;
                DisconnectFromTDF();
                ResetTimer.Enabled = true;
                timer1.Enabled = false;
            }
            log.Debug("DataMonitor response error. Data requested with no response.");

        }

        private void ResetTimer_Tick(object sender, EventArgs e)
        {
            ResetTimer.Enabled = false;
            log.Debug("Reset Timer fired.");
            ResetTDFConnection(false);
        }

        public bool MarketOpenStatus()
        {
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            DateTime timeNow = DateTime.Now;
            bool marketOpenStat;

            bool weekday;
            if (timeNow.DayOfWeek != DayOfWeek.Saturday && timeNow.DayOfWeek != DayOfWeek.Sunday)
                weekday = true;
            else
                weekday = false;

            bool todayIsHoliday = false;

            for (int i = 0; i < marketHolidays.Count; i++)
            {
                if (marketHolidays[i].holiDate == DateTime.Today)
                    todayIsHoliday = true;
            }

            if (currentTime > marketOpen && currentTime < marketClose && weekday == true && todayIsHoliday == false)
            {


                marketOpenStat = true;
                
            }
            else
                marketOpenStat = false;

            return marketOpenStat;

        }

        public void ProcessKeepAliveRequest(itf_Parser_Return_Message TRmess)
        {
            try
            {
                // Build Logon Message
                string queryStr = System.Text.Encoding.Default.GetString(TRmess.Message.ToArray());

                ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
                byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, queryStr, TDFconstants.KEEP_ALIVE_RESPONSE, 0);
                TRSendCommand(outputbuf);
                //sendBuf(outputbuf);
            }
            catch (Exception ex)
            {
                log.Error($"Process KEEP ALIVE REQUEST error: {ex}");
            }
        }

        private void InitTimer_Tick(object sender, EventArgs e)
        {
            InitTimer.Enabled = false;
            tsMax = TimeSpan.Zero;
            nSkip = 0;
        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void fixSymbols(fin_Data fd)
        {

            try
            {
                if (fd.fieldName != "isiErrCode" && fd.fieldName != "errMsg")
                {
                    symbolData sd = new symbolData();
                    Dow30Database.Dow30DB.Russel3000symbolData R3000 = new Dow30Database.Dow30DB.Russel3000symbolData();
                    sd.symbol = fd.symbol;
                    R3000.Symbol = fd.symbol;
                    int indx = addedSymbols.FindIndex(x => x.symbol == R3000.Symbol);

                    if (indx < 0)
                    {
                        Russel3000Data.Add(R3000);
                        TDFGlobals.symbols.Add(sd);
                        addedSymbols.Add(sd);
                        try
                        {
                            string fieldList = "trdPrc, netChg, pcntChg, opn, hi, lo, cumVol, lastActivity, lastActivityNetChg, lastActivityPcntChg, lastActivityVol, annHi, annLo, issuerName";
                            string query = $"SELECT {fieldList} FROM QUOTES WHERE usrSymbol= {quot}{fd.symbol}{quot}";
                            if (TRConnected)
                            {
                                ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
                                byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, query, TDFconstants.DATA_REQUEST, uint.MaxValue);

                                TRSendCommand(outputbuf);
                                log.Info($"Symbol {fd.symbol} not in the list. Checking info.");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error($"fixSymbols error - {ex}");
                        }
                    }
                }
                else
                {
                    if (fd.fieldName == "isiErrCode")
                        log.Error($"Symbol { fd.symbol} has returned an error code of {fd.iData}");
                    if (fd.fieldName == "errMsg")
                        log.Error($"Symbol { fd.symbol} has returned an error {fd.sData}");
                    symbolError = true;
                }
            }
            catch (Exception ex)
            {
                log.Error($"fixSymbols error; {ex}");
            }
        }

        private void UpdateDBNewSymbol(symbolData sd)
        {
            try
            {
                UpdateDB(sd);
                string msg = $"[{DateTime.Now}] New symbol detected:  {sd.symbol}   {sd.issuerName} {Environment.NewLine} {Environment.NewLine}Adding {sd.symbol} to DB.";
                log.Info($"[{DateTime.Now}] New symbol detected:  {sd.symbol}   {sd.issuerName} {Environment.NewLine} {Environment.NewLine}Adding {sd.symbol} to DB.");
                SendEmail(msg);
            }
            catch (Exception ex)
            {
                log.Error($"UpdateDBNewSymbol error; {ex}");
            }
        }

        private void label13_Click(object sender, EventArgs e)
        {
            tsMax = TimeSpan.MinValue;
            nSkip = 0;
        }
        
        //For example to find the day for 2nd Friday, February, 2016
        //=>call FindDay(2016, 2, DayOfWeek.Friday, 2)
        public static int FindDay(int year, int month, DayOfWeek Day, int occurance)
        {

            if (occurance <= 0 || occurance > 5)
                throw new Exception("Occurance is invalid");

            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            //Substract first day of the month with the required day of the week 
            var daysneeded = (int)Day - (int)firstDayOfMonth.DayOfWeek;
            //if it is less than zero we need to get the next week day (add 7 days)
            if (daysneeded < 0) daysneeded = daysneeded + 7;
            //DayOfWeek is zero index based; multiply by the Occurance to get the day
            var resultedDay = (daysneeded + 1) + (7 * (occurance - 1));

            if (resultedDay > (firstDayOfMonth.AddMonths(1) - firstDayOfMonth).Days)
                throw new Exception(String.Format("No {0} occurance(s) of {1} in the required month", occurance, Day.ToString()));

            return resultedDay;
        }

        private void ReconnectTimer_Tick(object sender, EventArgs e)
        {
            ReconnectTimer.Enabled = false;
            log.Debug("Starting reconnect...");
            ConnectToTDF();
            Thread.Sleep(2000);
            resetting = false;
            if (TRConnected == true)
            {
                resetComplete = true;
                timer1.Enabled = true;
                log.Debug("Reset complete.");
            }
            else
            {
                resetFlag = true;
                string msg = "[" + DateTime.Now + "] DataMonitor reset error. Failed to reconnect after timed disconnect.";
                SendEmail(msg);
                log.Debug("DataMonitor reset error. Failed to reconnect after timed disconnect.");
            }
        }

        public DateTime GetNextServerResetTime(MarketModel.ServerReset sr)
        {
            int weekNo = sr.weekNo;
            DateTime now = DateTime.Now;
            DayOfWeek resetDay = (DayOfWeek)sr.resetDay;
            DateTime resetTime = sr.resetTime;
            DateTime nextServerReset;

            int srDate = FindDay(now.Year, now.Month, resetDay, weekNo);
            nextServerReset = new DateTime(now.Year, now.Month, srDate, resetTime.Hour, resetTime.Minute, resetTime.Second);

            if (now.AddMinutes(3) > nextServerReset)
                nextServerReset = nextServerReset.AddDays(28);

            nextServerReset = nextServerReset.AddMinutes(-2);
            ServerResetLabel.Text = $"Next Server Reset: {nextServerReset}";

            return nextServerReset;

        }

        public DateTime GetNextDailyResetTime(MarketModel.ServerReset sr)
        {
            DateTime disconnectTime;
            DateTime resetTime = sr.resetTime;

            disconnectTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, resetTime.Hour, resetTime.Minute, resetTime.Second);
            if (DateTime.Now.AddMinutes(3) >= disconnectTime)
                disconnectTime = disconnectTime.AddDays(1);

            DailyResetLabel.Text = $"Next Daily Reset: {disconnectTime}";

            return disconnectTime;

        }

        private void DataResetLabel_Click(object sender, EventArgs e)
        {

        }

        private void DailyResetLabel_Click(object sender, EventArgs e)
        {

        }
    }

 

}


