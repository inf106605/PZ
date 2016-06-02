using SkyCrab.Common_classes;
using SkyCrab.Common_classes.Chats;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Rooms;
using SkyCrabServer.ServerLogics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SkyCrabServer.Consoles
{
    //TODO hide prompt after close
    sealed class ServerConsole : IDisposable
    {

        private delegate bool CommandAction(string param);


        private const string COMMAND_PROMPT = "> ";
        private static readonly Dictionary<string, CommandAction> commands = new Dictionary<string, CommandAction>();
        private Task task;
        private object _writeLock = new object();
        private object _inputLock = new object();
        private volatile string inputString = "";
        private volatile int lockCounter = 0;
        private volatile bool closed = false;
        private volatile bool disposing = false;


        static ServerConsole()
        {
            commands.Add("HELP", Help);
            commands.Add("SAY", Say);
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
            while (true)
            {
                GetCommand();
                if (disposing)
                    break;
                InterpretCommand();
                if (closed)
                    break;
            }
        }

        private void GetCommand()
        {
            while (true)
            {
                while (!Console.KeyAvailable)
                {
                    if (disposing)
                        return;
                    Thread.Sleep(50);
                }
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
            char[] otherAllowedChars = new char[] { ' ', ',', '.', '!', '?', '+', '-', '_', '(', ')' };
            if ((character >= 'A' && character <= 'Z') ||
                (character >= 'a' && character <= 'z') ||
                (character >= '0' && character <= '9'))
                return true;
            foreach (char allowedChar in otherAllowedChars)
                if (character == allowedChar)
                    return true;
            return false;
        }

        private void InterpretCommand()
        {
            try
            {
                Lock();
                Console.WriteLine('\r' + COMMAND_PROMPT + inputString);
                string[] splittedInputString = inputString.Split(new char[] { ' ' }, 2);
                if (splittedInputString.Length == 1)
                    splittedInputString = new string[] { splittedInputString[0], "" };
                splittedInputString[0] = splittedInputString[0].ToUpper();
                inputString = "";
                CommandAction commandAction;
                if (commands.TryGetValue(splittedInputString[0], out commandAction))
                {
                    if (commandAction.Invoke(splittedInputString[1]))
                        closed = true;
                }
                else
                {
                    Console.WriteLine("Unknown command!");
                    Console.WriteLine("Type 'HELP' for more info.");
                }
            }
            finally
            {
                Unlock();
            }
        }

        public void Lock()
        {
            Monitor.Enter(_writeLock);
            if (lockCounter++ == 0 && !closed)
                Console.Write('\r' + new string(' ', COMMAND_PROMPT.Length) + '\r');
        }

        public void Unlock()
        {
            if (--lockCounter == 0)
            {
                if (closed)
                {
                    if (!disposing)
                        Console.WriteLine();
                }
                else
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

        private static bool Help(string param)
        {
            Console.WriteLine("-------------------------------------HELP--------------------------------------");
            Console.WriteLine("Available commands:");
            Console.WriteLine("\t'HELP'\tShow this message.");
            Console.WriteLine("\t'SAY'\tSend a message to all users.");
            Console.WriteLine("\t'STOP'\tStop server and exit the program.\n\t\t(Alternative forms: 'EXIT', 'CLOSE'.)");
            Console.WriteLine("-------------------------------------HELP--------------------------------------");
            return false;
        }

        private static bool Say(string param)
        {
            if (LengthLimit.ChatMessage.Check(param) != LengthLimit.Result.OK)
            {
                Console.WriteLine("Incorrect length of message!");
                return false;
            }
            ChatMessage chatMessage = new ChatMessage();
            chatMessage.PlayerId = 0;
            chatMessage.Message = param;
            Globals.dataLock.AcquireReaderLock(-1);
            try
            {
                foreach (ServerPlayer serverPlayer in Globals.players.Values)
                    ChatMsg.AsyncPost(serverPlayer.connection, chatMessage, null);
            }
            finally
            {
                Globals.dataLock.ReleaseReaderLock();
            }
            Console.WriteLine("Message is sended.");
            return false;
        }

        private static bool Exit(string param)
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
            StopReadingThread();
            Lock();
            task.Dispose();
        }

        private void StopReadingThread()
        {
            Lock();
            closed = true;
            disposing = true;
            Unlock();
            task.Wait();
        }

    }
}
