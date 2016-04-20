using SkyCrab.Connection.PresentationLayer;
using System.Net;
using System.Net.Sockets;

namespace SkyCrab.Connection.AplicationLayer
{
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
