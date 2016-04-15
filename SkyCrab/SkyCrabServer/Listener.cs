using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SkyCrabServer
{
    class Listener
    {

        private static List<ServerConnection> connections = new List<ServerConnection>();


        public static bool Listen(IPAddress ipAddress, int port)
        {
            Console.WriteLine("Initializing the server...\n\tAddress: " + ipAddress + "\n\tPort: " + port);
            InitializeServer();
            Console.WriteLine("\tDONE!\n");

            try
            {
                TcpListener tcpListener = new TcpListener(ipAddress, port);
                tcpListener.Start();
                //TODO while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
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
                Console.Out.WriteLine("Press Enter to close a sever");
                Console.In.ReadLine();
                tcpListener.Stop();

                Console.WriteLine("Closing connections with clients...\n");
                foreach (ServerConnection serverConnection in connections)
                    serverConnection.Dispose();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return false;
            }

            Console.WriteLine("Stoping the server...");
            StopServer();
            Console.WriteLine("\tDONE!\n");

            return true;
        }

        private static void InitializeServer()
        {
            ServerConnection.Inicjalize();
        }

        private static void StopServer()
        {
            ServerConnection.Deinicjalize();
        }

    }
}
