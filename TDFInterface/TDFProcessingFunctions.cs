using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Globalization;
using log4net.Appender;


namespace TDFInterface
{
    public delegate void SendBuf(byte[] outbuf);

    public class TDFProcessingFunctions
    {
        
        public static string XMLStr = "";
        public static XmlDocument xmlResponse = new XmlDocument();
        public static Chart_Data ch = new Chart_Data();
        public SendBuf sendBuf;

        public static itf_Header stdHeadr = new itf_Header()
        {
            sync = TDFconstants.SYNC,
            msgType = TDFconstants.LOGON_REQUEST,
            protId = TDFconstants.PROT_ID,
            seqId = 0,
            sessionId = 0xffff,
            msgSize = 0,
            dataOffset = TDFconstants.DATA_OFFSET
        };


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


        #region Processing functions

        // Pre Process XML Data 
        public PreProXML GetXmlType(itf_Parser_Return_Message ChartData)
        {

            PreProXML xmlData = new PreProXML();
            string XMLRaw = System.Text.Encoding.Default.GetString(ChartData.Message.ToArray());
            string CRLF2 = "\r\n\r\n";
            int i = XMLRaw.IndexOf(CRLF2);
            string hdr = XMLRaw.Substring(0, i + 5);
            string XMLgzip = XMLRaw.Substring(i + 5);
            ChartData.Message.RemoveRange(0, i + 4);

            // parse the header info
            string[] strSeparator = new string[] { "\r\n" };
            string[] hdrStrings;
            string[] vals;

            // this takes the header and splits it into key-value pairs
            hdrStrings = hdr.Split(strSeparator, StringSplitOptions.None);

            Dictionary<string, string> hdrVals = new Dictionary<string, string>();
            string[] valSeparator = new string[] { ":" };

            foreach (string hdrS in hdrStrings)
            {
                //separte into key and value and add to dictionary
                vals = hdrS.Split(valSeparator, StringSplitOptions.RemoveEmptyEntries);
                if (vals.Length > 1)
                    hdrVals.Add(vals[0], vals[1]);

            }

            xmlData.hdrVals = hdrVals;
            string encode;
            hdrVals.TryGetValue("CONTENT-ENCODING", out encode);

            // check if compressed
            if (encode.Trim() == "GZIP")
            {
                // compressed using gzip compression
                MemoryStream compressed = new MemoryStream(ChartData.Message.ToArray());
                MemoryStream decompressedXML = Decompress(compressed);
                XMLStr = Encoding.ASCII.GetString(decompressedXML.ToArray());

                decompressedXML.Position = 0;
                xmlResponse.Load(decompressedXML);

            }
            else
            {
                // not compressed
                MemoryStream decompressedXML = new MemoryStream(ChartData.Message.ToArray());
                XMLStr = Encoding.ASCII.GetString(decompressedXML.ToArray());

                decompressedXML.Position = 0;
                xmlResponse.Load(decompressedXML);
            }

            string XmlResponseType = "";
            XMLTypes XmlCode = XMLTypes.Unknown;
            CultureInfo provider = CultureInfo.InvariantCulture;

            XmlNode root = xmlResponse.FirstChild;
            XmlNode nextNode = root.NextSibling;
            XmlResponseType = nextNode.Name;

            if (XmlResponseType == "XMLChartResponse")
            {
                XmlCode = XMLTypes.XMLCharts;
            }
            else if (XmlResponseType == "marketPages")
            {
                XmlCode = XMLTypes.marketPages;
            }
            else if (XmlResponseType == "bpPages")
            {
                XmlCode = XMLTypes.bpPages;
            }
            else
            {
                XmlCode = XMLTypes.Unknown;
            }

            xmlData.xmlCode = XmlCode;
            xmlData.xmlStr = XMLStr;

            return xmlData;
        }

        // Process Market Pulse Pages
        public pageData ProcessMarketPages(PreProXML marketPage)
        {
            XmlDocument mpData = new XmlDocument();
            pageData marketPageData = new pageData();
            string fieldName;
            string val;
            int rowNum = 0;
            int colNum = 0;
            mpData.LoadXml(marketPage.xmlStr);
            XmlNodeList def = mpData.GetElementsByTagName("definition");

            foreach (XmlNode hdr in def)
            {
                //fieldName = "code";
                val = hdr.ChildNodes[0].InnerText;
                //cols.Add(fieldName, val);
                string s = val;

                //fieldName = "exch";
                val = hdr.ChildNodes[1].InnerText;
                //cols.Add(fieldName, val);
                s += "," + val;

                //fieldName = "curr";
                val = hdr.ChildNodes[2].InnerText;
                //cols.Add(fieldName, val);
                s += "," + val;

                //fieldName = "sess";
                val = hdr.ChildNodes[3].InnerText;
                //cols.Add(fieldName, val);
                s += "," + val;

                //fieldName = "dataState";
                val = hdr.ChildNodes[4].InnerText;
                s += "," + val;

                //fieldName = "pageType";
                val = hdr.ChildNodes[5].InnerText;
                s += "," + val;

                marketPageData.headerInfo = s;

            }

            XmlNodeList header = mpData.GetElementsByTagName("header");

            foreach (XmlNode hdr in header)
            {
                //fieldName = "title";
                val = hdr.ChildNodes[0].InnerText;
                //cols.Add(fieldName, val);
                string s = val;

                //fieldName = "subTitle";
                val = hdr.ChildNodes[1].InnerText;
                //cols.Add(fieldName, val);
                s += "," + val;

                //fieldName = "dateTime";
                val = hdr.ChildNodes[2].InnerText;
                //cols.Add(fieldName, val);
                s += "," + val;

                marketPageData.title = s;

            }


            XmlNodeList nodelistR = mpData.GetElementsByTagName("row");
            foreach (XmlNode row in nodelistR)
            {
                colNum = 0;
                Dictionary<string, string> cols = new Dictionary<string, string>();

                XmlNodeList nodelistC = row.ChildNodes;
                foreach (XmlNode col in nodelistC)
                {
                    if (rowNum == 0)
                    {
                        val = col.InnerText;
                        marketPageData.colNames.Add(val);
                    }
                    else
                    {
                        if (colNum == 0)
                        {
                            fieldName = "symbol";
                            val = col.ChildNodes[0].InnerText;
                            //cols.Add(fieldName, val);
                            string s = val;

                            //fieldName = "exch";
                            val = col.ChildNodes[1].InnerText;
                            //cols.Add(fieldName, val);
                            s += "," + val;

                            //fieldName = "curr";
                            val = col.ChildNodes[2].InnerText;
                            //cols.Add(fieldName, val);
                            s += "," + val;

                            //fieldName = "sess";
                            val = col.ChildNodes[3].InnerText;
                            //cols.Add(fieldName, val);
                            s += "," + val;

                            //fieldName = "dataState";
                            val = col.ChildNodes[4].InnerText;
                            s += "," + val;


                            cols.Add(fieldName, s);

                        }
                        else
                        {
                            val = col.ChildNodes[0].InnerText;
                            fieldName = col.ChildNodes[0].Name;
                            cols.Add(fieldName, val);
                        }

                    }
                    colNum++;
                }
                if (rowNum > 0)
                    marketPageData.rows.Add(cols);
                rowNum++;


            }
            return marketPageData;
        }

        // Process Business Pulse Pages
        public pageData ProcessBusinessPages(PreProXML marketPage)
        {
            XmlDocument mpData = new XmlDocument();
            pageData marketPageData = new pageData();
            string fieldName;
            string val;
            int rowNum = 0;
            int colNum = 0;
            mpData.LoadXml(marketPage.xmlStr);
            XmlNodeList def = mpData.GetElementsByTagName("definition");

            foreach (XmlNode hdr in def)
            {
                //fieldName = "code";
                val = hdr.ChildNodes[0].InnerText;
                //cols.Add(fieldName, val);
                string s = val;

                //fieldName = "exch";
                val = hdr.ChildNodes[1].InnerText;
                //cols.Add(fieldName, val);
                s += "," + val;

                //fieldName = "curr";
                val = hdr.ChildNodes[2].InnerText;
                //cols.Add(fieldName, val);
                s += "," + val;

                //fieldName = "sess";
                val = hdr.ChildNodes[3].InnerText;
                //cols.Add(fieldName, val);
                s += "," + val;

                //fieldName = "dataState";
                val = hdr.ChildNodes[4].InnerText;
                s += "," + val;

                //fieldName = "pageType";
                val = hdr.ChildNodes[5].InnerText;
                s += "," + val;

                marketPageData.headerInfo = s;

            }

            XmlNodeList header = mpData.GetElementsByTagName("header");

            foreach (XmlNode hdr in header)
            {
                //fieldName = "title";
                val = hdr.ChildNodes[0].InnerText;
                //cols.Add(fieldName, val);
                string s = val;

                //fieldName = "subTitle";
                val = hdr.ChildNodes[1].InnerText;
                //cols.Add(fieldName, val);
                s += "," + val;

                //fieldName = "dateTime";
                val = hdr.ChildNodes[2].InnerText;
                //cols.Add(fieldName, val);
                s += "," + val;

                marketPageData.title = s;

            }


            XmlNodeList nodelistR = mpData.GetElementsByTagName("row");
            foreach (XmlNode row in nodelistR)
            {
                colNum = 0;
                Dictionary<string, string> cols = new Dictionary<string, string>();

                XmlNodeList nodelistC = row.ChildNodes;
                foreach (XmlNode col in nodelistC)
                {
                    if (rowNum == 0)
                    {
                        val = col.InnerText;
                        marketPageData.colNames.Add(val);
                    }
                    else
                    {
                        if (colNum == 0)
                        {
                            //fieldName = "symbol";
                            fieldName = col.ChildNodes[0].Name;
                            val = col.ChildNodes[0].InnerText;
                            //cols.Add(fieldName, val);
                            string s = val;

                            /*
                            //fieldName = "exch";
                            val = col.ChildNodes[1].InnerText;
                            //cols.Add(fieldName, val);
                            s += "," + val;

                            //fieldName = "curr";
                            val = col.ChildNodes[2].InnerText;
                            //cols.Add(fieldName, val);
                            s += "," + val;

                            //fieldName = "sess";
                            val = col.ChildNodes[3].InnerText;
                            //cols.Add(fieldName, val);
                            s += "," + val;

                            //fieldName = "dataState";
                            val = col.ChildNodes[4].InnerText;
                            s += "," + val;
                            */

                            cols.Add(fieldName, s);

                        }
                        else
                        {
                            val = col.ChildNodes[0].InnerText;
                            fieldName = col.ChildNodes[0].Name;
                            cols.Add(fieldName, val);
                        }

                    }
                    colNum++;
                }
                if (rowNum > 0)
                    marketPageData.rows.Add(cols);
                rowNum++;


            }
            return marketPageData;
        }


        public Chart_Data ProcessXMLChartData(PreProXML ChartData)
        {
            //string XMLRaw = System.Text.Encoding.Default.GetString(ChartData.Message.ToArray());
            string XMLRaw = ChartData.xmlStr;
            string CRLF2 = "\r\n\r\n";
            int i = XMLRaw.IndexOf(CRLF2);


            xmlResponse.LoadXml(ChartData.xmlStr);

            Chart_Data Chart1 = new Chart_Data();
            Chart1.dataHi = 0;
            Chart1.dataLo = 9999999;

            string y1 = "";
            string m1 = "";
            string d1 = "";
            string eName = "";
            string fieldName = "";
            string fieldLabel = "";
            string XmlResponseType = "";
            CultureInfo provider = CultureInfo.InvariantCulture;

            XmlNode root = xmlResponse.FirstChild;
            XmlNode nextNode = root.NextSibling;
            XmlResponseType = nextNode.Name;


            XmlNodeList nodelistS = xmlResponse.GetElementsByTagName("Security");
            // Get symbol info
            foreach (XmlNode ts in nodelistS)
            {
                if (ts?.Attributes["symbol"] != null)
                    Chart1.symbol = ts.Attributes.GetNamedItem("symbol").Value ?? "n/a";
                if (ts?.Attributes["exchange"] != null)
                    Chart1.exchange = ts.Attributes.GetNamedItem("exchange").Value ?? "n/a";
                if (ts?.Attributes["currency"] != null)
                    Chart1.currency = ts.Attributes.GetNamedItem("currency").Value ?? "n/a";
                if (ts?.Attributes["session"] != null)
                    Chart1.session = ts.Attributes.GetNamedItem("session").Value ?? "n/a";
                if (ts?.Attributes["variant"] != null)
                    Chart1.variant = ts.Attributes.GetNamedItem("variant").Value ?? "n/a";
                if (ts?.Attributes["securityType"] != null)
                    Chart1.securityType = ts.Attributes.GetNamedItem("securityType").Value ?? "n/a";
                if (ts?.Attributes["instrument"] != null)
                    Chart1.instrument = ts.Attributes.GetNamedItem("instrument").Value ?? "n/a";
            }

            XmlNodeList nodelistP = xmlResponse.GetElementsByTagName("Price");
            foreach (XmlNode ts in nodelistP)
            {
                if (ts?.Attributes["set"] != null)
                    Chart1.datasetType = ts.Attributes.GetNamedItem("set").Value;
                if (ts?.Attributes["priceFormatCode"] != null)
                    Chart1.prcFormatCode = Int32.Parse(ts.Attributes.GetNamedItem("priceFormatCode").Value);
            }

            XmlNodeList nodelistTS = xmlResponse.GetElementsByTagName("TimeSeries");
            foreach (XmlNode ts in nodelistTS)
            {
                if (ts?.Attributes["frequency"] != null)
                    Chart1.frequency = ts.Attributes.GetNamedItem("frequency").Value;
                if (ts?.Attributes["interval"] != null)
                    Chart1.interval = Int32.Parse(ts.Attributes.GetNamedItem("interval").Value);
                if (ts?.Attributes["dataPoints"] != null)
                    Chart1.numDP = Int32.Parse(ts.Attributes.GetNamedItem("dataPoints").Value);
                if (ts?.Attributes["fromDateTime"] != null)
                    Chart1.fromDateTime = DateTime.ParseExact(ts.Attributes.GetNamedItem("fromDateTime").Value, "yyyyMMdd HH:mm:ss", provider);
                if (ts?.Attributes["toDateTime"] != null)
                    Chart1.toDateTime = DateTime.ParseExact(ts.Attributes.GetNamedItem("toDateTime").Value, "yyyyMMdd HH:mm:ss", provider);
            }

            XmlNodeList nodelistE = xmlResponse.GetElementsByTagName("Element");
            int pn = 0;
            foreach (XmlNode ele in nodelistE)
            {
                eName = ele.Attributes.GetNamedItem("name").Value;
                fieldName = ele.Attributes.GetNamedItem("fidName").Value;
                fieldLabel = ele.Attributes.GetNamedItem("label").Value;
                pn++;
                switch (pn)
                {
                    case 1:
                        Chart1.fidNamep1 = fieldName;
                        Chart1.labelp1 = fieldLabel;
                        break;
                    case 2:
                        Chart1.fidNamep2 = fieldName;
                        Chart1.labelp2 = fieldLabel;
                        break;
                    case 3:
                        Chart1.fidNamep3 = fieldName;
                        Chart1.labelp3 = fieldLabel;
                        break;
                    case 4:
                        Chart1.fidNamep4 = fieldName;
                        Chart1.labelp4 = fieldLabel;
                        break;
                    case 5:
                        Chart1.fidNamep5 = fieldName;
                        Chart1.labelp5 = fieldLabel;
                        break;
                }
            }

            if (Chart1.frequency == "IntraDay")
            {
                // Daily
                string h = null;
                string m = null;
                int nHol = 0;

                XmlNodeList nl = xmlResponse.GetElementsByTagName("YI");
                foreach (XmlNode n in nl)
                {
                    y1 = n.Attributes.GetNamedItem("ymd").Value;
                    XmlNodeList nDM = n.ChildNodes;

                    foreach (XmlNode nm in nDM)
                    {
                        h = nm.Attributes.GetNamedItem("h").Value;
                        XmlNodeList nDChild = nm.ChildNodes;

                        foreach (XmlNode d in nDChild)
                        {
                            Chart_DP cdp = new Chart_DP();
                            m = d.Attributes.GetNamedItem("m").Value;
                            cdp.tlab = m;
                            try
                            {
                                cdp.p1 = d.Attributes.GetNamedItem("p1").Value;
                                cdp.p2 = d.Attributes.GetNamedItem("p2").Value;
                                cdp.p3 = d.Attributes.GetNamedItem("p3").Value;
                                cdp.p4 = d.Attributes.GetNamedItem("p4").Value;
                                cdp.p5 = d.Attributes.GetNamedItem("p5").Value;
                                cdp.close = cdp.p4;

                                string dt = y1.Substring(4, 2) + "/" + y1.Substring(6, 2) + "/" + y1.Substring(0, 4) + " " + h + ":" + m + ":00";

                                cdp.timestamp = DateTime.Parse(dt);
                                Chart1.dataPts.Add(cdp);
                                if (float.Parse(cdp.p2) > Chart1.dataHi)
                                {
                                    Chart1.dataHi = float.Parse(cdp.p2);
                                    Chart1.dateHi = cdp.timestamp;
                                }
                                if (float.Parse(cdp.p3) < Chart1.dataLo)
                                {
                                    Chart1.dataLo = float.Parse(cdp.p3);
                                    Chart1.dateLo = cdp.timestamp;
                                }

                            }
                            catch
                            {
                                nHol++;
                            }

                        }
                    }
                }
                int ndp = Chart1.dataPts.Count;
                int nTot = ndp + nHol;
            }
            if (Chart1.frequency == "Daily")
            {
                // Daily
                XmlNodeList nl = xmlResponse.GetElementsByTagName("YD");
                int nHol = 0;
                foreach (XmlNode n in nl)
                {
                    y1 = n.Attributes.GetNamedItem("y").Value;
                    //XmlNodeList nDM = xmlResponse.GetElementsByTagName("DM");
                    XmlNodeList nDM = n.ChildNodes;

                    foreach (XmlNode nm in nDM)
                    {
                        m1 = nm.Attributes.GetNamedItem("m").Value;
                        XmlNodeList nDChild = nm.ChildNodes;

                        foreach (XmlNode d in nDChild)
                        {
                            Chart_DP cdp = new Chart_DP();
                            cdp.d = d.Attributes.GetNamedItem("d").Value;
                            cdp.tlab = d.Attributes.GetNamedItem("d").Value;

                            //if (d.Attributes.GetNamedItem("day").Value == null)
                            try
                            {
                                cdp.p1 = d.Attributes.GetNamedItem("p1").Value;
                                cdp.p2 = d.Attributes.GetNamedItem("p2").Value;
                                cdp.p3 = d.Attributes.GetNamedItem("p3").Value;
                                cdp.p4 = d.Attributes.GetNamedItem("p4").Value;
                                cdp.p5 = d.Attributes.GetNamedItem("p5").Value;
                                cdp.close = cdp.p5;

                                string dt = m1 + "/" + cdp.d + "/" + y1 + " 16:00:00";
                                cdp.timestamp = DateTime.Parse(dt);
                                Chart1.dataPts.Add(cdp);
                                if (float.Parse(cdp.p3) > Chart1.dataHi)
                                {
                                    Chart1.dataHi = float.Parse(cdp.p3);
                                    Chart1.dateHi = cdp.timestamp;
                                }
                                if (float.Parse(cdp.p4) < Chart1.dataLo)
                                {
                                    Chart1.dataLo = float.Parse(cdp.p4);
                                    Chart1.dateLo = cdp.timestamp;
                                }
                            }
                            catch
                            {
                                nHol++;
                            }

                        }
                    }
                }
                int ndp = Chart1.dataPts.Count;
                int nTot = ndp + nHol;
            }
            if (Chart1.frequency == "Weekly")
            {
                // Weekly
                XmlNodeList nl = xmlResponse.GetElementsByTagName("YW");
                int nHol = 0;
                foreach (XmlNode n in nl)
                {
                    y1 = n.Attributes.GetNamedItem("y").Value;
                    //XmlNodeList nDM = xmlResponse.GetElementsByTagName("DM");
                    XmlNodeList nDM = n.ChildNodes;

                    foreach (XmlNode nm in nDM)
                    {
                        //m1 = nm.Attributes.GetNamedItem("m").Value;
                        Chart_DP cdp = new Chart_DP();
                        cdp.wd = nm.Attributes.GetNamedItem("wd").Value;
                        cdp.tlab = nm.Attributes.GetNamedItem("wd").Value;
                        m1 = cdp.wd.Substring(0, 2);
                        d1 = cdp.wd.Substring(2, 2);

                        //if (d.Attributes.GetNamedItem("day").Value == null)
                        try
                        {
                            cdp.p1 = nm.Attributes.GetNamedItem("p1").Value;
                            cdp.p2 = nm.Attributes.GetNamedItem("p2").Value;
                            cdp.p3 = nm.Attributes.GetNamedItem("p3").Value;
                            cdp.p4 = nm.Attributes.GetNamedItem("p4").Value;
                            cdp.p5 = nm.Attributes.GetNamedItem("p5").Value;
                            cdp.close = cdp.p5;

                            string dt = m1 + "/" + d1 + "/" + y1 + " 16:00:00";
                            cdp.timestamp = DateTime.Parse(dt);
                            Chart1.dataPts.Add(cdp);
                            if (float.Parse(cdp.p3) > Chart1.dataHi)
                            {
                                Chart1.dataHi = float.Parse(cdp.p3);
                                Chart1.dateHi = cdp.timestamp;
                            }
                            if (float.Parse(cdp.p4) < Chart1.dataLo)
                            {
                                Chart1.dataLo = float.Parse(cdp.p4);
                                Chart1.dateLo = cdp.timestamp;
                            }

                        }
                        catch
                        {
                            nHol++;
                        }


                    }
                }
                int ndp = Chart1.dataPts.Count;
                int nTot = ndp + nHol;
            }

            else if (Chart1.frequency == "Monthly")
            {
                // Monthly
                XmlNodeList nl = xmlResponse.GetElementsByTagName("YM");
                int nHol = 0;
                foreach (XmlNode n in nl)
                {
                    y1 = n.Attributes.GetNamedItem("y").Value;
                    XmlNodeList nDM = n.ChildNodes;

                    foreach (XmlNode nm in nDM)
                    {
                        //m1 = nm.Attributes.GetNamedItem("md").Value;

                        Chart_DP cdp = new Chart_DP();
                        cdp.md = nm.Attributes.GetNamedItem("md").Value;
                        cdp.tlab = nm.Attributes.GetNamedItem("md").Value;
                        m1 = cdp.md.Substring(0, 2);
                        d1 = cdp.md.Substring(2, 2);


                        //if (d.Attributes.GetNamedItem("day").Value == null)
                        try
                        {
                            cdp.p1 = nm.Attributes.GetNamedItem("p1").Value;
                            cdp.p2 = nm.Attributes.GetNamedItem("p2").Value;
                            cdp.p3 = nm.Attributes.GetNamedItem("p3").Value;
                            cdp.p4 = nm.Attributes.GetNamedItem("p4").Value;
                            cdp.p5 = nm.Attributes.GetNamedItem("p5").Value;
                            cdp.close = cdp.p5;

                            string dt = m1 + "/" + d1 + "/" + y1 + " 16:00:00";
                            cdp.timestamp = DateTime.Parse(dt);
                            Chart1.dataPts.Add(cdp);
                            if (float.Parse(cdp.p3) > Chart1.dataHi)
                            {
                                Chart1.dataHi = float.Parse(cdp.p3);
                                Chart1.dateHi = cdp.timestamp;
                            }
                            if (float.Parse(cdp.p4) < Chart1.dataLo)
                            {
                                Chart1.dataLo = float.Parse(cdp.p4);
                                Chart1.dateLo = cdp.timestamp;
                            }


                        }
                        catch
                        {
                            nHol++;
                        }


                    }
                }
                int ndp = Chart1.dataPts.Count;
                int nTot = ndp + nHol;
            }
            ch = Chart1;
            return Chart1;
        }

        public static MemoryStream Decompress(MemoryStream compressedXML)
        {

            //Decompress                
            var bigStream = new GZipStream(compressedXML, CompressionMode.Decompress);
            var bigStreamOut = new System.IO.MemoryStream();
            bigStream.CopyTo(bigStreamOut);
            return bigStreamOut;

        }

        public List<string> ProcessCataloger(byte[] catalog)
        {
            List<string> s = new List<string>();
            UInt16 endian = BitConverter.ToUInt16(catalog, 0);
            UInt16 numPatterns = ReverseBytes2(BitConverter.ToUInt16(catalog, 2));
            TDFGlobals.numCat = (short)numPatterns;
            UInt16 patternNum = 0;
            UInt16 numFIDS = 0;
            int index = 4;
            Int32 FID = 0;
            s.Add("Num Patterns: " + numPatterns.ToString());

            for (int i = 0; i < numPatterns; i++)
            {
                patternNum = ReverseBytes2(BitConverter.ToUInt16(catalog, index));
                s.Add("Pattern Num: " + patternNum.ToString());
                index = index + 2;
                numFIDS = ReverseBytes2(BitConverter.ToUInt16(catalog, index));
                index = index + 2;
                s.Add("Num FIDS: " + numFIDS.ToString());

                for (int j = 0; j < numFIDS; j++)
                {
                    FID = BitConverter.ToInt32(catalog, index);
                    TDFGlobals.CatalogData[i, j] = FID;
                    index = index + 4;
                    s.Add(j.ToString() + "  " + FID.ToString());
                }
            }
            return s;
        }
        
        public void ProcessFieldInfoTable(itf_Parser_Return_Message TRmess)
        {
            local_Cataloger lCat = new local_Cataloger();
            field_Info fir = new field_Info();
            lCat = GetLocalCatalog(TRmess.Message.ToArray());
            int numRecords = TRmess.data_Header.numRecSize;
            byte[] numBytes = new byte[6];
            UInt32[] FIDS = new UInt32[6];
            string[] fieldNames = new string[6];
            byte[] rData = new byte[TRmess.Message.Count];
            rData = TRmess.Message.ToArray();
            UInt32 FID = 0;
            int indx = 0;
            ushort len = 0;
            ushort dataLen = 0;
            string s = "";
            UInt32 fidIndx = 0;
            int starIndx = 0;

            for (int j = 0; j < lCat.numFIDS; j++)
            {
                FID = lCat.FIDs[j];
                switch (FID)
                {
                    case 918584:
                        numBytes[j] = 4;
                        FIDS[j] = FID;
                        fieldNames[j] = "fieldId";
                        break;
                    case 132047:
                        numBytes[j] = 255;
                        FIDS[j] = FID;
                        fieldNames[j] = "fieldName";
                        break;
                    case 264554:
                        numBytes[j] = 1;
                        FIDS[j] = FID;
                        fieldNames[j] = "fieldFmtId";
                        break;
                    case 263225:
                        numBytes[j] = 1;
                        FIDS[j] = FID;
                        fieldNames[j] = "fieldType";
                        break;
                    case 133483:
                        numBytes[j] = 255;
                        FIDS[j] = FID;
                        fieldNames[j] = "fieldFmtOptional";
                        break;
                    case 133484:
                        numBytes[j] = 255;
                        FIDS[j] = FID;
                        fieldNames[j] = "fieldDesc";
                        break;
                }
            }

            indx = lCat.rec_Size;

            for (int i = 0; i < numRecords - 1; i++)
            {
                dataLen = ReverseBytes2(BitConverter.ToUInt16(rData, indx));
                indx += 8; // 2 byte for dataLen + 6 bytes for unknown stuff

                for (int j = 0; j < lCat.numFIDS; j++)
                {

                    if (numBytes[j] == 255)
                    {
                        len = rData[indx];
                        indx += 1;
                        byte[] temp = new byte[len];
                        Array.Copy(rData, indx, temp, 0, len);
                        s = System.Text.Encoding.Default.GetString(temp);
                        indx += len;

                        switch (FIDS[j])
                        {
                            case 132047:
                                fir.fieldName = s;

                                if (TDFGlobals.starredFields.Contains(s))
                                {
                                    starIndx = TDFGlobals.starredFields.IndexOf(s);
                                    fir.dataIndx = starIndx;
                                    fir.show = true;
                                    //starIndx++;
                                    //starredFields.Remove(s);
                                }
                                else
                                {
                                    fir.show = false;
                                    fir.dataIndx = -1;
                                }

                                break;
                            case 133483:
                                fir.fieldFmtOptional = s;
                                break;
                            case 133484:
                                fir.fieldDesc = s;
                                break;
                        }
                    }
                    else
                    {
                        switch (FIDS[j])
                        {
                            case 918584:
                                fir.fieldId = BitConverter.ToUInt32(rData, indx);
                                indx += 4;
                                break;
                            case 264554:
                                fir.fieldfFmtId = rData[indx];
                                indx += 1;
                                break;
                            case 263225:
                                fir.fieldType = rData[indx];
                                indx += 1;
                                break;

                        }
                    }
                }
                fidIndx = fir.fieldId & 0x0000ffff;
                fir.businessId = (ushort)fidIndx;
                fir.fieldDataType = (byte)((fir.fieldId / 0x10000) & 0x00ff);

                switch (fir.fieldDataType)
                {
                    // long  
                    case 1:
                        fir.numBytes = 4;
                        break;
                    // string 
                    case 2:
                        fir.numBytes = 99;
                        break;
                    // boolean byte 0 or 1
                    case 3:
                        fir.numBytes = 1;
                        break;
                    // byte
                    case 4:
                        fir.numBytes = 1;
                        break;
                    // float
                    case 5:
                        fir.numBytes = 4;
                        break;
                    // double
                    case 6:
                        fir.numBytes = 8;
                        break;
                    // Date "MM/DD/YYYY"
                    case 9:
                        fir.numBytes = 4;
                        break;
                    // Time "HH:MM:SS"
                    case 10:
                        fir.numBytes = 4;
                        break;
                    // huge
                    case 13:
                        fir.numBytes = 8;
                        break;
                    // ulong  
                    case 14:
                        fir.numBytes = 4;
                        break;
                }
                TDFGlobals.field_Info_Table[fidIndx] = fir;
            }
        }


        public static int GetSymbolIndx(string sym)
        {
            try
            {
                string s;
                int indx = -1;
                for (int i = 0; i < TDFGlobals.symbols.Count; i++)
                {
                    if (TDFGlobals.symbols[i].symbol == sym)
                        indx = i;
                }


                if (indx < 0)
                    s = $"sym: {sym}  Count: {TDFGlobals.symbols.Count}";

                return indx;
            }
            catch (Exception ex)
            {
                log.Error($"GetSymbolIndx error - {ex}");
                return -1;

            }

        }
        
        public static void SetSymbolData(List<fin_Data> f, int i, int symIndx)
        {
            try
            {
                if (symIndx >= 0)
                {
                    TDFGlobals.symbols[symIndx].symbolFull = f[i].symbolFull;
                    if (f[i].show)
                    {
                        switch (f[i].dataIndx)
                        {
                            case 0:
                                TDFGlobals.symbols[symIndx].trdPrc = f[i].fData;
                                break;
                            case 1:
                                TDFGlobals.symbols[symIndx].netChg = f[i].fData;
                                break;
                            case 2:
                                TDFGlobals.symbols[symIndx].ycls = f[i].fData;
                                break;
                            case 3:
                                TDFGlobals.symbols[symIndx].pcntChg = f[i].fData;
                                break;
                            case 4:
                                TDFGlobals.symbols[symIndx].hi = f[i].fData;
                                break;
                            case 5:
                                TDFGlobals.symbols[symIndx].lo = f[i].fData;
                                break;
                            case 6:
                                TDFGlobals.symbols[symIndx].opn = f[i].fData;
                                break;
                            case 7:
                                TDFGlobals.symbols[symIndx].cumVol = f[i].iData;
                                break;
                            case 8:
                                TDFGlobals.symbols[symIndx].lastActivity = f[i].fData;
                                break;
                            case 9:
                                TDFGlobals.symbols[symIndx].lastActivityNetChg = f[i].fData;
                                break;
                            case 10:
                                TDFGlobals.symbols[symIndx].lastActivityPcntChg = f[i].fData;
                                break;
                            case 11:
                                TDFGlobals.symbols[symIndx].lastActivityVol = f[i].iData;
                                break;
                            case 12:
                                TDFGlobals.symbols[symIndx].annHi = f[i].fData;
                                break;
                            case 13:
                                TDFGlobals.symbols[symIndx].annLo = f[i].fData;
                                break;
                            case 14:
                                TDFGlobals.symbols[symIndx].isiErrCode = f[i].iData;
                                break;
                            case 15:
                                TDFGlobals.symbols[symIndx].errMsg = f[i].sData;
                                break;
                            case 16:
                                TDFGlobals.symbols[symIndx].issuerName = f[i].sData;
                                break;

                        }


                        /*
                        switch (f[i].dataIndx)
                        {
                            case 0:
                                symbols[symIndx].trdPrc = f[i].fData;
                                break;
                            case 1:
                                symbols[symIndx].netChg = f[i].fData;
                                break;
                            case 2:
                                symbols[symIndx].ycls = f[i].fData;
                                break;
                            case 3:
                                symbols[symIndx].pcntChg = f[i].fData;
                                break;
                            case 4:
                                symbols[symIndx].hi = f[i].fData;
                                break;
                            case 5:
                                symbols[symIndx].lo = f[i].fData;
                                break;
                            case 6:
                                symbols[symIndx].annHi = f[i].fData;
                                break;
                            case 7:
                                symbols[symIndx].annLo = f[i].fData;
                                break;
                            case 8:
                                symbols[symIndx].cumVol = f[i].iData;
                                break;
                            case 9:
                                symbols[symIndx].peRatio = f[i].fData;
                                break;
                            case 10:
                                symbols[symIndx].eps = f[i].fData;
                                break;
                            case 11:
                                symbols[symIndx].ask = f[i].fData;
                                break;
                            case 12:
                                symbols[symIndx].bid = f[i].fData;
                                break;
                            case 13:
                                symbols[symIndx].lastActivity = f[i].fData;
                                break;
                            case 14:
                                symbols[symIndx].lastActivityNetChg = f[i].fData;
                                break;
                            case 15:
                                symbols[symIndx].lastActivityPcntChg = f[i].fData;
                                break;
                            case 16:
                                symbols[symIndx].divAnn = f[i].fData;
                                break;
                            case 17:
                                symbols[symIndx].intRate = f[i].fData;
                                break;
                            case 18:
                                symbols[symIndx].bidYld = f[i].fData;
                                break;
                            case 19:
                                symbols[symIndx].bidNetChg = f[i].fData;
                                break;
                            case 20:
                                symbols[symIndx].askYld = f[i].fData;
                                break;
                            case 21:
                                symbols[symIndx].bidYldNetChg = f[i].fData;
                                break;
                            case 22:
                                symbols[symIndx].yrClsPrc = f[i].fData;
                                break;
                            case 23:
                                symbols[symIndx].monthClsPrc = f[i].fData;
                                break;
                            case 24:
                                symbols[symIndx].mktCap = f[i].fData;
                                break;
                            case 25:
                                symbols[symIndx].opn = f[i].fData;
                                break;
                            case 26:
                                symbols[symIndx].yld = f[i].fData;
                                break;
                            case 27:
                                symbols[symIndx].prcFmtCode = (ushort)f[i].iData;
                                break;
                            case 28:
                                symbols[symIndx].companyShrsOutstanding = f[i].iData;
                                break;
                            case 29:
                                symbols[symIndx].sectyType = f[i].bData;
                                break;
                            case 30:
                                symbols[symIndx].symbol = f[i].sData;
                                break;
                            case 31:
                                symbols[symIndx].yldNetChg = f[i].fData;
                                break;
                            case 32:
                                symbols[symIndx].isiErrCode = f[i].iData;
                                break;
                            case 33:
                                symbols[symIndx].errMsg = f[i].sData;
                                break;
                            case 34:
                            TDFGlobals.symbols[symIndx].issuerName = f[i].sData;
                            break;
                            
                        }
                        */

                        SymbolUpdateEventArgs sduea = new SymbolUpdateEventArgs();
                        sduea.symbolUpdate = TDFGlobals.symbols[symIndx];
                        //OnSymbolDataUpdated(sduea);

                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"SetSymbolData error - {ex}");
                
            }

        }


        public static local_Cataloger GetLocalCatalog(byte[] catalog)
        {
            local_Cataloger lCat = new local_Cataloger();
            lCat.rec_Size = ReverseBytes2(BitConverter.ToUInt16(catalog, 0));
            lCat.header = BitConverter.ToUInt16(catalog, 2);
            lCat.numPatterns = ReverseBytes2(BitConverter.ToUInt16(catalog, 4));
            int index = 6;

            for (int i = 0; i < lCat.numPatterns; i++)
            {
                lCat.patternNumber = ReverseBytes2(BitConverter.ToUInt16(catalog, index));
                index = index + 2;
                lCat.numFIDS = ReverseBytes2(BitConverter.ToUInt16(catalog, index));
                index = index + 2;

                for (int j = 0; j < lCat.numFIDS; j++)
                {
                    //lCat.FIDs.Add(ReverseBytes4(BitConverter.ToUInt32(catalog, index)));
                    lCat.FIDs.Add(BitConverter.ToUInt32(catalog, index));
                    index = index + 4;
                }


            }
            return lCat;
        }


        public void ProcessFinancialData(itf_Parser_Return_Message TRmess)
        {
            byte[] rData = new byte[TRmess.Message.Count];
            rData = TRmess.Message.ToArray();
            fin_Data fin = new fin_Data();
            string s = "";
            UInt32 fidIndx = 0;
            ushort len = 0;

            int numRecords = TRmess.data_Header.numRecSize;
            int indx = 0;

            for (int j = 0; j < numRecords; j++)
            {
                ushort dataLen = ReverseBytes2(BitConverter.ToUInt16(rData, indx));
                indx += 2;
                ushort OPenFIDHdr = ReverseBytes2(BitConverter.ToUInt16(rData, indx));
                indx += 2;

                // Decode Header
                Open_FID_Hdr h = new Open_FID_Hdr();
                h = Decode_OpenFID_Header(OPenFIDHdr);

                if (h.symBol == true)
                {
                    // get Symbol
                    len = rData[indx];
                    indx += 1;
                    byte[] temp = new byte[len];
                    Array.Copy(rData, indx, temp, 0, len);
                    s = System.Text.Encoding.Default.GetString(temp);
                    fin.symbolFull = s;
                    int pos = s.IndexOf("-");
                    if (pos > 0)
                        fin.symbol = s.Substring(0, pos);
                    else
                        fin.symbol = s;

                    indx += len;

                    if (fin.symbol == null || fin.symbol == string.Empty)
                    {
                        fin.symbol = s;
                    }

                }
                else
                {
                    fin.symbol = "none";
                }


                ushort numFIDS = ReverseBytes2(BitConverter.ToUInt16(rData, indx));
                indx += 2;

                for (int i = 0; i < numFIDS; i++)
                {

                    fin.fieldId = BitConverter.ToUInt32(rData, indx);
                    fidIndx = fin.fieldId & 0x0000ffff;
                    fin.fieldfFmtId = TDFGlobals.field_Info_Table[fidIndx].fieldfFmtId;
                    fin.fieldFmtOptional = TDFGlobals.field_Info_Table[fidIndx].fieldFmtOptional;
                    fin.fieldName = TDFGlobals.field_Info_Table[fidIndx].fieldName;
                    fin.businessId = TDFGlobals.field_Info_Table[fidIndx].businessId;
                    fin.fieldDataType = TDFGlobals.field_Info_Table[fidIndx].fieldDataType;
                    fin.show = TDFGlobals.field_Info_Table[fidIndx].show;
                    fin.dataIndx = TDFGlobals.field_Info_Table[fidIndx].dataIndx;
                    indx += 4;

                    ConvertBytes(ref fin, ref indx, rData);
                    TDFGlobals.financialResults.Add(fin);
                }
            }
        }

        // Process data from dynamic subscriptions
        public void ProcessFinancialUpdateData(itf_Parser_Update_Message TRmess)
        {
            try
            {
                byte[] rData = new byte[TRmess.Message.Count];
                rData = TRmess.Message.ToArray();
                fin_Data fin = new fin_Data();
                string s = "";
                UInt32 fidIndx = 0;
                bool inCataloggedPart = false;

                int indx = 0;
                ushort numRecords = ReverseBytes2(BitConverter.ToUInt16(rData, indx));
                indx += 2;

                for (int j = 0; j < numRecords; j++)
                {
                    if (j > 0)
                    {
                        fin.resultIndx = 255;
                    }

                    ushort dataLen = ReverseBytes2(BitConverter.ToUInt16(rData, indx));
                    indx += 2;
                    ushort OPenFIDHdr = ReverseBytes2(BitConverter.ToUInt16(rData, indx));
                    indx += 2;

                    // Decode Header
                    Open_FID_Hdr h = new Open_FID_Hdr();
                    h = Decode_OpenFID_Header(OPenFIDHdr);

                    ushort len;

                    // get Symbol
                    if (h.symBol == true)
                    {
                        len = rData[indx];
                        indx += 1;
                        byte[] temp = new byte[len];
                        Array.Copy(rData, indx, temp, 0, len);
                        s = System.Text.Encoding.Default.GetString(temp);
                        fin.symbolFull = s;
                        int pos = s.IndexOf("-");
                        if (pos > 0)
                            fin.symbol = s.Substring(0, pos);
                        indx += len;

                        if (fin.symbol == null || fin.symbol == string.Empty)
                        {
                            fin.symbol = s;
                        }
                    }
                    else
                    {
                        fin.symbol = "none";
                    }

                    ushort patternNum = 0;
                    ushort numFIDS = ReverseBytes2(BitConverter.ToUInt16(rData, indx));
                    indx += 2;
                    ushort num = 0;


                    if (h.msgStruct <= 2)
                    {
                        patternNum = ReverseBytes2(BitConverter.ToUInt16(rData, indx));
                        indx += 2;
                        if (patternNum > 130)
                        {
                            num = 0;
                        }
                        else
                            num = GetCatalogNumFIDS(patternNum);

                        numFIDS += num;
                        numFIDS -= 1;
                    }

                    for (int i = 0; i < numFIDS; i++)
                    {
                        if (i > num - 1)
                        {
                            patternNum = 0;
                            inCataloggedPart = false;
                        }

                        if (patternNum > 0)
                        {
                            fin.fieldId = (UInt32)TDFGlobals.CatalogData[patternNum - 1, i];
                            inCataloggedPart = true;
                        }
                        else
                        {
                            fin.fieldId = BitConverter.ToUInt32(rData, indx);
                            inCataloggedPart = false;
                            indx += 4;
                        }

                        // Directives only apply to non-catalogged part
                        if (h.directives == true && inCataloggedPart == false)
                        {
                            fin.operation = rData[indx];
                            indx += 1;
                        }
                        else
                        {
                            fin.operation = 1;
                        }



                        fidIndx = fin.fieldId & 0x0000ffff;
                        fin.fieldfFmtId = TDFGlobals.field_Info_Table[fidIndx].fieldfFmtId;
                        fin.fieldFmtOptional = TDFGlobals.field_Info_Table[fidIndx].fieldFmtOptional;
                        fin.fieldName = TDFGlobals.field_Info_Table[fidIndx].fieldName;
                        fin.businessId = (ushort)fidIndx;
                        fin.fieldDataType = (byte)((fin.fieldId / 0x10000) & 0xff);
                        fin.numBytes = TDFGlobals.field_Info_Table[fidIndx].numBytes;
                        fin.show = TDFGlobals.field_Info_Table[fidIndx].show;
                        fin.dataIndx = TDFGlobals.field_Info_Table[fidIndx].dataIndx;



                        ConvertBytes(ref fin, ref indx, rData);

                        if (fin.operation > 0)
                        {
                            if (TDFGlobals.showAllFields == true || fin.show == true)
                            {
                                fin.resultIndx = TDFGlobals.financialResults.Count;
                                TDFGlobals.financialResults.Add(fin);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"ProcessFinancialUpdateData error: {ex}");
            }
        }

        public static void ConvertBytes(ref fin_Data fin, ref int indx, byte[] rData)
        {
            int len;
            string s = "";
            //UInt32 fidIndx = 0;
            byte month = 0;
            byte day = 0;
            ushort year = 0;
            UInt32 nSec = 0;
            DateTime timeData = new DateTime();


            /*
            fidIndx = fin.fieldId & 0x0000ffff;
            fin.fieldfFmtId = field_Info_Table[fidIndx].fieldfFmtId;
            fin.fieldFmtOptional = field_Info_Table[fidIndx].fieldFmtOptional;
            fin.fieldName = field_Info_Table[fidIndx].fieldName;
            fin.businessId = field_Info_Table[fidIndx].businessId;
            fin.fieldDataType = field_Info_Table[fidIndx].fieldDataType;
            fin.show = field_Info_Table[fidIndx].show;
            fin.dataIndx = field_Info_Table[fidIndx].dataIndx;
            indx += 4;
            */

            switch (fin.fieldDataType)
            {
                // long  
                case 1:
                    fin.iData = BitConverter.ToInt32(rData, indx);
                    indx += 4;
                    break;
                // string 
                case 2:
                    len = rData[indx];
                    indx += 1;
                    byte[] temp20 = new byte[len];
                    Array.Copy(rData, indx, temp20, 0, len);
                    s = System.Text.Encoding.Default.GetString(temp20);
                    indx += len;
                    fin.sData = s;
                    break;
                // boolean byte 0 or 1
                case 3:
                    fin.bData = rData[indx];
                    indx += 1;
                    break;
                // byte
                case 4:
                    fin.bData = rData[indx];
                    indx += 1;
                    break;
                // float
                case 5:
                    fin.fData = BitConverter.ToSingle(rData, indx);
                    if (Math.Abs(fin.fData) < 0.000001)
                        fin.fData = 0.0F;
                    indx += 4;
                    break;
                // double
                case 6:
                    fin.dData = BitConverter.ToDouble(rData, indx);
                    indx += 8;
                    break;
                // Date "MM/DD/YYYY"
                case 9:
                    month = rData[indx];
                    indx += 1;
                    day = rData[indx];
                    indx += 1;
                    year = ReverseBytes2(BitConverter.ToUInt16(rData, indx));
                    indx += 2;
                    s = month.ToString() + "/" + day.ToString() + "/" + year.ToString();
                    fin.sData = s;
                    break;
                // Time "HH:MM:SS"
                case 10:
                    nSec = BitConverter.ToUInt32(rData, indx);
                    indx += 4;
                    //fin.sData = s;
                    fin.iData = (int)nSec;
                    timeData = UnixEpoch.AddSeconds(nSec);
                    fin.sData = timeData.ToShortDateString() + " " + timeData.ToShortTimeString();
                    break;
                // huge
                case 13:
                    fin.hData = BitConverter.ToInt64(rData, indx);
                    indx += 8;
                    break;
                // ulong  
                case 14:
                    fin.iData = BitConverter.ToInt32(rData, indx);
                    indx += 4;
                    break;
            }
        }




        public static Open_FID_Hdr Decode_OpenFID_Header(ushort OPenFIDHdr)
        {
            Open_FID_Hdr h = new Open_FID_Hdr();
            h.symBol = Convert.ToBoolean(OPenFIDHdr & 0x4000);
            h.littleEndndian = Convert.ToBoolean(OPenFIDHdr & 0x1000);
            h.msgCmd = (byte)((OPenFIDHdr & 0x0F00) >> 8);
            h.msgStruct = (byte)(OPenFIDHdr & 0x00FF);
            h.msgCmdStr = "";
            h.msgStructStr = "";

            switch (h.msgCmd)
            {
                case 0:
                    h.msgCmdStr = "unknown";
                    break;
                case 1:
                    h.msgCmdStr = "insert";
                    break;
                case 2:
                    h.msgCmdStr = "update";
                    break;
                case 3:
                    h.msgCmdStr = "select";
                    break;
                case 4:
                    h.msgCmdStr = "remove";
                    break;
                case 5:
                    h.msgCmdStr = "run";
                    break;
                case 6:
                    h.msgCmdStr = "refresh";
                    break;
            }

            switch (h.msgStruct)
            {
                case 1:
                    h.msgStructStr = "catalog with all sets";
                    h.directives = false;
                    break;
                case 2:
                    h.msgStructStr = "catalog with directives";
                    h.directives = true;
                    break;
                case 3:
                    h.msgStructStr = "nocatalog with all sets";
                    h.directives = false;
                    break;
                case 4:
                    h.msgStructStr = "nocatalog with directives";
                    h.directives = true;
                    break;
                case 5:
                    h.msgStructStr = "local catalog with all sets";
                    h.directives = false;
                    break;
                case 6:
                    h.msgStructStr = "local catalog with directives";
                    h.directives = true;
                    break;
            }
            return h;
        }
        public static ushort GetCatalogNumFIDS(ushort patternNo)
        {
            bool done = false;
            ushort i = 0;

            while (done == false)
            {
                if (TDFGlobals.CatalogData[patternNo - 1, i] == 0)
                {
                    done = true;
                }
                i++;
            }
            i--;
            return i;
        }

        // Reverses byte order (32 bit - 4 bytes)
        public static UInt32 ReverseBytes4(UInt32 value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }
        // reverse byte order (16 bit - 2 bytes)
        public static UInt16 ReverseBytes2(UInt16 value)
        {
            return (UInt16)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long GetCurrentUnixTimestampMillis()
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
        }

        public static DateTime DateTimeFromUnixTimestampMillis(long millis)
        {
            return UnixEpoch.AddMilliseconds(millis);
        }

        public static long GetCurrentUnixTimestampSeconds()
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalSeconds;
        }

        public static DateTime DateTimeFromUnixTimestampSeconds(long seconds)
        {
            return UnixEpoch.AddSeconds(seconds);
        }

        public void unSubscribeSymbol(string unSymbol)
        {
            if (unSymbol != null)
            {
                string queryStr = "DELETE FROM SUBSCRIPTION_TABLE WHERE channelName = DYNAMIC_QUOTES AND usrSymbol = \"" + unSymbol + "\"";
                uint seqID = 97;

                ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
                byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, queryStr, TDFconstants.DATA_REQUEST, seqID);
                //TRSendCommand(outputbuf);
                sendBuf(outputbuf);

            }
        }

        public void GetCataloger()
        {
            // Build Logon Message
            string queryStr = "SELECT * FROM CATALOGER_TABLE";

            ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
            byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, queryStr, TDFconstants.DATA_REQUEST, 99);
            //TRSendCommand(outputbuf);
            sendBuf(outputbuf);
        }
        public void GetFieldInfoTable()
        {
            // Build Logon Message
            string queryStr = "SELECT * FROM FIELD_INFO_TABLE";

            ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
            byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, queryStr, TDFconstants.DATA_REQUEST, 98);
            //TRSendCommand(outputbuf);
            sendBuf(outputbuf);
        }

        public void ProcessKeepAliveRequest(itf_Parser_Return_Message TRmess)
        {
            try
            {
                // Build Logon Message
                string queryStr = System.Text.Encoding.Default.GetString(TRmess.Message.ToArray());

                ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
                byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, queryStr, TDFconstants.KEEP_ALIVE_RESPONSE, 0);
                //TRSendCommand(outputbuf);
                sendBuf(outputbuf);
            }
            catch (Exception ex)
            {
                log.Error($"Process KEEP ALIVE REQUEST error: {ex}");
            }
        }
        public void SendKeepAliveRequest()
        {
            // Build Logon Message
            string queryStr = "KEEPALIVE";

            ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
            byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, queryStr, TDFconstants.KEEP_ALIVE_REQUEST, 0);
            //TRSendCommand(outputbuf);
            sendBuf(outputbuf);

        }



        #endregion


    }

}

