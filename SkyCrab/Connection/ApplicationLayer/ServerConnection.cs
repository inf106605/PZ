using SkyCrab.Connection.PresentationLayer;
using System.Net.Sockets;

namespace SkyCrab.Connection.AplicationLayer
{
    class ServerConnection : ServerEncryptedConnection
    {

        public ServerConnection(TcpClient tcpClient, int readTimeout) :
            base(tcpClient, readTimeout)
        {
        }

    }
}
