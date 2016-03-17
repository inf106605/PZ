using SkyCrab.connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server_test
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TcpListener tcpListener = new TcpListener(BasicConnection.PORT);
                tcpListener.Start();
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                tcpListener.Stop();

                DataConnection connection = new ServerConnection(tcpClient, 100);
                connection.WriteUInt32(1000000000);
                uint aaa = connection.ReadUInt32();
                Console.WriteLine(aaa);
                Thread.Sleep(1000);
                connection.Close();
                Console.WriteLine("OK");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
