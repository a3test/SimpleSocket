using System.Collections.Generic;
using System.Net.Sockets;

namespace SimpleSocket
{
    public class StateObject
    {
        public const int BufferSize = 1024;

        public Socket WorkSocket { get; set; }

        public byte[] Buffer { get; set; }

        /// <summary>
        /// 接收的数据
        /// </summary>
        public List<byte> ReceiveBytes { get; set; }

        /// <summary>
        /// 接收的数据转成的字符串
        /// </summary>
        public List<string> Contents = new List<string>();

        public StateObject()
        {
            WorkSocket = null;
            Buffer = new byte[BufferSize];
            ReceiveBytes = new List<byte>();
        }
    }
}