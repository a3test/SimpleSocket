using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SimpleSocket
{
    public class SocketServer : BaseSocket, IDisposable
    {
        private Socket _listener;

        private readonly ManualResetEvent _acceptSignal = new ManualResetEvent(false);

        #region 属性(只读)

        /// <summary>
        /// 监听IP
        /// </summary>
        public string Ip { get; private set; }

        /// <summary>
        /// 监听端口
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// 挂起连接的最大长度
        /// </summary>
        public int Backlog { get; private set; }

        /// <summary>
        /// 监听IP
        /// </summary>
        public IPAddress IpAddress { get; private set; }

        #endregion

        #region 构造函数

        public SocketServer(IPAddress ipAddress, int port, int backlog)
        {
            IpAddress = ipAddress;
            Ip = IpAddress.ToString();
            Port = port;
            Backlog = backlog;
        }

        public SocketServer(string ip, int port, int backlog)
            : this(IPAddress.Parse(ip), port, backlog)
        {
        }

        public SocketServer(IPAddress ipAddress, int port)
            : this(ipAddress, port, 100)
        {
        }

        public SocketServer(string ip, int port)
            : this(IPAddress.Parse(ip), port, 100)
        {
        }

        public SocketServer(int port, int backlog)
            : this(IPAddress.Any, port, backlog)
        {
        }

        public SocketServer(int port)
            : this(IPAddress.Any, port)
        {
        }

        public SocketServer(IPEndPoint localPoint, int backlog)
            : this(localPoint.Address, localPoint.Port, backlog)
        {
        }

        public SocketServer(IPEndPoint localPoint)
            : this(localPoint, 100)
        {
        }

        #endregion

        /// <summary>
        /// 开始监听
        /// </summary>
        public void Listening()
        {
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(new IPEndPoint(IpAddress, Port));
            _listener.Listen(Backlog);

            while (true)
            {
                _acceptSignal.Reset();
                _listener.BeginAccept(AcceptCallback, _listener);
                _acceptSignal.WaitOne();
            }
        }

        /// <summary>
        /// 新连接回调
        /// </summary>
        /// <param name="ar"></param>
        private void AcceptCallback(IAsyncResult ar)
        {
            _acceptSignal.Set();
            Socket listener = (Socket) ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            StateObject state = new StateObject();
            state.WorkSocket = handler;
            SocketError se;
            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, SocketFlags.None, out se, ReceiveCallback,
                state);
            if (se != SocketError.Success)
            {
                //TODO:: Do something
                Console.WriteLine("Test message: AcceptCallback()");
            }
        }

        /// <summary>
        /// 测试监听
        /// </summary>
        /// <returns></returns>
        public bool TestListening()
        {
            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    socket.Bind(new IPEndPoint(IpAddress, Port));
                    socket.Listen(Backlog);
                }

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SocketServer()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    //Release managed resources
                    _acceptSignal.Dispose();

                }
                //Release unmanaged resources
                if (_listener != null)
                {
                    _listener.Close();
                    _listener.Dispose();
                }

                _disposed = true;
            }
        }

        #endregion

    }
}