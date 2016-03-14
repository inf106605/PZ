using SkyCrab.connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server_test
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TcpListener tcpListener = new TcpListener(Connection.PORT);
                tcpListener.Start();
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                tcpListener.Stop();

                Connection connection = new ServerConnection(tcpClient);
                connection.Write("Message from server.");
                string text = connection.Read();
                Console.WriteLine(text);
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
