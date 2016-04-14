using SkyCrab.Connection.Utils;
using System;
using System.IO;
using System.Net.Sockets;

namespace SkyCrab.Connection.SessionLayer
{
    public class ReadTimeoutException : SkyCrabConnectionException
    {
    }

    //TODO error handling
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
            NetworkStream stream;
            try
            {
                stream = tcpClient.GetStream();
            }catch (InvalidOperationException)
            {
                throw new ReadTimeoutException(); //TODO EVEN WRONGER THAT BELOW! Repair this
            }
            byte[] bytes = new byte[size];
            UInt16 offset = 0;
            do
            {
                UInt16 readedBytes;
                try
                {
                    readedBytes = (UInt16)stream.Read(bytes, offset, size - offset);
                }catch (IOException)
                {
                    throw new ReadTimeoutException(); //TODO WRONG! Repair this
                }
                if (readedBytes == 0)
                    throw new ReadTimeoutException();
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
