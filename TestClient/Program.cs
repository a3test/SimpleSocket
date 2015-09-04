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
            using (SocketClient client = new SocketClient("115.28.137.134", 8081))
            {
                client.Connect();
                client.OnSocketReceive += client_OnSocketReceive;
                client.Receive();
                client.Send("你好");
                client.Send("12345");
                client.Send("abcd");

                Console.ReadLine();

                client.Send("你好2");
                client.Send("123452");
                client.Send("abcd2");

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