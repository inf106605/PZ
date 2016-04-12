using SkyCrab.Connection.SessionLayer;
using System;
using System.Threading;

namespace SkyCrab.Connection.Utils
{
    internal sealed class Synchronizer : IDisposable
    {

        Semaphore semaphore = new Semaphore(0, 1);

        
        public static QueuedConnection.Callback Callback
        {
            get
            {
                return RunSyncWriteCallbackBody;
            }
        }


        public void wait()
        {
            semaphore.WaitOne();
        }

        private static void RunSyncWriteCallbackBody(object state)
        {
            Synchronizer synchronizer = (Synchronizer)state;
            Semaphore semaphore = synchronizer.semaphore;
            semaphore.Release();
        }

        public void Dispose()
        {
            semaphore.Dispose();
        }

    }
}
