using SkyCrab.Connection.PresentationLayer;
using System.Net.Sockets;

namespace SkyCrab.Connection.AplicationLayer
{
    class ClientConnection : ClientEncryptedConnection
    {

        public ClientConnection(string host, int readTimeout) :
            base(new TcpClient(host, PORT), readTimeout)
        {
        }

    }
}
