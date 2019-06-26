using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;


// Wrapper for TCP client
// Rev: 08/01/16
// M. Dilworth

namespace AsyncClientSocket
{
    /// <summary>
    /// Event driven TCP client wrapper
    /// </summary>
    public class ClientSocket : IDisposable
    {
        #region Logger instantiation - uses reflection to get module name

        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Consts/Default values

        private const int Defaulttimeout = 5000; //Default to 5 seconds on all timeouts
        private const int Reconnectinterval = 2000; //Default to 2 seconds reconnect attempt rate

        #endregion

        #region Components (Timers), Events, Delegates, and CTOR

        //Timer used to detect receive timeouts
        private readonly System.Timers.Timer _tmrReceiveTimeout = new System.Timers.Timer();
        private readonly System.Timers.Timer _tmrSendTimeout = new System.Timers.Timer();
        private readonly System.Timers.Timer _tmrConnectTimeout = new System.Timers.Timer();

        //public delegate void DelDataReceived(ClientSocket sender, object data);
        public delegate void DelDataReceived(ClientSocket sender, byte[] data);
        //public delegate void DelDataReceived(ClientSocket sender, List<byte> data);
        public int bufLen = 0;

        public event DelDataReceived DataReceived;

        public delegate void DelConnectionStatusChanged(ClientSocket sender, ConnectionStatus status);

        public event DelConnectionStatusChanged ConnectionStatusChanged;

        public enum ConnectionStatus
        {
            NeverConnected,
            Connecting,
            Connected,
            AutoReconnecting,
            DisconnectedByUser,
            DisconnectedByHost,
            ConnectFailTimeout,
            ReceiveFailTimeout,
            SendFailTimeout,
            SendFailNotConnected,
            Error
        }

        // Constructor for TCPClientWrapper
        public ClientSocket(IPAddress ip, int port, bool autoreconnect = true)
        {
            _ip = ip;
            _port = port;
            AutoReconnect = autoreconnect;
            _client = new TcpClient(AddressFamily.InterNetwork) {NoDelay = true};
            //Disable the nagel algorithm for simplicity
            ReceiveTimeout = Defaulttimeout;
            SendTimeout = Defaulttimeout;
            ConnectTimeout = Defaulttimeout;
            ReconnectInterval = Reconnectinterval;
            _tmrReceiveTimeout.AutoReset = true;
            _tmrReceiveTimeout.Elapsed += tmrReceiveTimeout_Elapsed;
            _tmrConnectTimeout.AutoReset = false;
            _tmrConnectTimeout.Elapsed += tmrConnectTimeout_Elapsed;
            _tmrSendTimeout.AutoReset = false;
            _tmrSendTimeout.Elapsed += tmrSendTimeout_Elapsed;

            ConnectionState = ConnectionStatus.NeverConnected;
        }

        #endregion

        #region Private methods/Event Handlers

        void tmrSendTimeout_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ConnectionState = ConnectionStatus.SendFailTimeout;
            DisconnectByHost();
        }

        void tmrReceiveTimeout_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ConnectionState = ConnectionStatus.ReceiveFailTimeout;
            DisconnectByHost();
        }

        void tmrConnectTimeout_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ConnectionState = ConnectionStatus.ConnectFailTimeout;
            DisconnectByHost();
        }

        private void DisconnectByHost()
        {
            ConnectionState = ConnectionStatus.DisconnectedByHost;
            _tmrReceiveTimeout.Stop();
            if (AutoReconnect)
                Reconnect();
        }

        private void Reconnect()
        {
            if (ConnectionState == ConnectionStatus.Connected)
                return;
            ConnectionState = ConnectionStatus.AutoReconnecting;
            try
            {
                _client.Client.BeginDisconnect(true, CbDisconnectByHostComplete, _client.Client);
            }
            catch (Exception ex)
            {
                // Log error
                Log.Error("ClientSocket Exception occurred while attempting reconnect: " + ex.Message);
                Log.Debug("ClientSocket Exception occurred while attempting reconnect", ex);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Try connecting to the remote host
        /// </summary>
        public void Connect()
        {
            try
            {
                if (ConnectionState == ConnectionStatus.Connected)
                    return;
                ConnectionState = ConnectionStatus.Connecting;
                _tmrConnectTimeout.Start();
                _client.BeginConnect(_ip, _port, CbConnect, _client.Client);
            }
            catch (Exception ex)
            {
                // Log error
                Log.Error("ClientSocket Exception occurred while attempting connect: " + ex.Message);
                Log.Debug("ClientSocket Exception occurred while attempting connect", ex);
            }
        }

        /// <summary>
        /// Try disconnecting from the remote host
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (ConnectionState != ConnectionStatus.Connected)
                    
                return;

                _tmrReceiveTimeout.Stop();

                _client.Client.BeginDisconnect(true, CbDisconnectComplete, _client.Client);
            }
            catch (Exception ex)
            {
                // Log error
                Log.Error("ClientSocket Exception occurred while attempting disconnect: " + ex.Message);
                Log.Debug("ClientSocket Exception occurred while attempting disconnect", ex);
            }
        }

        /// <summary>
        /// Try sending a string to the remote host
        /// </summary>
        /// <param name="data">The data to send</param>
        public void Send(byte[] data)
        {
            try
            {
                if (ConnectionState != ConnectionStatus.Connected)
                {
                    ConnectionState = ConnectionStatus.SendFailNotConnected;
                    return;
                }
                //var bytes = _encode.GetBytes(data);
                SocketError err;
                _tmrSendTimeout.Start();
                //_client.Client.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, out err, CbSendComplete,
                //    _client.Client);

                _client.Client.BeginSend(data , 0, data.Length , SocketFlags.None, out err, CbSendComplete,
                    _client.Client);


                if (err != SocketError.Success)
                {
                    Action doDcHost = DisconnectByHost;
                    doDcHost.Invoke();
                }
            }
            catch (Exception ex)
            {
                // Log error
                Log.Error("ClientSocket Exception occurred while attempting to send data (string): " + ex.Message);
                Log.Debug("ClientSocket Exception occurred while attempting to send data (string)", ex);
            }
        }

        /// <summary>
        /// Try sending byte data to the remote host
        /// </summary>
        /// <param name="data">The data to send</param>

        /*
          public void Send(byte[] data)
        {
            try
            {
                if (ConnectionState != ConnectionStatus.Connected)
                    throw new InvalidOperationException("Cannot send data, socket is not connected");
                SocketError err;
                _client.Client.BeginSend(data, 0, data.Length, SocketFlags.None, out err, CbSendComplete, _client.Client);
                if (err != SocketError.Success)
                {
                    Action doDcHost = DisconnectByHost;
                    doDcHost.Invoke();
                }
            }
            catch (Exception ex)
            {
                // Log error
                Log.Error("ClientSocket Exception occurred while attempting to send data (byte array): " + ex.Message);
                Log.Debug("ClientSocket Exception occurred while attempting to send data (byte array)", ex);
            }
        }
        */
        // Implement IDisposable; do not make this method virtual; a derived class should not be able to override this method
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to take this object off the finalization queue 
            // and prevent finalization code for this object from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly or indirectly by a user's code. Managed and unmanaged resources 
        // can be disposed. 
        // If disposing equals false, the method has been called by the runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {
            lock (SyncLock)
            {
                // Check to see if Dispose has already been called. 
                if (!_disposed)
                {
                
                    _tmrConnectTimeout.Enabled = false;
                    _tmrReceiveTimeout.Enabled = false;
                    _tmrSendTimeout.Enabled = false;              
                
                    // If disposing equals true, dispose all managed 
                    // and unmanaged resources. 
                    if (disposing)
                    {
                        // Dispose managed resources.
                        //this._client.Client.Dispose();
                        _client.Close(); 
                    }

                    // Call the appropriate methods to clean up unmanaged resources here. If disposing is false, only the following code is executed.
                    // Note disposing has been done.
                    _disposed = true;

                }

            }
        }     

        // Use C# destructor syntax for finalization code. This destructor will run only if the Dispose method does not get called. 
        // It gives your base class the opportunity to finalize. Do not provide destructors in types derived from this class.
        ~ClientSocket()
        {
            // Do not re-create Dispose clean-up code here. Calling Dispose(false) is optimal in terms of readability and maintainability.
            Dispose(false);
        }

        #endregion

        #region Callbacks

        private void CbConnectComplete()
        {
            if (_client.Connected)
            {
                _tmrConnectTimeout.Stop();
                ConnectionState = ConnectionStatus.Connected;
                //_client.Client.BeginReceive(_dataBuffer, 0, _dataBuffer.Length, SocketFlags.None, CbDataReceived, _client.Client);
                _client.Client.BeginReceive(_dataBuffer, 0, _dataBuffer.Length  , SocketFlags.None, CbDataReceived, _client.Client);
            }
            else
            {
                ConnectionState = ConnectionStatus.Error;
            }
        }

        private void CbDisconnectByHostComplete(IAsyncResult result)
        {
            var r = result.AsyncState as Socket;
            if (r == null)
                throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as a socket object");
            r.EndDisconnect(result);
            if (AutoReconnect)
            {
                Action doConnect = Connect;
                doConnect.Invoke();
           
            }
        }

        private void CbDisconnectComplete(IAsyncResult result)
        {
            var r = result.AsyncState as Socket;
            if (r == null)
                throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as a socket object");
            r.EndDisconnect(result);
            ConnectionState = ConnectionStatus.DisconnectedByUser;

        }

        private void CbConnect(IAsyncResult result)
        {
            var sock = result.AsyncState as Socket;
            if (result == null)
                throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as a socket object");
            if (sock != null && !sock.Connected)
            {
                if (AutoReconnect)
                {
                    System.Threading.Thread.Sleep(ReconnectInterval);
                    Action reconnect = Connect;
                    reconnect.Invoke();
                    return;
                }
                else
                    return;
            }
            if (sock != null) sock.EndConnect(result);
            var callBack = new Action(CbConnectComplete);
            callBack.Invoke();
        }

        private void CbSendComplete(IAsyncResult result)
        {
            var r = result.AsyncState as Socket;
            if (r == null)
                throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as a socket object");
            SocketError err;
            r.EndSend(result, out err);
            if (err != SocketError.Success)
            {
                Action doDcHost = DisconnectByHost;
                doDcHost.Invoke();
            }
            else
            {
                lock (SyncLock)
                {
                    _tmrSendTimeout.Stop();
                }
            }
        }
        
        private static void CbChangeConnectionStateComplete(IAsyncResult result)
        {
            var r = result.AsyncState as ClientSocket;
            if (r == null)
                throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as a EDTC object");
            r.ConnectionStatusChanged.EndInvoke(result);
        }

        private void CbDataReceived(IAsyncResult result)
        {
            var sock = result.AsyncState as Socket;
            if (sock == null)
                throw new InvalidOperationException("Invalid IASyncResult - Could not interpret as a socket");
            SocketError err;
            int bytes = sock.EndReceive(result, out err);
            bufLen = bytes;
            if (bytes == 0 || err != SocketError.Success)
            {
                lock (SyncLock)
                {
                    _tmrReceiveTimeout.Start();
                    return ;
                }
            }
            else
            {
                lock (SyncLock)
                {
                    _tmrReceiveTimeout.Stop();
                }
            }
            if (DataReceived != null)
                DataReceived.BeginInvoke(this, _dataBuffer, CbDataRecievedCallbackComplete, this);
            //    DataReceived.BeginInvoke(this, _encode.GetString(_dataBuffer, 0, bytes), CbDataRecievedCallbackComplete, this);
            return ;
        }

        private void CbDataRecievedCallbackComplete(IAsyncResult result)
        {
            
            var r = result.AsyncState as ClientSocket;
            if (r == null)
                throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as EDTC object");
            r.DataReceived.EndInvoke(result);
            SocketError err;
            _client.Client.BeginReceive(_dataBuffer, 0, _dataBuffer.Length, SocketFlags.None, out err, CbDataReceived, _client.Client);
            //_client.Client.BeginReceive(_dataBuffer, 0, bufLen, SocketFlags.None, out err, CbDataReceived, _client.Client);
            if (err != SocketError.Success)
            {
                Action doDcHost = DisconnectByHost;
                doDcHost.Invoke();
            }
        }

        #endregion

        #region Properties and members
        
        private readonly IPAddress _ip;
        private ConnectionStatus _conStat;
        private readonly TcpClient _client;
        private readonly byte[] _dataBuffer = new byte[500000];
        //private readonly byte[] _dataBuffer = new byte[5000];
        //private readonly List<byte> _dataBuffer = new List<byte>();
        private readonly int _port;
        private Encoding _encode = Encoding.Default;
        private readonly object _syncLock = new object();
        // Track whether Dispose has been called. 
        private bool _disposed;

        /// <summary>
        /// Syncronizing object for asyncronous operations
        /// </summary>
        public object SyncLock
        {
            get
            {
                return _syncLock;
            }
        }

        /// <summary>
        /// Encoding to use for sending and receiving
        /// </summary>
        public Encoding DataEncoding
        {
            get
            {
                return _encode;
            }
            set
            {
                _encode = value;
            }
        }

        /// <summary>
        /// Current state that the connection is in
        /// </summary>
        public ConnectionStatus ConnectionState
        {
            get
            {
                return _conStat;
            }
            private set
            {
                bool raiseEvent = value != _conStat;
                _conStat = value;
                if (ConnectionStatusChanged != null && raiseEvent)
                    ConnectionStatusChanged.BeginInvoke(this, _conStat, CbChangeConnectionStateComplete, this);
            }
        }

        /// <summary>
        /// True to autoreconnect at the given reconnection interval after a remote host closes the connection
        /// </summary>
        public bool AutoReconnect { get; set; }

        public int ReconnectInterval { get; set; }

        /// <summary>
        /// IP of the remote host
        /// </summary>
        public IPAddress Ip
        {
            get
            {
                return _ip;
            }
        }
        
        /// <summary>
        /// Port to connect to on the remote host
        /// </summary>
        public int Port
        {
            get
            {
                return _port;
            }
        }
       
        /// <summary>
        /// Time to wait after a receive operation is attempted before a timeout event occurs
        /// </summary>
        public int ReceiveTimeout
        {
            get
            {
                return (int)_tmrReceiveTimeout.Interval;
            }
            set
            {
                _tmrReceiveTimeout.Interval = value;
            }
        }
       
        /// <summary>
        /// Time to wait after a send operation is attempted before a timeout event occurs
        /// </summary>
        public int SendTimeout
        {
            get
            {
                return (int)_tmrSendTimeout.Interval;
            }
            set
            {
                _tmrSendTimeout.Interval = value;
            }
        }
        
        /// <summary>
        /// Time to wait after a connection is attempted before a timeout event occurs
        /// </summary>
        public int ConnectTimeout
        {
            get
            {
                return (int)_tmrConnectTimeout.Interval;
            }
            set
            {
                _tmrConnectTimeout.Interval = value;
            }
        }
        #endregion
    }
}
