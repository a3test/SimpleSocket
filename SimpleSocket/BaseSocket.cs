using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SimpleSocket
{
    public class BaseSocket
    {
        /// <summary>
        /// 接收到数据的处理事件
        /// </summary>
        public event EventHandler<SocketReceiveEventArgs> OnSocketReceive;

        /// <summary>
        /// 接受到数据事件处理函数
        /// </summary>
        /// <param name="e"></param>
        private void SocketReceiveProcess(SocketReceiveEventArgs e)
        {
            if (OnSocketReceive != null)
            {
                OnSocketReceive(this, e);
            }
        }

        /// <summary>
        /// 接收回调
        /// </summary>
        /// <param name="ar"></param>
        protected void ReceiveCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject) ar.AsyncState;
            Socket handler = state.WorkSocket;

            SocketError se;
            int recLen = handler.EndReceive(ar, out se);
            if (se != SocketError.Success)
            {
                handler.Dispose();
                return;
            }

            byte[] buffer = new byte[recLen];
            Array.Copy(state.Buffer, 0, buffer, 0, recLen);
            bool readState = AnalyticReceiveData(buffer, state.ReceiveBytes, state.Contents);
            if (readState && state.ReceiveBytes.Count == 0)
            {
                //Event
                Sender sender = new Sender(handler);
                SocketReceiveProcess(new SocketReceiveEventArgs(sender, state.Contents));

                state.Contents.Clear();
            }
            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, SocketFlags.None, out se, ReceiveCallback,
                state);
            if (se != SocketError.Success)
            {
                handler.Dispose();
            }
        }

        /// <summary>
        /// 解析接收到的数据
        /// </summary>
        /// <param name="buffer">接收缓冲区</param>
        /// <param name="data">多次接收到字节都存到该变量中</param>
        /// <param name="contents">解析出的内容</param>
        /// <returns>
        /// <para>true:有解析出的新数据</para>
        /// <para>false:没能解析出新数据,可能需要接着读取</para>
        /// </returns>
        private bool AnalyticReceiveData(byte[] buffer, List<byte> data, List<string> contents)
        {
            data.AddRange(buffer);
            List<string> tmp = new List<string>();
            GetReceiveContent(data, tmp);
            contents.AddRange(tmp);
            return tmp.Count > 0;
        }

        /// <summary>
        /// 从接收的数据中获取内容 递归
        /// 说明：数据传输主要分两种情况。
        /// 1.缓冲区中存在多次发送的数据这样就需要递归
        /// 2.缓冲区中只存在一次发送的数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="contents"></param>
        private void GetReceiveContent(List<byte> data, List<string> contents)
        {
            if (data.Count >= Config.ByteSizeLength)
            {
                byte[] sizeBytes = data.GetRange(0, Config.ByteSizeLength).ToArray();
                int size = BitConverter.ToInt32(sizeBytes, 0);
                if (size > 0 && data.Count >= size + Config.ByteSizeLength) //如果小于则说明缓冲区没能容纳一次完整发送的数据
                {
                    data.RemoveRange(0, Config.ByteSizeLength);

                    byte[] contentBytes = data.GetRange(0, size).ToArray();
                    data.RemoveRange(0, size);

                    contents.Add(Config.DefaultEncoding.GetString(contentBytes));
                    GetReceiveContent(data, contents);
                }
            }
        }
    }
}