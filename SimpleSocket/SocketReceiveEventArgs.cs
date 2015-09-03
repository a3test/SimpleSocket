using System;
using System.Collections.Generic;

namespace SimpleSocket
{
    public class SocketReceiveEventArgs : EventArgs
    {
        /// <summary>
        /// 接收到的数据
        /// </summary>
        public List<string> Contents { get; set; }

        /// <summary>
        /// <para>当前远端和本地的有效连接,用于向远端发送数据</para>
        /// </summary>
        public Sender DataSender { get; set; }

        public SocketReceiveEventArgs(Sender dataSender, List<string> contents)
        {
            DataSender = dataSender;
            Contents = contents;
        }
    }
}