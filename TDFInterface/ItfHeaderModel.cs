using System;
using System.Collections.Generic;

namespace TDFInterface
{
    // Thomson-Reuters ITF Message Header Definition
    public class itf_Header
    {
        public byte sync { get; set; }
        public ushort msgSize { get; set; }  // little endian
        public byte dataOffset { get; set; }
        public byte msgType { get; set; }
        public uint seqId { get; set; }
        public ushort protId { get; set; } // little endian
        public ushort sessionId { get; set; }
    }
    public class itf_Short_Header
    {
        public byte sync { get; set; }
        public ushort msgSize { get; set; }  // little endian
        public byte dataOffset { get; set; }
        public byte msgType { get; set; }
        }
    public class log_data_Header
    {
        public ushort dataSize { get; set; }  // little endian
        public byte respType { get; set; }
        public byte reserved { get; set; }
        public ushort errCode { get; set; } 

    }
    public class service_data_Header
    {
        public ushort errCode { get; set; }
        public ushort respType { get; set; }
        public ushort numRec { get; set; }  // little endian
        public byte respCtrl{ get; set; }
        public byte headerExt { get; set; }
        
    }
    public class return_data_Header
    {
        public ushort errCode { get; set; }
        public ushort respType { get; set; }
        public ushort numRecSize { get; set; }  // little endian
        public byte respCtrl { get; set; }
        public bool entitlements { get; set; }
        public bool controlRecord { get; set; }
        public bool localCatalogger { get; set; }
        public bool traversal { get; set; }
        public bool more { get; set; }

    }

    public class itf_Log_Message
    {
        public itf_Header itf_Header { get; set; }
        public log_data_Header data_Header { get; set; }
        public byte[] Message { get; set; }  

    }
    public class itf_Service_Message
    {
        public itf_Header itf_Header { get; set; }
        public service_data_Header data_Header { get; set; }
        public byte[] Message { get; set; }

    }
    public class itf_Parser_Return_Message
    {
        public itf_Header itf_Header { get; set; }
        public return_data_Header data_Header { get; set; }
        //public byte[] Message { get; set; }
        public List<byte> Message = new List<byte>();
        public Int32 totalMessageSize { get; set; }
        
    }
    public class itf_Parser_Update_Message
    {
        public itf_Short_Header itf_Short_Header { get; set; }
        public List<byte> Message = new List<byte>();
        public Int32 totalMessageSize { get; set; }

    }
    public class itf_Control_Message
    {
        public itf_Short_Header itf_Short_Header { get; set; }
        public Control_Message_Header control_Message_Header { get; set; }
        public int messageLen { get; set; }
        public List<byte> Message = new List<byte>();
        public Int32 totalMessageSize { get; set; }

    }
    public class Control_Message_Header
    {
        public ushort numRec { get; set; }
        public byte messageCategory { get; set; }
        public byte messageCode { get; set; }

    }

    public class FIDS_List
    {
        public List<UInt32> catFIDS = new List<UInt32>();
    }

    public class last_Response
    {
        public byte messageType { get; set; }
        public ushort responseType { get; set; }
        public ushort responseCtrl { get; set; }
    }
    public struct field_Info
    {
        public UInt32 fieldId { get; set; }
        public UInt16 businessId { get; set; }
        public string fieldName { get; set; }
        public ushort fieldfFmtId { get; set; }
        public string fieldFmtOptional { get; set; }
        public ushort fieldType { get; set; }
        public string fieldDesc { get; set; }
        public byte fieldDataType { get; set; }
        public byte numBytes { get; set; }
        public bool show { get; set; }
        public int dataIndx { get; set; }

    }

    public class local_Cataloger
    {
        public ushort rec_Size { get; set; }
        public ushort header { get; set; }
        public ushort numPatterns { get; set; }
        public ushort patternNumber { get; set; }
        public ushort numFIDS { get; set; }
        public List<UInt32> FIDs = new List<UInt32>(); 
        
    }

    public struct fin_Data
    {
        public UInt32 fieldId { get; set; }
        public UInt16 businessId { get; set; }
        public string fieldName { get; set; }
        public ushort fieldfFmtId { get; set; }
        public string fieldFmtOptional { get; set; }
        public byte fieldDataType { get; set; }
        public string symbol { get; set; }
        public string symbolFull { get; set; }
        public int queryType { get; set; }
        public float fData { get; set; }
        public double dData { get; set; }
        public Int32 iData { get; set; }
        public Int64 hData { get; set; }
        public string sData { get; set; }
        public byte bData { get; set; }
        public DateTime dtData { set; get; }
        public ushort operation { get; set; }
        public byte numBytes { get; set; }
        public int resultIndx { get; set; }
        public bool show { get; set; }
        public int dataIndx { get; set; }
        
    }

    public struct Open_FID_Hdr
    {
        public bool symBol { set; get; }
        public bool littleEndndian { set; get; }
        public bool directives { get; set; }
        public byte msgCmd { set; get; }
        public byte msgStruct { set; get; }
        public string msgCmdStr { set; get; }
        public string msgStructStr { set; get; }
    }
    public class symbolData
    {
        public string symbol { get; set; }
        public string symbolEx { get; set; }
        public string symbolFull { get; set; }
        public string company_Name { get; set; }
        public UInt32 seqId { get; set; }
        public int queryType { get; set; }
        public string queryStr { get; set; }
        public int sectyType { get; set; }
        public float trdPrc { get; set; }
        public float netChg { get; set; }
        public float pcntChg { get; set; }
        public float ycls { get; set; }
        public float hi { get; set; }
        public float lo { get; set; }
        public float annHi { get; set; }
        public float annLo { get; set; }
        public Int64 cumVol { get; set; }
        public float peRatio { get; set; }
        public float eps { get; set; }
        public float divAnn { get; set; }
        public float bid { get; set; }
        public float ask { get; set; }
        public float lastActivity { get; set; }
        public float lastActivityNetChg { get; set; }
        public float lastActivityPcntChg { get; set; }
        public Int64 lastActivityVol { get; set; }
        public float intRate { get; set; }
        public float bidYld { get; set; }
        public float bidNetChg { get; set; }
        public float askYld { get; set; }
        public float bidYldNetChg { get; set; }
        public float yrClsPrc { get; set; }
        public float monthClsPrc { get; set; }
        public float mktCap { get; set; }
        public float opn { get; set; }
        public float yld { get; set; }
        public UInt16 prcFmtCode { get; set; }
        public Int64 companyShrsOutstanding { get; set; }
        public float yldNetChg { get; set; }

        public float value { get; set; }
        public float referenceValue { get; set; }
        public float change { get; set; }
        public float percentChange { get; set; }

        public Int32 isiErrCode { get; set; }
        public string errMsg { get; set; }
        public string issuerName { get; set; }

        public DateTime updated { get; set; }
        public bool updateFlag { get; set; }
        public bool attachedChart { get; set; }


    }
    
    public class SymbolUpdateEventArgs : EventArgs
    {
        public symbolData symbolUpdate { get; set; }
    }
    public class ChartLiveUpdateEventArgs : EventArgs
    {
        public symbolData symbolUpdate { get; set; }
    }

    public class ChartUpdateEventArgs : EventArgs
    {
        public Chart_Info chartInfo { get; set; }
        public Chart_Data chartData { get; set; }
    }

    public class ChartClosedEventArgs : EventArgs
    {
        public string symbol1 { get; set; }
        public string symbol2 { get; set; }
    }
    public class SymbolUpdateClosedEventArgs : EventArgs
    {
        public string symbol1 { get; set; }
    }


    public struct  Chart_DP
    {
        public string d { get; set; }
        public string md { get; set; }
        public string wd { get; set; }
        public string tlab { get; set; }
        public string p1 { get; set; }
        public string p2 { get; set; }
        public string p3 { get; set; }
        public string p4 { get; set; }
        public string p5 { get; set; }
        public string close { get; set; }
        public DateTime timestamp { get; set; }

    }
    public struct Chart_Info
    {
        public int chartSelection { get; set; }
        public int chartType { get; set; }
        public string chartDuration { get; set; }
        public string chartDesc { get; set; }
        public int interval { get; set; }
        public string period { get; set; }
        public int numSymbols { get; set; }
        public int numQueries { get; set; }
        public int numCharts { get; set; }
        public string symbol1 { get; set; }
        public string symbol2 { get; set; }
        public int seqId1 { get; set; }
        public int seqId2 { get; set; }
        public string StrtTimestamp1 { get; set; }
        public string EndTimestamp1 { get; set; }
        public string StrtTimestamp2 { get; set; }
        public string EndTimestamp2 { get; set; }
        public string QueryStr1 { get; set; }
        public string QueryStr2 { get; set; }


    }

    public class Chart_Data
    {
        public string symbol { get; set; }
        public Chart_Info chartInfo { get; set; }
        public float trdPrc { get; set; }
        public float netChg { get; set; }
        public float pcntChg { get; set; }
        public bool tickFlag { get; set; }
        public string exchange { get; set; }
        public string currency { get; set; }
        public string session { get; set; }
        public string variant { get; set; }
        public string securityType { get; set; }
        public string instrument { get; set; }
        public string frequency { get; set; }
        public int numDP { get; set; }
        public int interval { get; set; }
        public DateTime fromDateTime { get; set; }
        public DateTime toDateTime { get; set; }
        public string datasetType { get; set; }
        public int prcFormatCode { get; set; }
        public string fidNamep1 { get; set; }
        public string fidNamep2 { get; set; }
        public string fidNamep3 { get; set; }
        public string fidNamep4 { get; set; }
        public string fidNamep5 { get; set; }
        public string labelp1 { get; set; }
        public string labelp2 { get; set; }
        public string labelp3 { get; set; }
        public string labelp4 { get; set; }
        public string labelp5 { get; set; }
        public float dataHi { get; set; }
        public DateTime dateHi { get; set; }
        public float dataLo { get; set; }
        public DateTime dateLo { get; set; }

        public List<Chart_DP> dataPts = new List<Chart_DP>(); 
        
    }
    public struct PreProXML
    {
        public XMLTypes xmlCode { get; set; }
        //public string[] hdrStrings { get; set; }
        public Dictionary<string,string> hdrVals { get; set; }
        public string xmlStr { get; set; }
    }

    public class pageData
    {
        public string headerInfo { get; set; }
        public string title { get; set; }
        public List<string> colNames = new List<string>();
        public List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();

    }
}

