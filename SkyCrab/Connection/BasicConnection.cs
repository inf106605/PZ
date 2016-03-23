using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SkyCrab.connection
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
            #if DEBUG
            Console.Write("<< " + bytes.Length + " bytes");
            #endif
            NetworkStream stream = tcpClient.GetStream();
            stream.Write(bytes, 0, bytes.Length);
            #if DEBUG
            Console.WriteLine(" [DONE]");
            #endif
        }

        protected virtual byte[] ReadBytes(UInt16 size)
        {
            #if DEBUG
            Console.Write(">> " + size + " bytes");
            #endif
            NetworkStream stream = tcpClient.GetStream();
            byte[] bytes = new byte[size];
            UInt16 offset = 0;
            do
            {
                UInt16 readedBytes = (UInt16)stream.Read(bytes, offset, size - offset);
                if (readedBytes == 0)
                    throw new Exception("Cannot read any more bytes from socket!");
                offset += readedBytes;
            } while (offset != size);
            #if DEBUG
            Console.WriteLine(" [DONE]");
            #endif
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
