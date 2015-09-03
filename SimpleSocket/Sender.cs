using System;
using System.IO;
using System.Net.Sockets;

namespace SimpleSocket
{
    /// <summary>
    /// 数据发送处理类
    /// </summary>
    public class Sender
    {
        private Socket _handler;

        /// <summary>
        /// 所有数据发送都应经过该方法
        /// </summary>
        /// <param name="handler"></param>
        public Sender(Socket handler)
        {
            _handler = handler;
        }

        public void Send(string data)
        {
            byte[] tmp;
            MemoryStream ms = new MemoryStream();
            using (BinaryWriter bw = new BinaryWriter(ms, Config.DefaultEncoding))
            {
                if (data.Length > 0)
                {
                    byte[] c = Config.DefaultEncoding.GetBytes(data);
                    bw.Write(c.Length);
                    bw.Write(c);
                }
                tmp = ms.ToArray();
            }
            _handler.Send(tmp);
        }
    }
}