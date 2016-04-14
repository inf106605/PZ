using System;
using System.Net;

namespace SkyCrabServer
{
    class Program
    {

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
                Console.Error.WriteLine("Write 'SkyCrabServer --help' for more informations.");
                Console.Error.WriteLine();
                return -1;
            }

            if (MainLoop(ipAddress, port))
                return 0;
            else
                return -1;
        }

        private static void ShowHelp()
        {
            Console.WriteLine("This application is a server for online game 'SkyCrab'.");
            Console.WriteLine();
            Console.WriteLine("Usage: SkyCrabServer [ADDRESS] [PORT]");
            Console.WriteLine("\tADDRESS\tAn address or a domain to which the server will listen\n\t\tfor new connections.\n\t\tIf it is not given, the server will listen to all adresses.");
            Console.WriteLine("\tPORT\tA port to which the server will listen for new connections.\n\t\tIf it is not given, the server will use default port.\n\t\t(Using the default port is recomended.)");
            Console.WriteLine();
        }

        private static bool MainLoop(IPAddress ipAddress, int port)
        {
            Console.WriteLine("Initializing the server...");
            Console.WriteLine("\tAddress: "+ipAddress.ToString());
            Console.WriteLine("\tPort: "+port);
            InitializeServer();
            Console.WriteLine("\tDONE!\n");

            try
            {
                
            }catch (Exception e)
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
