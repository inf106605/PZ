using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SkyCrabServer
{
    //TODO use this all the time?
    sealed class ServerConsole : IDisposable
    {

        private delegate bool CommandAction();


        private const string COMMAND_PROMPT = "> ";
        private static readonly Dictionary<string, CommandAction> commands = new Dictionary<string, CommandAction>();
        private Task task;
        private object _writeLock = new object();
        private object _inputLock = new object();
        private volatile string inputString = "";
        private volatile int lockCounter = 0;


        static ServerConsole()
        {
            commands.Add("HELP", Help);
            commands.Add("EXIT", Exit);
            commands.Add("CLOSE", Exit);
            commands.Add("STOP", Exit);
        }


        public ServerConsole()
        {
            task = Task.Factory.StartNew(StartTaskBody, TaskCreationOptions.LongRunning);
        }

        private void StartTaskBody()
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
                    if (inputString.Length != 0)
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
            try
            {
                Lock();
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
            }
            finally
            {
                Unlock();
            }
            return true;
        }

        public void Lock()
        {
            Monitor.Enter(_writeLock);
            if (lockCounter++ == 0)
                Console.Write('\r' + new string(' ', COMMAND_PROMPT.Length) + '\r');
        }

        public void Unlock()
        {
            if (--lockCounter == 0)
            {
                Console.Write("\n" + COMMAND_PROMPT);
                lock (_inputLock)
                    Console.Write(inputString);
            }
            Monitor.Exit(_writeLock);
        }

        public void Write(string text)
        {
            Write(text, Console.Out);
        }

        public void Write(string text, TextWriter writer)
        {
            Lock();
            Console.Write(text);
            Unlock();
        }

        public void WriteLine(string text)
        {
            WriteLine(text, Console.Out);
        }

        public void WriteLine(string text, TextWriter writer)
        {
            Lock();
            Console.WriteLine(text);
            Unlock();
        }

        private static bool Help()
        {
            Console.WriteLine("-------------------------------------HELP--------------------------------------");
            Console.WriteLine("Available commands:");
            Console.WriteLine("\t'HELP'\tShow this message.");
            Console.WriteLine("\t'STOP'\tStop server and exit the program.\n\t\t(Alternative forms: 'EXIT', 'CLOSE'.)");
            Console.WriteLine("-------------------------------------HELP--------------------------------------");
            return false;
        }

        private static bool Exit()
        {
            Console.WriteLine("Stopping the server...");
            return true;
        }

        public void Wait()
        {
            task.Wait();
        }

        public void Dispose()
        {
            //TODO force closing task
            Lock();
            task.Dispose();
        }

    }
}
