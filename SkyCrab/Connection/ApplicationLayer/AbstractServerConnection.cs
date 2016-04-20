using SkyCrab.Connection.PresentationLayer;
using SkyCrab.Connection.PresentationLayer.Messages;
using System.Net.Sockets;

namespace SkyCrab.Connection.AplicationLayer
{
    /// <summary>
    /// Messages to handle: <see cref="MessageId.LOGIN"/>, <see cref="MessageId.LOGOUT"/>, <see cref="MessageId.REGISTER"/>, <see cref="MessageId.EDIT_PROFILE"/>, <see cref="MessageId.DISCONNECT"/>
    /// </summary>
    abstract class AbstractServerConnection : ServerEncryptedConnection
    {

        public AbstractServerConnection(TcpClient tcpClient, int readTimeout) :
            base(tcpClient, readTimeout)
        {
        }

    }
}
