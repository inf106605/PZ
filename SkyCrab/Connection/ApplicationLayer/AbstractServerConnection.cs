using SkyCrab.Connection.PresentationLayer;
using SkyCrab.Connection.PresentationLayer.Messages;
using System.Net;
using System.Net.Sockets;

namespace SkyCrab.Connection.AplicationLayer
{
    abstract class AbstractServerConnection : ServerEncryptedConnection
    {

        public override IPEndPoint ClientEndPoint
        {
            get { return RemoteEndPoint; }
        }

        public override IPEndPoint ServerEndPoint
        {
            get { return LocalEndPoint; }
        }


        public AbstractServerConnection(TcpClient tcpClient, int readTimeout) :
            base(tcpClient, readTimeout)
        {
        }

    }
}
