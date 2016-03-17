using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace SkyCrab.connection
{
    class ServerConnection : DataConnection
    {

        public ServerConnection(TcpClient tcpClient, int readTimeout) :
            base(tcpClient, readTimeout)
        {
        }

    }
}
