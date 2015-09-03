using System;
using SimpleSocket;

namespace TestServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (SocketServer server = new SocketServer(9231))
            {
                server.TestListening();
                server.OnSocketReceive += server_OnSocketReceive;
                server.Listening();
            }

            Console.ReadLine();
        }

        static void server_OnSocketReceive(object sender, SocketReceiveEventArgs e)
        {
            foreach (string content in e.Contents)
            {
                Console.WriteLine(content);
                e.DataSender.Send(content.Length.ToString());
            }
        }

    }
}