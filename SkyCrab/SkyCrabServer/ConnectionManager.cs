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
                    Listener.serverConsole.Write("New client connected. (" + connection.ServerEndPoint.Address + ", " + connection.ClientAuthority + ")\n");
                    connections.Add(connection);
                    connection.addConnectionCloseListener((disconectedConnection, exceptions) => OnCloseConnection((ServerConnection) disconectedConnection, exceptions));
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                    Console.Error.WriteLine("Cannot initialize connection with client!\n");
                }
            }
        }

        private static void OnCloseConnection(ServerConnection disconectedConnection, AggregateException exceptions)
        {
            if (exceptions != null)
                Console.WriteLine(exceptions.ToString());
            lock (connections)
                connections.Remove((ServerConnection) disconectedConnection);
        }

        public static void Close(ServerConnection connection)
        {
            connection.Dispose();
        }

        public static void CloseAll()
        {
            Console.WriteLine("Closing connections with clients...\n");
            List<ServerConnection> connectionsCopy;
            lock (connections)
                connectionsCopy = new List<ServerConnection>(connections);
            foreach (ServerConnection connection in connectionsCopy)
                connection.Dispose();
        }

    }
}
