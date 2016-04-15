using SkyCrab.Connection.PresentationLayer;
using System.Net.Sockets;

namespace SkyCrab.Connection.AplicationLayer
{
    abstract class AbstractServerConnection : ServerEncryptedConnection
    {

        public AbstractServerConnection(TcpClient tcpClient, int readTimeout) :
            base(tcpClient, readTimeout)
        {
        }

    }
}
