using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace SkyCrab.connection
{
    class ClientConnection : DataConnection
    {

        public ClientConnection(string host, int readTimeout) :
            base(new TcpClient(host, PORT), readTimeout)
        {
        }

    }
}
