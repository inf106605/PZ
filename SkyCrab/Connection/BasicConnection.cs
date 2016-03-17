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
        private int readTimeout;


        protected BasicConnection(TcpClient tcpClient, int readTimeout)
        {
            this.tcpClient = tcpClient;
            this.readTimeout = readTimeout;
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
                var asyncResult = stream.BeginRead(bytes, offset, size-offset, null, null);
                WaitHandle waitHandle = asyncResult.AsyncWaitHandle;
                waitHandle.WaitOne(readTimeout); //TODO timeout for reading ALL bytes
                offset += (UInt16)stream.EndRead(asyncResult);
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
