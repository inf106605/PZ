using SkyCrab.Connection.AplicationLayer;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.Utils;
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
                    using (AnswerSynchronizer synchronizer = new AnswerSynchronizer())
                    {
                        Ping.PostPing(connection, 21, AnswerSynchronizer.Callback, synchronizer);
                        Console.WriteLine("Ping");
                        synchronizer.Wait();
                        if (synchronizer.Answer.HasValue)
                        {
                            ClientConnection.MessageInfo answer = synchronizer.Answer.Value;
                            Console.WriteLine("Pong: "+(byte)answer.message);
                        }
                    }
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
