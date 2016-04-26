﻿using SkyCrab.Connection.PresentationLayer;
using SkyCrab.Connection.PresentationLayer.Messages;
using System.Net;
using System.Net.Sockets;

namespace SkyCrab.Connection.AplicationLayer
{
    /// <summary>
    /// Messages to handle: <see cref="MessageId.DISCONNECT"/>, <see cref="MessageId.NO_PONG"/>
    /// </summary>
    abstract class AbstractClientConnection : ClientEncryptedConnection
    {

        public IPEndPoint ClientEndPoint
        {
            get { return LocalEndPoint; }
        }

        public IPEndPoint ServerEndPoint
        {
            get { return RemoteEndPoint; }
        }


        public AbstractClientConnection(string host, int readTimeout) :
            base(new TcpClient(host, PORT), readTimeout)
        {
        }

    }
}
