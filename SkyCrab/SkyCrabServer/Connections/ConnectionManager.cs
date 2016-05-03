using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SkyCrabServer.Connactions
{
    static class ConnectionManager
    {

        private const int CONNECTION_TIMEOUT = 5000;

        private static List<ServerConnection> connections = new List<ServerConnection>();


        public static void Open(TcpClient tcpClient)
        {
            lock (connections)
            {
                try
                {
                    Globals.serverConsole.Lock();
                    ServerConnection connection = new ServerConnection(tcpClient, CONNECTION_TIMEOUT);
                    Globals.serverConsole.WriteLine("New client connected. (" + connection.ServerEndPoint.Address + ", " + connection.ClientAuthority + ")");
                    connections.Add(connection);
                    connection.AddConnectionCloseListener((disconectedConnection, exceptions) => OnCloseConnection((ServerConnection) disconectedConnection, exceptions));
                }
                catch (Exception e)
                {
                    Globals.serverConsole.Write(e.ToString(), Console.Error);
                    Globals.serverConsole.WriteLine("Cannot initialize connection with client!", Console.Error);
                }
                finally
                {
                    Globals.serverConsole.Unlock();
                }
            }
        }

        private static void OnCloseConnection(ServerConnection disconectedConnection, AggregateException exceptions)
        {
            if (exceptions != null)
                Globals.serverConsole.Write(exceptions.ToString(), Console.Error);
            lock (connections)
                connections.Remove((ServerConnection) disconectedConnection);
        }

        public static void Close(ServerConnection connection)
        {
            connection.Dispose();
        }

        public static void CloseAll()
        {
            List<ServerConnection> connectionsCopy;
            lock (connections)
                connectionsCopy = new List<ServerConnection>(connections);
            if (connectionsCopy.Count == 0)
                return;

            Globals.serverConsole.WriteLine("Closing connections with clients...");
            Task[] tasks = new Task[connectionsCopy.Count];
            for (int i = 0; i != connectionsCopy.Count; ++i)
                tasks[i] = Task.Factory.StartNew(connectionsCopy[i].Dispose);
            connectionsCopy = null;

            if (Task.WaitAll(tasks, 30000))
                return;
            Globals.serverConsole.WriteLine("It looks like connections close pretty slowly...");
            if (Task.WaitAll(tasks, 30000))
                return;
            Globals.serverConsole.WriteLine("Perhaps, one of them has deadlock...");
            if (Task.WaitAll(tasks, 30000))
                return;
            Globals.serverConsole.WriteLine("Yeah, this is almost certain now...");
            if (Task.WaitAll(tasks, 30000))
                return;
            Globals.serverConsole.WriteLine("OK, enough of waiting....");
            Globals.serverConsole.WriteLine("Closing the server by force...", Console.Error);
        }

    }
}
