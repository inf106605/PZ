using SkyCrab.Connection.PresentationLayer.MessageConnections;
using System;
using System.Threading;

namespace SkyCrab.Connection.Utils
{
    internal sealed class AnswerSynchronizer : IDisposable
    {

        private Semaphore semaphore = new Semaphore(0, 1);
        private MessageInfo? answer;
        private object _lock = new object();



        public MessageInfo? Answer
        {
            get { return answer; }
        }


        public static AnswerCallback Callback
        {
            get { return RunSyncWriteCallbackBody; }
        }


        public void Wait(int timeout)
        {
            semaphore.WaitOne(timeout);
        }

        private static void RunSyncWriteCallbackBody(MessageInfo? answer, object state)
        {
            AnswerSynchronizer synchronizer = (AnswerSynchronizer)state;
            synchronizer.answer = answer;
            Semaphore semaphore = synchronizer.semaphore;
            lock (synchronizer._lock)
            {
                if (semaphore != null)
                    semaphore.Release();
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                semaphore.Dispose();
                semaphore = null;
            }
        }

    }
}
