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
                    SkyCrab_Server.serverConsole.Lock();
                    ServerConnection connection = new ServerConnection(tcpClient, 100); //TODO remove constant
                    SkyCrab_Server.serverConsole.WriteLine("New client connected. (" + connection.ServerEndPoint.Address + ", " + connection.ClientAuthority + ")");
                    connections.Add(connection);
                    connection.AddConnectionCloseListener((disconectedConnection, exceptions) => OnCloseConnection((ServerConnection) disconectedConnection, exceptions));
                }
                catch (Exception e)
                {
                    SkyCrab_Server.serverConsole.Write(e.ToString(), Console.Error);
                    SkyCrab_Server.serverConsole.WriteLine("Cannot initialize connection with client!", Console.Error);
                }
                finally
                {
                    SkyCrab_Server.serverConsole.Unlock();
                }
            }
        }

        private static void OnCloseConnection(ServerConnection disconectedConnection, AggregateException exceptions)
        {
            if (exceptions != null)
                SkyCrab_Server.serverConsole.Write(exceptions.ToString(), Console.Error);
            lock (connections)
                connections.Remove((ServerConnection) disconectedConnection);
        }

        public static void Close(ServerConnection connection)
        {
            connection.Dispose();
        }

        public static void CloseAll()
        {
            SkyCrab_Server.serverConsole.WriteLine("Closing connections with clients...");
            List<ServerConnection> connectionsCopy;
            lock (connections)
                connectionsCopy = new List<ServerConnection>(connections);
            foreach (ServerConnection connection in connectionsCopy)
                connection.Dispose();
        }

    }
}
