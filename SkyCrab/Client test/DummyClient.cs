using SkyCrab.connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client_test
{
    class DummyClient
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Client");
                ClientConnection.Inicjalize();

                DataConnection connection = new ClientConnection("localhost", 100);
                connection.WriteString("Hello World!");
                UInt32 aaa = connection.ReadUInt32();
                Console.WriteLine(aaa);
                Thread.Sleep(1000);
                connection.Close();

                ClientConnection.Deinicjalize();
                Console.WriteLine("OK");
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
