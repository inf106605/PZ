using System;
using System.Net;
using System.Net.Sockets;

namespace SkyCrabServer.Connactions
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
                Globals.serverConsole.WriteLine("Initializing the server at " + ipAddress + ":" + port + "...");

                Globals.serverConsole.WriteLine("Generating/Loading criptographic keys...");
                ServerConnection.PreLoadStaticMembers();

                try
                {
                    tcpListener = new TcpListener(ipAddress, port);
                    tcpListener.Start();
                }
                catch (SocketException e)
                {
                    if (e.HResult == -2147467259)
                        Globals.serverConsole.WriteLine("Port " + port + " is already occupied!", Console.Error);
                    else
                        Globals.serverConsole.WriteLine("Cannot start listening!\n" + e.ToString(), Console.Error);
                    return false;
                }

                asyncResult = tcpListener.BeginAcceptTcpClient(ClientAccepter, null);
                Globals.serverConsole.WriteLine("Accepting clients is begun.");

                Globals.serverConsole.Wait();

                lock (tcpListener)
                    asyncResult = null;
            }
            catch (Exception e)
            {
                Globals.serverConsole.WriteLine(e.ToString(), Console.Error);
                return false;
            }
            finally
            {
                tcpListener.Stop();
                Globals.serverConsole.WriteLine("Accepting clients is stopped.");

                lock (_lock)
                {
                    stoping = true;
                    ConnectionManager.CloseAll();
                }

                Globals.serverConsole.WriteLine("Clearing memory...");
                ServerConnection.DisposeStaticMembers();
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
