using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TDFInterface
{
    public class ItfHeaderAccess

    {
        last_Response LR = new last_Response();

        #region Public Methods
        public itf_Header ParseItfHeader(byte[] data)
        {
            itf_Header itfh = new itf_Header();
            
            itfh.sync = data[0];
            itfh.msgSize = BitConverter.ToUInt16(data, 1);
            itfh.dataOffset = data[3];
            itfh.msgType = data[4];
            itfh.seqId = BitConverter.ToUInt32(data, 5);
            itfh.protId = BitConverter.ToUInt16(data, 9);
            itfh.sessionId = BitConverter.ToUInt16(data, 11);

            itfh.msgSize = (ushort)((itfh.msgSize & 0xFFU) << 8 | (itfh.msgSize & 0xFF00U) >> 8); // reverse bytes

            return itfh;
        }

        public itf_Parser_Return_Message ParseItfMessage(byte[] ldata) 
        {

            itf_Parser_Return_Message itf = new itf_Parser_Return_Message();
            itf_Header itfh = new itf_Header();
            log_data_Header dataH = new log_data_Header();
            service_data_Header sdatah = new service_data_Header ();
            return_data_Header rdatah = new return_data_Header();

            byte[] data = ldata.ToArray();

            itfh.sync = data[0];
            itfh.msgSize = BitConverter.ToUInt16(data,1);
            itfh.msgSize = (ushort)((itfh.msgSize & 0xFFU) << 8 | (itfh.msgSize & 0xFF00U) >> 8); // reverse bytes
            itfh.dataOffset = data[3];
            itfh.msgType = data[4];

            itfh.seqId = BitConverter.ToUInt32(data, 5);
            itfh.protId = BitConverter.ToUInt16(data, 9);
            itfh.sessionId = BitConverter.ToUInt16(data, 11);

            int messageLength = itfh.msgSize - itfh.dataOffset - 6;
            
            if (itfh.msgType == 0x58 || itfh.msgType == 0x49)
            //if (itfh.msgType == 0x49)
            {
                dataH.dataSize = BitConverter.ToUInt16(data, 13);  // logon response
                dataH.respType = data[15];
                dataH.errCode = BitConverter.ToUInt16(data, 17);
                rdatah.errCode = dataH.errCode;
                rdatah.respType = (ushort)dataH.respType;
                rdatah.numRecSize = dataH.dataSize;
                rdatah.respCtrl = 0;
                byte rc = 0;

                // decode respCtrl
                rdatah.entitlements = Convert.ToBoolean((byte)(rc & 0x80));
                rdatah.controlRecord = Convert.ToBoolean((byte)(rc & 0x08));
                rdatah.localCatalogger = Convert.ToBoolean((byte)(rc & 0x04));
                rdatah.traversal = Convert.ToBoolean((byte)(rc & 0x02));
                rdatah.more = Convert.ToBoolean((byte)(rc & 0x01));

                byte[] mess = new byte[data.Length - 20];
                int messageLen = mess.Length;
                Array.Copy(data, 19, mess, 0, messageLen - 1);
                //itf.Message = mess;
                itf.Message.AddRange(mess);
                itf.totalMessageSize = itf.Message.Count + 19;

            }
            else 
            {
                sdatah.errCode = BitConverter.ToUInt16(data, 13);  // data reponse
                sdatah.respType = BitConverter.ToUInt16(data, 15);
                sdatah.respType = (ushort)((sdatah.respType & 0xFFU) << 8 | (sdatah.respType & 0xFF00U) >> 8); // reverse bytes
                sdatah.numRec = BitConverter.ToUInt16(data, 17);
                sdatah.numRec = (ushort)((sdatah.numRec & 0xFFU) << 8 | (sdatah.numRec & 0xFF00U) >> 8); // reverse bytes
                sdatah.respCtrl = data[19];
                sdatah.headerExt = data[20];
                byte rc = data[19];
                            
                // decode respCtrl
                rdatah.entitlements = Convert.ToBoolean((byte)(rc & 0x80));
                rdatah.controlRecord = Convert.ToBoolean((byte)(rc & 0x08));
                rdatah.localCatalogger = Convert.ToBoolean((byte)(rc & 0x04));
                rdatah.traversal = Convert.ToBoolean((byte)(rc & 0x02));
                rdatah.more = Convert.ToBoolean((byte)(rc & 0x01));

                rdatah.errCode = sdatah.errCode;
                rdatah.respType = sdatah.respType;
                rdatah.numRecSize = sdatah.numRec;
                rdatah.respCtrl = sdatah.respCtrl;

                int buflen = 0;

                if (itfh.msgSize < data.Length)
                    buflen = itfh.msgSize - 20 + 1;
                else
                    buflen = data.Length - 20;
                
                byte[] mess = new byte[buflen + 1];
                //int messageLen = mess.Length;
                //Array.Copy(data, 21, mess, 0, buflen);
                Array.Copy(data, 21, mess, 0, buflen - 1);
                //itf.Message = mess;
                itf.Message.AddRange(mess);
                itf.totalMessageSize = itf.Message.Count + 20;
            
            }

            //LR.messageType = itfh.msgType;
            //LR.responseType = rdatah.respType;
            //LR.responseCtrl = rdatah.respCtrl;

            itf.itf_Header = itfh;
            itf.data_Header = rdatah;
            
            return itf;
        }

        public byte[] Build_Outbuf(itf_Header outHeader, string query, byte requestType, uint sequenceID)
        {
            
            // Build Message
            int msgLen = query.Length + 1 + outHeader.dataOffset;
            ushort msgLength = (ushort)((msgLen & 0xFFU) << 8 | (msgLen & 0xFF00U) >> 8); // reverse bytes
            outHeader.msgSize = msgLength;
           
            int bufsize = outHeader.dataOffset + query.Length + 2;
            byte[] outputbuf = new byte[bufsize];
            byte[] temp2 = new byte[2];
            byte[] temp4 = new byte[4];
            byte[] qbuf = Encoding.ASCII.GetBytes(query);

            outputbuf[0] = outHeader.sync;
            temp2 = BitConverter.GetBytes(outHeader.msgSize);
            temp2.CopyTo(outputbuf, 1);
            outputbuf[3] = outHeader.dataOffset;
            outputbuf[4] = requestType;
            temp4 = BitConverter.GetBytes(sequenceID);
            temp4.CopyTo(outputbuf, 5);
            temp2 = BitConverter.GetBytes(outHeader.protId);
            temp2.CopyTo(outputbuf, 9);
            temp2 = BitConverter.GetBytes(outHeader.sessionId);
            temp2.CopyTo(outputbuf, 11);
            qbuf.CopyTo(outputbuf, 13);
            outputbuf[bufsize - 1] = 0;

            return outputbuf;
        }

        public byte GetMsgType(byte[] ldata)
        {

            itf_Short_Header itfh = new itf_Short_Header();

            byte[] data = ldata.ToArray();

            itfh.sync = data[0];
            itfh.msgSize = BitConverter.ToUInt16(data, 1);
            itfh.msgSize = (ushort)((itfh.msgSize & 0xFFU) << 8 | (itfh.msgSize & 0xFF00U) >> 8); // reverse bytes
            itfh.dataOffset = data[3];
            itfh.msgType = data[4];

            return itfh.msgType;
        }

        public ushort GetMsgSize(byte[] ldata)
        {

            itf_Short_Header itfh = new itf_Short_Header();

            byte[] data = ldata.ToArray();

            itfh.sync = data[0];
            itfh.msgSize = BitConverter.ToUInt16(data, 1);
            itfh.msgSize = (ushort)((itfh.msgSize & 0xFFU) << 8 | (itfh.msgSize & 0xFF00U) >> 8); // reverse bytes
            itfh.dataOffset = data[3];
            itfh.msgType = data[4];

            return itfh.msgSize;
        }

        public itf_Parser_Update_Message ParseItfUpdateMessage(byte[] ldata)
        {

            itf_Short_Header itfsh = new itf_Short_Header();
            itf_Parser_Update_Message itfu = new itf_Parser_Update_Message();
            
            byte[] data = ldata.ToArray();

            itfsh.sync = data[0];
            itfsh.msgSize = BitConverter.ToUInt16(data, 1);
            itfsh.msgSize = (ushort)((itfsh.msgSize & 0xFFU) << 8 | (itfsh.msgSize & 0xFF00U) >> 8); // reverse bytes
            itfsh.dataOffset = data[3];
            itfsh.msgType = data[4];

            int messageLength = itfsh.msgSize - itfsh.dataOffset - 6;

                
                int buflen = 0;

                if (itfsh.msgSize < data.Length)
                    buflen = itfsh.msgSize - 6 + 1;
                else
                    buflen = data.Length - 26;

                byte[] mess = new byte[buflen + 1];
                Array.Copy(data, 5, mess, 0, buflen - 1);
                itfu.Message.AddRange(mess);
                itfu.totalMessageSize = itfu.Message.Count + 6;

            
            //LR.messageType = itfsh.msgType;
            //LR.responseType = 0;
            //LR.responseCtrl = 0;

            itfu.itf_Short_Header = itfsh;
            
            return itfu;
        }
        

        public itf_Control_Message ParseItfControlMessage(byte[] ldata)
        {

            itf_Short_Header itfsh = new itf_Short_Header();
            Control_Message_Header cmh = new Control_Message_Header();
            itf_Control_Message icm = new itf_Control_Message();

            byte[] data = ldata.ToArray();

            itfsh.sync = data[0];
            itfsh.msgSize = BitConverter.ToUInt16(data, 1);
            itfsh.msgSize = (ushort)((itfsh.msgSize & 0xFFU) << 8 | (itfsh.msgSize & 0xFF00U) >> 8); // reverse bytes
            itfsh.dataOffset = data[3];
            itfsh.msgType = data[4];

            cmh.numRec = BitConverter.ToUInt16(data, 5);
            cmh.messageCategory = data[7];
            cmh.messageCode = data[8];

            int messageLength = itfsh.msgSize - itfsh.dataOffset - 6;


            int buflen = 0;

            if (itfsh.msgSize < data.Length)
                buflen = itfsh.msgSize - 6 + 1;
            else
                buflen = data.Length - 26;

            if (ldata.Length >= 11)
            {
                icm.messageLen = BitConverter.ToUInt16(data, 9);
                byte[] mess = new byte[icm.messageLen];
                if (ldata.Length >= icm.messageLen + 11)
                {
                    Array.Copy(data, 11, mess, 0, icm.messageLen - 2);
                    icm.Message.AddRange(mess);
                }
            }

            icm.totalMessageSize = icm.Message.Count + 6;
            icm.itf_Short_Header = itfsh;
            icm.control_Message_Header = cmh;

            return icm;
        }


        #endregion
    }
}
