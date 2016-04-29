using System;
using System.Net;

namespace SkyCrabServer
{
    class SkyCrab_Server
    {

        private static readonly Version version = new Version(0, 1, 6);


        static int Main(string[] args)
        {
            if (args.Length != 0 && args[0] == "--help")
            {
                PrintHelp();
                return 0;
            }

            IPAddress ipAddress;
            int port;
            if (!GetAddressAndPort(args, out ipAddress, out port))
                return -1;

            Banners.Banner.PrintBanner(version);
            if (Listener.Listen(ipAddress, port))
                return 0;
            else
                return -1;
        }

        private static void PrintHelp()
        {
            Console.WriteLine("This application is a server for online game 'SkyCrab'.");
            Console.WriteLine();
            Console.WriteLine("Usage: SkyCrab_Server [ADDRESS] [PORT]");
            Console.WriteLine("\tADDRESS\tAn address or a domain to which the server will listen\n\t\tfor new connections.\n\t\tIf it is not given, the server will listen to all adresses.");
            Console.WriteLine("\tPORT\tA port to which the server will listen for new connections.\n\t\tIf it is not given, the server will use default port.\n\t\t(Using the default port is recomended.)");
            Console.WriteLine();
        }

        private static bool GetAddressAndPort(string[] args, out IPAddress ipAddress, out int port)
        {
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
                    }
                    else
                        ipAddress = IPAddress.Any;
                }
                catch (Exception e)
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
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Cannot parse port!");
                    throw e;
                }
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Write 'SkyCrab_Server --help' for more informations.");
                Console.Error.WriteLine();
                ipAddress = null;
                port = 0;
                return false;
            }
            return true;
        }

    }
}
