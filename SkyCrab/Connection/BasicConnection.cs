using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SkyCrab.connection
{
    abstract class BasicConnection
    {
        public const int PORT = 8888;

        private TcpClient tcpClient;


        protected BasicConnection(TcpClient tcpClient, int readTimeout)
        {
            this.tcpClient = tcpClient;
            tcpClient.ReceiveTimeout = readTimeout;
        }

        protected void WriteBytes(byte[] bytes)
        {
            NetworkStream stream = tcpClient.GetStream();
            stream.Write(bytes, 0, bytes.Length);
        }

        protected byte[] ReadBytes(UInt16 size)
        {
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
            return bytes;
        }

        public void Close()
        {
            tcpClient.GetStream().Close();
            tcpClient.Close();
            tcpClient = null;
        }

    }
}
