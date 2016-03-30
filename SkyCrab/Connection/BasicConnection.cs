using System;
using System.Net.Sockets;

namespace SkyCrab.Connection
{
    internal abstract class BasicConnection : IDisposable
    {

        public const int PORT = 8888;

        private TcpClient tcpClient;


        protected BasicConnection(TcpClient tcpClient, int readTimeout)
        {
            this.tcpClient = tcpClient;
            tcpClient.ReceiveTimeout = readTimeout;
        }

        protected virtual void WriteBytes(byte[] bytes)
        {
            NetworkStream stream = tcpClient.GetStream();
            stream.Write(bytes, 0, bytes.Length);
        }

        protected virtual byte[] ReadBytes(UInt16 size)
        {
            NetworkStream stream = tcpClient.GetStream();
            byte[] bytes = new byte[size];
            UInt16 offset = 0;
            do
            {
                UInt16 readedBytes = (UInt16)stream.Read(bytes, offset, size - offset);
                if (readedBytes == 0)
                    throw new SkyCrabConnectionException("Cannot read any more bytes from socket!");
                offset += readedBytes;
            } while (offset != size);
            return bytes;
        }

        public void Close()
        {
            if (tcpClient != null)
            {
                tcpClient.GetStream().Close();
                tcpClient.Close();
            }
        }

        public virtual void Dispose()
        {
            Close();
            tcpClient = null;
        }

    }
}
