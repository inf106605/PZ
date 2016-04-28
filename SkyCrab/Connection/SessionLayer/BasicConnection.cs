using SkyCrab.Connection.Utils;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace SkyCrab.Connection.SessionLayer
{
    public class ReadTimeoutException : SkyCrabConnectionException
    {
    }

    //TODO error handling
    public abstract class BasicConnection : IDisposable
    {

        public const int PORT = 8888;

        private TcpClient tcpClient;


        public IPEndPoint LocalEndPoint
        {
            get { return (IPEndPoint)tcpClient.Client.LocalEndPoint; }
        }

        public IPEndPoint RemoteEndPoint
        {
            get { return (IPEndPoint)tcpClient.Client.RemoteEndPoint; }
        }


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
                UInt16 readedBytes;
                try
                {
                    readedBytes = (UInt16)stream.Read(bytes, offset, size - offset);
                }
                catch (IOException ex)
                {
                    var socketExept = ex.InnerException as SocketException;
                    if (socketExept != null && socketExept.ErrorCode == 10060)
                        throw new ReadTimeoutException();
                    else
                        throw ex;
                }
                if (readedBytes == 0)
                    throw new ReadTimeoutException();
                offset += readedBytes;
            } while (offset != size);
            return bytes;
        }

        public virtual void Dispose()
        {
            if (tcpClient != null)
            {
                tcpClient.GetStream().Close();
                tcpClient.Close();
                tcpClient = null;
            }
        }

    }
}
