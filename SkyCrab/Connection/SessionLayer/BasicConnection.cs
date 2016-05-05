using SkyCrab.Connection.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SkyCrab.Connection.SessionLayer
{
    public class ReadTimeoutException : SkyCrabConnectionException
    {
    }
    
    public abstract class BasicConnection : IDisposable
    {

        public const int PORT = 56077;

        private TcpClient tcpClient;
        private int readTimeout;

        protected volatile bool closing = false;
        protected volatile bool disposed = false;
        private readonly object _disposeLock = new object();

        public delegate void SkyCrabConnectionListener(BasicConnection connection, bool errors);
        public List<SkyCrabConnectionListener> closeListeners = new List<SkyCrabConnectionListener>();
        public List<SkyCrabConnectionListener> disposedListeners = new List<SkyCrabConnectionListener>();
        private List<Exception> exceptions = new List<Exception>(); //TODO synchronization


        public IPEndPoint LocalEndPoint
        {
            get { return (IPEndPoint)tcpClient.Client.LocalEndPoint; }
        }
        public IPEndPoint RemoteEndPoint
        {
            get { return (IPEndPoint)tcpClient.Client.RemoteEndPoint; }
        }
        public abstract IPEndPoint ClientEndPoint { get; }
        public abstract IPEndPoint ServerEndPoint { get; }
        public string ClientAuthority
        {
            get { return CreateAuthorityString(ClientEndPoint); }
        }
        public string ServerAuthority
        {
            get { return CreateAuthorityString(ServerEndPoint); }
        }

        public int ReadTimeout
        {
            get { return readTimeout; }
        }

        public List<Exception> Exceptions
        {
            get { return exceptions; }
        }


        private static string CreateAuthorityString(IPEndPoint endPoint)
        {
            string authorityString = endPoint.Address + ":" + endPoint.Port;
            return authorityString;
        }

        protected void OnException(Exception exception)
        {
            exceptions.Add(exception);
            AsyncDispose();
        }

        private AggregateException CreateAggregateException()
        {
            if (exceptions.Count == 0)
                return null;
            else
                return new AggregateException(exceptions);
        }

        protected BasicConnection(TcpClient tcpClient, int readTimeout)
        {
            this.tcpClient = tcpClient;
            this.readTimeout = readTimeout;
            tcpClient.ReceiveTimeout = readTimeout;
        }

        public void AddCloseListener(SkyCrabConnectionListener listener)
        {
            lock (closeListeners)
            {
                closeListeners.Add(listener);
                if (closing)
                    listener.Invoke(this, exceptions.Count != 0);
            }
        }

        public void AddDisposedListener(SkyCrabConnectionListener listener)
        {
            lock (disposedListeners)
            {
                disposedListeners.Add(listener);
                if (disposed)
                    listener.Invoke(this, exceptions.Count != 0);
            }
        }

        public void ClearExceptions()
        {
            exceptions.Clear();
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

        protected void AsyncDispose()
        {
            Task.Factory.StartNew(Dispose);
        }

        public void Dispose()
        {
            lock (_disposeLock)
            {
                if (disposed)
                    return;
                closing = true;
                StopCreatingMessages();
                CallListeners(closeListeners);
                DoDispose();
                disposed = true;
                CallListeners(disposedListeners);
            }
        }

        protected abstract void StopCreatingMessages();

        protected virtual void DoDispose()
        {
            if (tcpClient != null)
            {
                tcpClient.GetStream().Close();
                tcpClient.Close();
                tcpClient = null;
            }
        }

        private void CallListeners(List<SkyCrabConnectionListener> listeners)
        {
            lock (listeners)
            {
                foreach (SkyCrabConnectionListener listener in listeners)
                    listener.Invoke(this, exceptions.Count != 0);
            }
        }

    }
}
