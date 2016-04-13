using System;
using System.Threading;

namespace SkyCrab.Connection.Utils
{
    public class MutexIsNotLockedForCurrentThreadException : SkyCrabConnectionException
    {
    }

    internal sealed class MutexForThread : IDisposable
    {

        private Mutex mutex = new Mutex();
        private Thread thread;


        public void WaitOne()
        {
            mutex.WaitOne();
            thread = Thread.CurrentThread;
        }

        public void Release()
        {
            CheckThread();
            thread = null;
            mutex.ReleaseMutex();
        }

        public void CheckThread()
        {
            if (thread != Thread.CurrentThread)
                throw new MutexIsNotLockedForCurrentThreadException();
        }

        public void Dispose()
        {
            mutex.Dispose();
            thread = null;
        }

    }
}
