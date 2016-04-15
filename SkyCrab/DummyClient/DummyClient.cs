using SkyCrab.Connection.PresentationLayer.Messages.Menu;
using System;

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

                using (ClientConnection connection = new ClientConnection("localhost", 100))
                {
                    Ok.PostOk(connection);
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
