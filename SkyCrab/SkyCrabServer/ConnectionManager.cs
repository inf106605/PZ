using SkyCrab.Connection.PresentationLayer.Messages;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SkyCrabServer
{
    static class ConnectionManager
    {

        private static List<ServerConnection> connections = new List<ServerConnection>();


        public static void Open(TcpClient tcpClient)
        {
            lock (connections)
            {
                try
                {
                    ServerConnection connection = new ServerConnection(tcpClient, 100); //TODO remove constant
                    Listener.serverConsole.Write("New client connected. (" + connection.ServerEndPoint.Address + ")\n\tAddress: " + connection.ClientEndPoint.Address + "\n\tport: " + connection.ClientEndPoint.Port + "\n");
                    connections.Add(connection);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                    Console.Error.WriteLine("Cannot initialize connection with client!\n");
                }
            }
        }

        public static void Close(ServerConnection connection)
        {
            lock (connections)
            {
                if (connections.Remove(connection))
                    connection.Dispose();
            }
        }

        public static void CloseAll()
        {
            lock (connections)
            {

                Console.WriteLine("Closing connections with clients...\n");
                foreach (ServerConnection connection in connections)
                {
                    DisconnectMsg.AsyncPostDisconnect(connection);
                    connection.Dispose();
                }
            }
        }

    }
}
