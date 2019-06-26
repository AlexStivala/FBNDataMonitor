using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDFInterface
{
    public static class MarketModel
    {
        public class MarketHolidays
        {
            public string holiday { get; set; }
            public DateTime holiDate { get; set; }
        }

        public class CompanyInfo
        {
            public SymbolDef symbol = new SymbolDef();
            public string Search_Symbol { get; set; }
            public string Ticker_Symbol1 { get; set; }
            public int Instrument_Type { get; set; }
            public string Instrument_Type_Mnemonic { get; set; }
            public string Trading_Exchange { get; set; }
            public string Currency { get; set; }
            public string Session { get; set; }
            public string Company_Name_Long { get; set; }
            public string Company_Name_Short { get; set; }
            public bool symbol_Valid { get; set; }
            public int securityType { get; set; }

        }

        public class BusinessPulsePages
        {
            public string pageCode { get; set; }
            public string pageDescription { get; set; }
        }
        public class MarketPulsePages
        {
            public string pageCode { get; set; }
            public string pageDescription { get; set; }
            public string exchange { get; set; }
        }
        public class marketSort
        {
            public string symbol { get; set; }
            public string companyName { get; set; }
            public float chng { get; set; }
            public float trdPrc { get; set; }
            public float pcntChg { get; set; }
            public Int64 cumVol { get; set; }
        }

        public class BoardData
        {
            public string title { get; set; }
            public string subTitle { get; set; }
            public int boardId { get; set; }
            public int boardType { get; set; }
            public int requestId { get; set; }

            public List<marketSort> symbolData = new List<marketSort>();
        }

        public class SymbolValues
        {
            public string valueStr { get; set; }
            public string chgStr { get; set; }
            public string pchgStr { get; set; }
        }

        public class DataRequests
        {
            public string IPAddress { get; set; }
            public int Port { get; set; }
            public int requestId { get; set; }
            public int requestType { get; set; }
            public uint seqStrt { get; set; }
            public uint seqEnd { get; set; }
            public int numItems { get; set; }
            public string description { get; set; }
            public List<uint> seqIDs = new List<uint>();
            public List<DataRequestSymbol> symbols = new List<DataRequestSymbol>();
            public List<string> queries = new List<string>();
            
        }

        public class DataRequestSymbol
        {
            public string symbol { get; set; }
            public string mapKey { get; set; }
            public float refVals { get; set; }
        }


        public class dataReceived
        {
            public int requestId { get; set; }
            public int requestType { get; set; }
            public int numItems { get; set; }
            public List<uint> seqIDs = new List<uint>();

        }

        public class LiveUpdates
        {
            public string IPAddress { get; set; }
            public int Port { get; set; }
            public int requestType { get; set; }
            public List<SymbolDef> symbols = new List<SymbolDef>();

        }

        public class SeqIds
        {
            public int boards { get; set; }
            public int charts { get; set; }
            public int changeSince { get; set; }
            public int pulse { get; set; }
        }

        public class SymbolDef
        {
            public string symbol { get; set; }
            public string tickerSymbol { get; set; }
            public string symbolFull { get; set; }
            public string symbolEx { get; set; }
            public string companyName { get; set; }
            public int securityType { get; set; }
            public string currency { get; set; }
            public string session { get; set; }
            public string exchange { get; set; }
            public string variant { get; set; }
            public bool fullyQualified { get; set; }
        }

        public class ServerReset
        {
            public string IPAddress { get; set; }
            public int weekNo { get; set; }
            public int ServerId { get; set; }
            public string UserId { get; set; }
            public DateTime resetTime { get; set; }
            public int resetDay { get; set; }
        }
    }
}
