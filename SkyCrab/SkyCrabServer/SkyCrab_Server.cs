using SkyCrab.Dictionaries;
using SkyCrabServer.Connactions;
using SkyCrabServer.Consoles;
using SkyCrabServer.Databases;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace SkyCrabServer
{
    class SkyCrab_Server
    {

        private static readonly Version version = new Version(1, 3, 1);


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

            FileStream lockFile;
            if (!CreateLockFile(out lockFile))
                return -1;
            using (lockFile)
            {
                try
                {
                    using (Globals.serverConsole = new ServerConsole())
                    using (Globals.database = new Database())
                    {
                        LoadDictionary();                        
                        bool result = Listener.Listen(ipAddress, port);
                        if (result)
                            return 0;
                        else
                            return -1;
                    }
                }
                catch (DatabaseVersionIsIncorrectException e)
                {
                    if (e.currentDatabaseVersion == null)
                        Console.Error.WriteLine("Incorrect database version!\nVersion '" + Database.VERSION + "' is expected.");
                    else
                        Console.Error.WriteLine("Incorrect database version '" + e.currentDatabaseVersion + "'!\nVersion '" + Database.VERSION + "' is expected.");
                    Console.WriteLine();
                    return -1;
                }
                catch (SocketException e)
                {
                    if (e.ErrorCode == -2147467259)
                        Console.Error.WriteLine("Port " + port + " is already occupied!");
                    else
                        Console.Error.WriteLine("Cannot start listening!");
                    Console.WriteLine();
                    return -1;
                }
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine("This application is a server for online game 'SkyCrab'.");
            Console.WriteLine();
            Console.WriteLine("Usage: SkyCrab_Server [ADDRESS] [PORT]");
            Console.WriteLine("\tADDRESS\tAn address or a domain to which the server will listen\n\t\tfor new connections.\n\t\tIf it is not given, the server will listen to all adresses.");
            Console.WriteLine("\tPORT\tA port to which the server will listen for new connections.\n\t\tIf it is not given, the server will use the default port.\n\t\t(Using the default port is recomended.)");
            Console.WriteLine();
        }

        private static bool CreateLockFile(out FileStream fileStream)
        {
            const string LOCK_FILE_NAME = "SkyCrab_server.lock";
            try
            {
                fileStream = new FileStream(LOCK_FILE_NAME, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, 8, FileOptions.DeleteOnClose);
                return true;
            }
            catch (IOException)
            {
                Console.Error.WriteLine("Only one instance of the program can be opened in the same directory!");
                fileStream = null;
                return false;
            }
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

        private static void LoadDictionary()
        {
            Globals.serverConsole.Lock();
            try
            {
                Globals.serverConsole.WriteLine("Loading dictionary...");
                Globals.dictionary = new Dictionary(Dictionary.Language.POLISH);
            }
            finally
            {
                Globals.serverConsole.Unlock();
            }
        }

    }
}
