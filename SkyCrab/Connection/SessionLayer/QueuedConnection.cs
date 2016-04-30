using SkyCrab.Connection.Utils;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SkyCrab.Connection.SessionLayer
{
    public class MethodIsNotThreadSafeException : SkyCrabConnectionException
    {
    }
    
    public abstract class QueuedConnection : BasicConnection
    {
        
        public delegate void Callback(Object state);

        private struct WriteInfo
        {
            public byte[] bytes;
            public Callback callback;
            public object state;
        }


        private MutexForThread readMutex = new MutexForThread();
        private BlockingCollection<BlockingCollection<WriteInfo>> writeQueue = new BlockingCollection<BlockingCollection<WriteInfo>>(new ConcurrentQueue<BlockingCollection<WriteInfo>>());
        private Task writeTask;
        private volatile bool writeTaskIsOk;


        protected QueuedConnection(TcpClient tcpClient, int readTimeout) :
            base(tcpClient, readTimeout)
        {
            runWriteTask();
        }

        private void runWriteTask()
        {
            writeTask = Task.Factory.StartNew(RunWriteTaskBody, TaskCreationOptions.LongRunning);
        }

        private void RunWriteTaskBody()
        {
            try
            {
                foreach (BlockingCollection<WriteInfo> queue in writeQueue.GetConsumingEnumerable())
                {
                    foreach (WriteInfo writeInfo in queue.GetConsumingEnumerable())
                    {
                        writeTaskIsOk = true;
                        if (!isDisposed && writeInfo.bytes != null)
                            base.WriteBytes(writeInfo.bytes);
                        if (writeInfo.callback != null)
                            writeInfo.callback.Invoke(writeInfo.state);
                    }
                    queue.Dispose();
                }
            }
            catch (Exception e)
            {
                StoreException(e);
                AsyncDispose();
            }
        }

        #pragma warning disable 809
        [Obsolete("This method is not tread safe.", true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected override byte[] ReadBytes(ushort size)
        {
            throw new MethodIsNotThreadSafeException();
        }
        #pragma warning restore 809

        #pragma warning disable 809
        [Obsolete("This method is not tread safe.", true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected override void WriteBytes(byte[] bytes)
        {
            throw new MethodIsNotThreadSafeException();
        }
        #pragma warning restore 809

        protected void BeginReadingBlock()
        {
            readMutex.WaitOne();
        }

        protected Object BeginWritingBlock()
        {
            BlockingCollection<WriteInfo> localWriteQueue = new BlockingCollection<WriteInfo>(new ConcurrentQueue<WriteInfo>());
            writeQueue.Add(localWriteQueue);
            return localWriteQueue;
        }

        protected void EndReadingBlock()
        {
            readMutex.Release();
        }

        protected void EndWritingBlock(Object writingBlock)
        {
            BlockingCollection<WriteInfo> localWriteQueue = (BlockingCollection<WriteInfo>)writingBlock;
            localWriteQueue.CompleteAdding();
        }

        internal virtual byte[] SyncReadBytes(UInt16 size)
        {
            readMutex.CheckThread();
            byte[] bytes = base.ReadBytes(size);
            return bytes;
        }

        internal virtual void AsyncWriteBytes(Object writingBlock, byte[] bytes, Callback callback = null, object state = null)
        {
            BlockingCollection<WriteInfo> localWriteQueue = (BlockingCollection<WriteInfo>)writingBlock;
            WriteInfo writeInfo = new WriteInfo();
            writeInfo.bytes = bytes;
            writeInfo.callback = callback;
            writeInfo.state = state;
            localWriteQueue.Add(writeInfo);
        }

        protected override void DoPrepareForDispose(bool answeringForDisconnectMsg)
        {
            for (int i = 0; i != 100; ++i)
            {
                if (writeQueue.Count == 0)
                    break;
                else
                    Thread.Sleep(10);
            }
        }

        protected override void DoDispose()
        {
            CloseWriteTask();
            writeTask.Dispose();
            readMutex.Dispose();
            writeQueue.Dispose();
            base.Dispose();
        }

        private void CloseWriteTask()
        {
            writeQueue.CompleteAdding();
            while (true)
            {
                writeTaskIsOk = false;
                if (writeTask.Wait(1000))
                    break;
                if (!writeTaskIsOk)
                    throw new TaskIsNotRespondingException();
            }
        }

    }
}
