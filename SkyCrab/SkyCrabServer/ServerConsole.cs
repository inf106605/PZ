using System;
using System.Collections.Generic;

namespace SkyCrabServer
{
    class ServerConsole
    {

        private delegate bool CommandAction();


        private static readonly Dictionary<string, CommandAction> commands = new Dictionary<string, CommandAction>();


        static ServerConsole()
        {
            commands.Add("help", Help);
            commands.Add("exit", Exit);
            commands.Add("close", Exit);
            commands.Add("stop", Exit);
        }


        public void Start()
        {
            while (true)
            {
                string commandName = Console.ReadLine();
                commandName = commandName.ToLower();
                CommandAction commandAction;
                if (commands.TryGetValue(commandName, out commandAction))
                {
                    if (commandAction.Invoke())
                        break;
                }
                else
                {
                    Console.WriteLine("Unknown command!");
                    Console.WriteLine("Type 'help' for more info.\n");
                }
            }
        }

        private static bool Help()
        {
            Console.WriteLine("-------------------------------------HELP--------------------------------------");
            Console.WriteLine("Available commands:");
            Console.WriteLine("\t'HELP'\tShow this message.");
            Console.WriteLine("\t'STOP'\tStop server and exit the program.\n\t\t(Alternative forms: 'EXIT', 'CLOSE'.)");
            Console.WriteLine("-------------------------------------HELP--------------------------------------\n");
            return false;
        }

        private static bool Exit()
        {
            Console.WriteLine("Stopping the server...\n");
            return true;
        }

    }
}
