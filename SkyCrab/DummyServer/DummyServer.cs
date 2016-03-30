using SkyCrab.Connection;
using System;
using System.Net.Sockets;
using System.Threading;

namespace DummyServer
{
    class DummyServer
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Server");
                ServerConnection.Inicjalize();

                TcpListener tcpListener = new TcpListener(BasicConnection.PORT);
                tcpListener.Start();
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                tcpListener.Stop();

                using (DataConnection connection = new ServerConnection(tcpClient, 100))
                {
                    connection.WriteUInt32(101011101);
                    string aaa = connection.ReadString();
                    Console.WriteLine(aaa);
                    Thread.Sleep(1000);
                }

                ServerConnection.Deinicjalize();
                Console.WriteLine("OK");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
