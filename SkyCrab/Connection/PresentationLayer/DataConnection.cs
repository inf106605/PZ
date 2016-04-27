using SkyCrab.Connection.PresentationLayer.DataTranscoders;
using System.Net.Sockets;

namespace SkyCrab.Connection.PresentationLayer
{
    public abstract class DataConnection : EncryptedConnection
    {

        protected DataConnection(TcpClient tcpClient, int readTimeout) :
            base(tcpClient, readTimeout)
        {
        }

        internal T SyncReadData<T>(ITranscoder<T> transcoder)
        {
            T data = transcoder.Read(this);
            return data;
        }

        internal void AsyncWriteData<T>(ITranscoder<T> transcoder, object writingBlock, T data)
        {
            transcoder.Write(this, writingBlock, data);
        }

    }
}
