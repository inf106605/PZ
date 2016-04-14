using System;
using System.Net;
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

                IPAddress ipAddress = new IPAddress(new byte[4] { 127, 0, 0, 1 });
                TcpListener tcpListener = new TcpListener(ipAddress, ServerConnection.PORT);
                tcpListener.Start();
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                tcpListener.Stop();

                using (ServerConnection connection = new ServerConnection(tcpClient, 100))
                {
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
