using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleSocket;

namespace TestClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (SocketClient client = new SocketClient("127.0.0.1", 9231))
            {
                client.Connect();
                client.OnSocketReceive += client_OnSocketReceive;
                client.Receive();
                client.Send("你好");
                client.Send("12345");
                client.Send("abcd");
                Console.ReadLine();
            }

            Console.ReadLine();
        }

        static void client_OnSocketReceive(object sender, SocketReceiveEventArgs e)
        {
            foreach (string content in e.Contents)
            {
                Console.WriteLine(content);
            }
        }
    }
}