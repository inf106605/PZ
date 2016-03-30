using SkyCrab.Connection;
using System;
using System.Threading;

namespace DummyClient
{
    class DummyClient
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Client");
                ClientConnection.Inicjalize();

                using (DataConnection connection = new ClientConnection("localhost", 100))
                {
                    connection.WriteString("Hello World!");
                    UInt32 aaa = connection.ReadUInt32();
                    Console.WriteLine(aaa);
                    Thread.Sleep(1000);
                }

                ClientConnection.Deinicjalize();
                Console.WriteLine("OK");
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
