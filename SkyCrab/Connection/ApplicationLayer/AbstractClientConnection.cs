﻿using SkyCrab.Connection.PresentationLayer;
using SkyCrab.Connection.PresentationLayer.Messages;
using System.Net.Sockets;

namespace SkyCrab.Connection.AplicationLayer
{
    /// <summary>
    /// Messages to handle: <see cref="MessageId.DISCONNECT"/>
    /// </summary>
    abstract class AbstractClientConnection : ClientEncryptedConnection
    {

        public AbstractClientConnection(string host, int readTimeout) :
            base(new TcpClient(host, PORT), readTimeout)
        {
        }

    }
}
