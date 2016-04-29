using SkyCrab.Connection.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SkyCrab.Connection.SessionLayer
{
    public class ReadTimeoutException : SkyCrabConnectionException
    {
    }
    
    public abstract class BasicConnection : IDisposable
    {

        public const int PORT = 8888;

        private TcpClient tcpClient;
        protected volatile bool isDisposing = false;
        protected volatile bool isDisposed = false;
        private volatile bool preparedForDispose = false;
        private object _prepareForDisposeLock = new object();
        private object _disposeLock = new object();
        public delegate void ConnectionCloseListener(BasicConnection connection, AggregateException exceptions);
        public List<ConnectionCloseListener> connectionCloseListeners = new List<ConnectionCloseListener>();
        private object _connectionCloseListenerLock = new object();
        private List<Exception> exceptions = new List<Exception>();


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


        private static string CreateAuthorityString(IPEndPoint endPoint)
        {
            string authorityString = endPoint.Address + ":" + endPoint.Port;
            return authorityString;
        }

        protected void StoreException(Exception e)
        {
            exceptions.Add(e);
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
            tcpClient.ReceiveTimeout = readTimeout;
        }

        public void addConnectionCloseListener(ConnectionCloseListener listener)
        {
            connectionCloseListeners.Add(listener);
            lock (_connectionCloseListenerLock)
            {
                AggregateException exceptions = CreateAggregateException();
                if (preparedForDispose)
                    listener.Invoke(this, exceptions);
            }
        }

        public void removeConnectionCloseListener(ConnectionCloseListener listener)
        {
            connectionCloseListeners.Remove(listener);
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

        protected void PrepareForDispose(bool answeringToDisposeMsg)
        {
            if (!answeringToDisposeMsg)
            {
                Monitor.Enter(_prepareForDisposeLock);
            }
            else
            {
                if (!Monitor.TryEnter(_prepareForDisposeLock))
                    return;
            }
            try
            {
                if (preparedForDispose)
                    return;
                preparedForDispose = true;
                lock (_connectionCloseListenerLock)
                    CallConnectionCloseListeners();
                DoPrepareForDispose(answeringToDisposeMsg);
            }
            finally
            {
                Monitor.Exit(_prepareForDisposeLock);
            }
        }

        private void CallConnectionCloseListeners()
        {
            AggregateException exceptions = CreateAggregateException();
            foreach (ConnectionCloseListener listener in connectionCloseListeners)
                listener.Invoke(this, exceptions);
        }

        protected void AsyncDispose()
        {
            Task.Factory.StartNew(Dispose);
        }

        protected abstract void DoPrepareForDispose(bool answeringToDisposeMsg);

        public void Dispose()
        {
            lock (_disposeLock)
            {
                if (isDisposing)
                    return;
                isDisposing = true;
            }
            PrepareForDispose(false);
            isDisposed = true;
            DoDispose();
        }

        protected virtual void DoDispose()
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
