using SkyCrab.Connection.PresentationLayer;
using System.Net.Sockets;

namespace SkyCrab.Connection.AplicationLayer
{
    abstract class AbstractClientConnection : ClientEncryptedConnection
    {

        public AbstractClientConnection(string host, int readTimeout) :
            base(new TcpClient(host, PORT), readTimeout)
        {
        }

    }
}
