using SkyCrab.Connection.PresentationLayer;
using SkyCrab.Connection.PresentationLayer.Messages;
using System.Net;
using System.Net.Sockets;

namespace SkyCrab.Connection.AplicationLayer
{
    abstract class AbstractClientConnection : ClientEncryptedConnection
    {

        public override IPEndPoint ClientEndPoint
        {
            get { return LocalEndPoint; }
        }

        public override IPEndPoint ServerEndPoint
        {
            get { return RemoteEndPoint; }
        }


        public AbstractClientConnection(string host, int readTimeout) :
            base(new TcpClient(host, PORT), readTimeout)
        {
        }

    }
}
