using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace SkyCrab.connection
{
    class ClientConnection : Connection
    {

        public ClientConnection(string host) :
            base(new TcpClient(host, PORT))
        {
        }

    }
}
