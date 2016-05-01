using System;
using System.Net;
using System.Net.Sockets;

namespace SkyCrabServer
{
    static class Listener
    {

        private static TcpListener tcpListener;
        private static volatile IAsyncResult asyncResult;
        private static volatile bool stoping = false;
        private static object _lock = new object();


        public static bool Listen(IPAddress ipAddress, int port)
        {
            try
            {
                SkyCrab_Server.serverConsole.WriteLine("Initializing the server at " + ipAddress + ":" + port + "...");

                SkyCrab_Server.serverConsole.WriteLine("Generating/Loading criptographic keys...");
                ServerConnection.PreLoadStaticMembers();

                tcpListener = new TcpListener(ipAddress, port);
                tcpListener.Start();

                asyncResult = tcpListener.BeginAcceptTcpClient(ClientAccepter, null);
                SkyCrab_Server.serverConsole.WriteLine("Accepting clients is begun.");

                SkyCrab_Server.serverConsole.Wait();

                lock (tcpListener)
                    asyncResult = null;

                tcpListener.Stop();
                SkyCrab_Server.serverConsole.WriteLine("Accepting clients is stopped.");

                lock (_lock)
                {
                    stoping = true;
                    ConnectionManager.CloseAll();
                }

                SkyCrab_Server.serverConsole.WriteLine("Clearing memory...");
                ServerConnection.DisposeStaticMembers();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return false;
            }

            return true;
        }

        private static void ClientAccepter(IAsyncResult result)
        {
            TcpClient tcpClient;
            lock (tcpListener)
            {
                if (asyncResult == null)
                    return;
                tcpClient = tcpListener.EndAcceptTcpClient(asyncResult);
                asyncResult = tcpListener.BeginAcceptTcpClient(ClientAccepter, null);
            }
            lock (_lock)
            {
                if (stoping)
                {
                    tcpClient.Close();
                    return;
                }
                ConnectionManager.Open(tcpClient);
            }
        }

    }
}
