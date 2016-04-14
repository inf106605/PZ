using SkyCrab.Connection.AplicationLayer;
using System;
using System.Net.Sockets;

namespace SkyCrabServer
{
    sealed class UnsuportedMessageException : SkyCrabServerException
    {
    }

    class ServerConnection : AbstractServerConnection
    {

        public ServerConnection(TcpClient tcpClient, int readTimeout) :
            base(tcpClient, readTimeout)
        {
        }

        protected override void ProcessMessages()
        {
            foreach (MessageInfo messageInfo in messages.GetConsumingEnumerable())
            {
                processingMessagesOk = true;
                switch (messageInfo.messageId)
                {
                    //TODO
                    default:
                        throw new UnsuportedMessageException();
                }
            }
            Console.WriteLine("Client disconnected.\n"); //TODO more info
        }

    }
}
