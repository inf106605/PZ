using System;
using System.Collections.Generic;
using System.Threading;

namespace SkyCrabServer
{
    //TODO use this all the time?
    class ServerConsole
    {

        private delegate bool CommandAction();


        private const string COMMAND_PROMPT = "> ";
        private static readonly Dictionary<string, CommandAction> commands = new Dictionary<string, CommandAction>();

        private object _writeLock = new object();
        private object _inputLock = new object();
        private volatile string inputString = "";


        static ServerConsole()
        {
            commands.Add("HELP", Help);
            commands.Add("EXIT", Exit);
            commands.Add("CLOSE", Exit);
            commands.Add("STOP", Exit);
        }


        public void Start()
        {
            Console.Write(COMMAND_PROMPT);
            while (true)
            {
                GetCommand();
                if (!InterpretCommand())
                    break;
            }
        }

        private void GetCommand()
        {
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Enter)
                    break;
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    lock (_inputLock)
                    {
                        if (Monitor.TryEnter(_writeLock))
                        {
                            Console.Write("\b \b");
                            Monitor.Exit(_writeLock);
                        }
                        inputString = inputString.Substring(0, inputString.Length - 1);
                    }
                }
                else if (IsAllowerChar(keyInfo.KeyChar))
                {
                    lock (_inputLock)
                    {
                        if (Monitor.TryEnter(_writeLock))
                        {
                            Console.Write(keyInfo.KeyChar);
                            Monitor.Exit(_writeLock);
                        }
                        inputString += keyInfo.KeyChar;
                    }
                }
            }
        }

        private bool IsAllowerChar(char character)
        {
            return (character >= 'A' && character <= 'Z') ||
                (character >= 'a' && character <= 'z');
        }

        private bool InterpretCommand()
        {
            lock (_writeLock)
            {
                Console.WriteLine('\r' + COMMAND_PROMPT + inputString);
                string upperInputString = inputString.ToUpper();
                inputString = "";
                CommandAction commandAction;
                if (commands.TryGetValue(upperInputString, out commandAction))
                {
                    if (commandAction.Invoke())
                        return false;
                }
                else
                {
                    Console.WriteLine("Unknown command!");
                    Console.WriteLine("Type 'HELP' for more info.\n");
                }
                Console.Write(COMMAND_PROMPT);
            }
            return true;
        }

        public void Write(string text)
        {
            lock (_writeLock)
            {
                Console.Write('\r' + new string(' ', COMMAND_PROMPT.Length) + '\r');
                Console.WriteLine(text);
                Console.Write(COMMAND_PROMPT);
                lock (_inputLock)
                    Console.Write(inputString);
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
