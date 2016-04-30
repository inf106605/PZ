using System;
using System.Net;
using System.Net.Sockets;

namespace SkyCrabServer
{
    class Listener
    {

        private static TcpListener tcpListener;
        private static volatile IAsyncResult asyncResult;
        private static volatile bool stoping = false;
        public static ServerConsole serverConsole;
        private static object _lock = new object();


        public static bool Listen(IPAddress ipAddress, int port)
        {
            try
            {
                Console.WriteLine("Initializing the server at " + ipAddress + ":" + port + "...\n");
                ServerConnection.PreLoadStaticMembers();

                tcpListener = new TcpListener(ipAddress, port);
                tcpListener.Start();

                asyncResult = tcpListener.BeginAcceptTcpClient(ClientAccepter, null);
                Console.WriteLine("Accepting clients is begun.\n");

                serverConsole = new ServerConsole();
                serverConsole.Start();
                serverConsole = null;

                lock (tcpListener)
                    asyncResult = null;

                tcpListener.Stop();
                Console.WriteLine("Accepting clients is stopped.\n");

                lock (_lock)
                {
                    stoping = true;
                    ConnectionManager.CloseAll();
                }

                Console.WriteLine("Clearing memory...\n");
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
