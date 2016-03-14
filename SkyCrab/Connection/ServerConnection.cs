using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace SkyCrab.connection
{
    class ServerConnection : Connection
    {

        public ServerConnection(TcpClient tcpClient) :
            base(tcpClient)
        {
        }

    }
}
