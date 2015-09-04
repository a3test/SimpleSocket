using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

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
            SetKeepAlive();
            _sender.Connect(remoteEP);
        }

        /// <summary>
        /// 设置KeepAlive
        /// </summary>
        private void SetKeepAlive()
        {
            if (_sender != null)
            {
                // Socket空闲多久后开始发送KeepAlive包(单位:毫秒)
                uint KeepAliveTime = 5000;
                //KeepAlive包发送间隔时间(单位:毫秒)
                uint KeepAliveInterval = 5000;

                uint dummy = 0;
                byte[] optionInValue = new byte[Marshal.SizeOf(dummy)*3];
                BitConverter.GetBytes((uint) 1).CopyTo(optionInValue, 0);
                BitConverter.GetBytes(KeepAliveTime).CopyTo(optionInValue, Marshal.SizeOf(dummy));
                BitConverter.GetBytes(KeepAliveInterval).CopyTo(optionInValue, Marshal.SizeOf(dummy)*2);
                _sender.IOControl(IOControlCode.KeepAliveValues, optionInValue, null);
            }
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

            try
            {
                _sender.BeginReceive(state.Buffer, 0, StateObject.BufferSize, SocketFlags.None, ReceiveCallback, state);
            }
            catch (Exception)
            {
                _sender.Shutdown(SocketShutdown.Both);
                _sender.Close();
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
                    _sender.Shutdown(SocketShutdown.Both);
                    _sender.Close();
                }

                _disposed = true;
            }
        }

        #endregion
    }
}