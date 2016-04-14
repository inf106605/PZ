using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SkyCrabServer
{
    class SkyCrab_Server
    {

        private static readonly Version version = new Version(0, 1, 1);

        private static List<ServerConnection> connections = new List<ServerConnection>();


        static int Main(string[] args)
        {
            if (args.Length != 0 && args[0] == "--help")
            {
                ShowHelp();
                return 0;
            }

            IPAddress ipAddress;
            int port;
            try
            {
                if (args.Length > 2)
                {
                    Console.Error.WriteLine("Wrong number of arguments!");
                    throw new Exception();
                }
                try
                {
                    if (args.Length >= 1)
                    {
                        if (!IPAddress.TryParse(args[0], out ipAddress))
                            ipAddress = Dns.GetHostEntry(args[0]).AddressList[0];
                    }else
                        ipAddress = IPAddress.Any;
                }catch (Exception e)
                {
                    Console.Error.WriteLine("Cannot parse IP address or domain!");
                    throw e;
                }
                try
                {
                    if (args.Length >= 2)
                        port = int.Parse(args[1]);
                    else
                        port = ServerConnection.PORT;
                }catch(Exception e)
                {
                    Console.Error.WriteLine("Cannot parse port!");
                    throw e;
                }
            }catch (Exception)
            {
                Console.Error.WriteLine("Write 'SkyCrab_Server --help' for more informations.");
                Console.Error.WriteLine();
                return -1;
            }

            Banners.Banner.PrintBanner(version);
            if (MainLoop(ipAddress, port))
                return 0;
            else
                return -1;
        }

        private static void ShowHelp()
        {
            Console.WriteLine("This application is a server for online game 'SkyCrab'.");
            Console.WriteLine();
            Console.WriteLine("Usage: SkyCrab_Server [ADDRESS] [PORT]");
            Console.WriteLine("\tADDRESS\tAn address or a domain to which the server will listen\n\t\tfor new connections.\n\t\tIf it is not given, the server will listen to all adresses.");
            Console.WriteLine("\tPORT\tA port to which the server will listen for new connections.\n\t\tIf it is not given, the server will use default port.\n\t\t(Using the default port is recomended.)");
            Console.WriteLine();
        }

        private static bool MainLoop(IPAddress ipAddress, int port)
        {
            Console.WriteLine("Initializing the server...\n\tAddress: "+ipAddress+"\n\tPort: "+port);
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
                    }catch (Exception e)
                    {
                        Console.Error.WriteLine(e);
                        Console.Error.WriteLine("Cannot initialize connection with client!\n");
                    }
                }
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
