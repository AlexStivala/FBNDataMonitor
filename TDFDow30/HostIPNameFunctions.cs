using System;
using System.Net;
using System.Net.Sockets;

namespace TDFDow30
{
    public static class HostIPNameFunctions
    {
        #region Logger instantiation - uses reflection to get module name
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        // Get the local IP address
        public static string GetLocalIPAddress()
        {
            string ipAddress = string.Empty;
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipAddress = ip.ToString();
                    }
                }
                if (ipAddress == string.Empty)
                {
                    throw new Exception("Local IP Address Not Found!");
                }
            }
            catch (Exception ex)
            {
                // Log error
                log.Error("HostIPNameFunctions Exception occurred while trying to get IP address: " + ex.Message);
                log.Debug("HostIPNameFunctions Exception occurred while trying to get IP address", ex);
            }
            return ipAddress;
        }

        // Get thre host name based on the IP address
        public static string GetHostName(string ipAddress)
        {
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(ipAddress);
                if (entry != null)
                {
                    return entry.HostName;
                }
            }
            catch (SocketException ex)
            {
                // Log error
                log.Error("HostIPNameFunctions Exception occurred while trying to get host name: " + ex.Message);
                log.Debug("HostIPNameFunctions Exception occurred while trying to get host name", ex);
            }

            return null;
        }



    }
}
