using SkyCrab.Connection.Utils;
using System;
using System.Net.Sockets;

namespace SkyCrab.Connection.PresentationLayer
{
    public class SkyCrabConnectionProtocolVersionException : SkyCrabConnectionException
    {

        public SkyCrabConnectionProtocolVersionException(String message) :
            base(message)
        {
        }

    }

    internal abstract class MessageConnection : DataConnection
    {

        private static readonly byte[] version = new byte[3] { 0, 0, 0 };


        protected MessageConnection(TcpClient tcpClient, int readTimeout) :
            base(tcpClient, readTimeout)
        {
            CheckVersion();
        }

        private void CheckVersion()
        {
            object writingBlock = BeginWritingBlock();
            AsyncWriteBytes(writingBlock, version);
            EndWritingBlock(writingBlock);
            BeginReadingBlock();
            byte[] otherVersion = SyncReadBytes(3);
            EndReadingBlock();
            if (version[0] > otherVersion[0])
                throw new SkyCrabConnectionProtocolVersionException("The other side of the connection has too old version of the protocol!");
            else if (version[0] < otherVersion[0])
                throw new SkyCrabConnectionProtocolVersionException("The other side of the connection has too new version of the protocol!");
            else if (version[1] > otherVersion[1])
                Console.WriteLine("The other side of the connection has an older version of the protocol. It should be updated.");
            else if (version[1] < otherVersion[1])
                Console.WriteLine("The other side of the connection has a newer version of the protocol. Update is recomended.");
            else if (version[2] != otherVersion[2])
                Console.WriteLine("The other side of the connection has another version of the protocol but it should work.");
        }

    }
}
