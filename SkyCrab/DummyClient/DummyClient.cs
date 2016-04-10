using SkyCrab.Connection.AplicationLayer;
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

                using (ClientConnection connection = new ClientConnection("localhost", 100))
                {
                    {
                        object writingBlock = connection.BeginWritingBlock();
                        connection.AsyncWriteData(ClientConnection.stringTranscoder, writingBlock, "Hello World!");
                        connection.EndWritingBlock(writingBlock);
                    }
                    UInt32 aaa;
                    {
                        connection.BeginReadingBlock();
                        aaa = connection.SyncReadData(ClientConnection.uint32Transcoder);
                        connection.EndReadingBlock();
                    }
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
