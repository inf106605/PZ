using SkyCrab.Connection.Utils;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SkyCrab.Connection.SessionLayer
{
    public class CannotReadOrWriteZeroBytesException : SkyCrabConnectionException
    {
    }

    public class TaskIsNotRespondingException : SkyCrabConnectionException
    {
    }

    public class MethodIsNotThreadSafeException : SkyCrabConnectionException
    {
    }

    //TODO make methods protected
    internal class QueuedConnection : BasicConnection
    {
        
        public delegate void Callback(Object state); //TODO make it protected

        private struct WriteInfo
        {
            public byte[] bytes;
            public Callback callback;
            public object state;
        }


        private SemaphoreForThread readSemaphore = new SemaphoreForThread();
        private BlockingCollection<BlockingCollection<WriteInfo>> writeQueue = new BlockingCollection<BlockingCollection<WriteInfo>>();  //TODO dispose?
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
            foreach (BlockingCollection<WriteInfo> queue in writeQueue.GetConsumingEnumerable())
                foreach (WriteInfo writeInfo in queue.GetConsumingEnumerable())
                {
                    writeTaskIsOk = true;
                    base.WriteBytes(writeInfo.bytes);
                    if (writeInfo.callback != null)
                        writeInfo.callback.Invoke(writeInfo.state);
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

        public void BeginReadingBlock()
        {
            readSemaphore.WaitOne();
        }

        public Object BeginWritingBlock()
        {
            BlockingCollection<WriteInfo> localWriteQueue = new BlockingCollection<WriteInfo>();
            writeQueue.Add(localWriteQueue);
            return localWriteQueue;
        }

        public void EndReadingBlock()
        {
            readSemaphore.Release();
        }

        public void EndWritingBlock(Object writingBlock)
        {
            BlockingCollection<WriteInfo> localWriteQueue = (BlockingCollection<WriteInfo>)writingBlock;
            localWriteQueue.CompleteAdding();
        }

        protected virtual byte[] SyncReadBytes(UInt16 size)
        {
            readSemaphore.CheckThread();
            byte[] bytes = base.ReadBytes(size);
            return bytes;
        }

        protected virtual void SyncWriteBytes(Object writingBlock, byte[] bytes)
        {
            using (Semaphore semaphore = new Semaphore(1, 1))
            {
                AsyncWriteBytes(writingBlock, bytes, RunSyncWriteCallbackBody, semaphore);
                semaphore.WaitOne();
            }
        }

        private static void RunSyncWriteCallbackBody(object state)
        {
            Semaphore semaphore = (Semaphore)state;
            semaphore.Release();
        }

        protected virtual void AsyncWriteBytes(Object writingBlock, byte[] bytes, Callback callback = null, object state = null)
        {
            BlockingCollection<WriteInfo> localWriteQueue = (BlockingCollection<WriteInfo>)writingBlock;
            if (bytes == null || bytes.Length == 0)
                throw new CannotReadOrWriteZeroBytesException();
            WriteInfo writeInfo = new WriteInfo();
            writeInfo.bytes = bytes;
            writeInfo.callback = callback;
            writeInfo.state = state;
            localWriteQueue.Add(writeInfo);
        }

        public override void Dispose()
        {
            CloseTasks();
            base.Dispose();
        }

        private void CloseTasks()
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
