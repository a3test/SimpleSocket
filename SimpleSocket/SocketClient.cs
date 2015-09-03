using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SimpleSocket
{
    public class SocketClient : BaseSocket, IDisposable
    {
        private Socket _sender;

        #region 属性(只读)

        /// <summary>
        /// 远端IP
        /// </summary>
        public string Ip { get; private set; }

        /// <summary>
        /// 远端端口
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// 远端IP
        /// </summary>
        public IPAddress IpAddress { get; private set; }

        #endregion

        #region 构造函数

        public SocketClient(IPAddress ipAddress, int port)
        {
            IpAddress = ipAddress;
            Ip = IpAddress.ToString();
            Port = port;
        }

        public SocketClient(string ip, int port)
            : this(IPAddress.Parse(ip), port)
        {
        }

        #endregion

        /// <summary>
        /// 连接
        /// </summary>
        public void Connect()
        {
            IPEndPoint remoteEP = new IPEndPoint(IpAddress, Port);
            _sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _sender.Connect(remoteEP);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public void Send(string data)
        {
            Sender sender = new Sender(_sender);
            sender.Send(data);
        }

        public void Receive()
        {
            StateObject state = new StateObject();
            state.WorkSocket = _sender;

            SocketError se;
            _sender.BeginReceive(state.Buffer, 0, StateObject.BufferSize, SocketFlags.None, out se, ReceiveCallback,
                state);
            if (se != SocketError.Success)
            {
                _sender.Close();
                _sender.Dispose();
            }
        }

        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SocketClient()
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

                }
                //Release unmanaged resources
                if (_sender != null)
                {
                    _sender.Close();
                    _sender.Dispose();
                }

                _disposed = true;
            }
        }

        #endregion
    }
}