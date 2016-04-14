using SkyCrab.Connection.AplicationLayer;
using System.Net.Sockets;
using System;

namespace DummyServer
{
    internal class ServerConnection : AbstractServerConnection
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
                    default:
                        throw new Exception("Unknown message!!!!!!!!!!!!!!!!1!11111111111111oneone");
                }
            }
        }

    }
}
