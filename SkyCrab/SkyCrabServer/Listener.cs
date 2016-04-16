using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SkyCrabServer
{
    class Listener
    {

        private static List<ServerConnection> connections = new List<ServerConnection>();
        private static TcpListener tcpListener;
        private static volatile IAsyncResult asyncResult;
        private static volatile bool stoping = false;


        public static bool Listen(IPAddress ipAddress, int port)
        {
            try
            {
                Console.WriteLine("Initializing the server...\n\tAddress: " + ipAddress + "\n\tPort: " + port);
                ServerConnection.PreLoadStaticMembers();
                Console.WriteLine("\tDONE!\n");

                tcpListener = new TcpListener(ipAddress, port);
                tcpListener.Start();

                asyncResult = tcpListener.BeginAcceptTcpClient(ClientAccepter, null);
                Console.WriteLine("Accepting clients is begun.\n");

                Console.WriteLine("Press Enter to close a sever...\n");
                Console.ReadLine();

                lock (tcpListener)
                    asyncResult = null;

                tcpListener.Stop();
                Console.WriteLine("Accepting clients is stopped.\n");

                Console.WriteLine("Closing connections with clients...\n");
                lock (connections)
                {
                    stoping = true;
                    foreach (ServerConnection serverConnection in connections)
                        serverConnection.Dispose();
                }
                Console.WriteLine("Stoping the server...");
                ServerConnection.DisposeStaticMembers();
                Console.WriteLine("\tDONE!\n");
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
            lock (connections)
            {
                if (stoping)
                {
                    tcpClient.Close();
                    return;
                }
                {
                    IPEndPoint clientEndPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
                    Console.WriteLine("New client connected.\n\tAddress: " + clientEndPoint.Address + "\n\tport: " + clientEndPoint.Port + "\n");
                }
                try
                {
                    ServerConnection serverConnection = new ServerConnection(tcpClient, 100); //TODO remove constant
                    connections.Add(serverConnection);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                    Console.Error.WriteLine("Cannot initialize connection with client!\n");
                }
            }
        }

    }
}
