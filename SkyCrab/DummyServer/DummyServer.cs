using SkyCrab.Connection.AplicationLayer;
using System;
using System.Net.Sockets;
using System.Threading;

namespace DummyServer
{
    class DummyServer
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Server");
                ServerConnection.Inicjalize();

                TcpListener tcpListener = new TcpListener(ServerConnection.PORT);
                tcpListener.Start();
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                tcpListener.Stop();

                using (ServerConnection connection = new ServerConnection(tcpClient, 100))
                {
                    {
                        object writingBlock = connection.BeginWritingBlock();
                        connection.AsyncWriteData(ServerConnection.uint32Transcoder, writingBlock, (UInt32)101011101);
                        connection.EndWritingBlock(writingBlock);
                    }
                    string aaa;
                    {
                        connection.BeginReadingBlock();
                        aaa = connection.SyncReadData(ServerConnection.stringTranscoder);
                        connection.EndReadingBlock();
                    }
                    Console.WriteLine(aaa);
                    Thread.Sleep(1000);
                }

                ServerConnection.Deinicjalize();
                Console.WriteLine("OK");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
