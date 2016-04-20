using SkyCrab.Connection.PresentationLayer;
using System.Net;
using System.Net.Sockets;

namespace SkyCrab.Connection.AplicationLayer
{
    abstract class AbstractServerConnection : ServerEncryptedConnection
    {

        public IPEndPoint ClientEndPoint
        {
            get { return RemoteEndPoint; }
        }

        public IPEndPoint ServerEndPoint
        {
            get { return LocalEndPoint; }
        }


        public AbstractServerConnection(TcpClient tcpClient, int readTimeout) :
            base(tcpClient, readTimeout)
        {
        }

    }
}
