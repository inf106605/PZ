using SkyCrab.Connection.PresentationLayer;
using SkyCrab.Connection.PresentationLayer.Messages;
using System.Net;
using System.Net.Sockets;

namespace SkyCrab.Connection.AplicationLayer
{
    /// <summary>
    /// Messages to handle: <see cref="MessageId.DISCONNECT"/>, <see cref="MessageId.NO_PONG"/>, <see cref="MessageId.LOGIN"/>, <see cref="MessageId.LOGOUT"/>, <see cref="MessageId.REGISTER"/>, <see cref="MessageId.EDIT_PROFILE"/>
    /// </summary>
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
