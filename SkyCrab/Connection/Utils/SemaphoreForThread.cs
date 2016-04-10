using System.Threading;

namespace SkyCrab.Connection.Utils
{
    public class SemaphoreIsNotLockedForCurrentThreadException : SkyCrabConnectionException
    {
    }

    internal sealed class SemaphoreForThread
    {

        private Semaphore semaphore = new Semaphore(1, 1);
        private Thread thread;


        public void WaitOne()
        {
            semaphore.WaitOne();
            thread = Thread.CurrentThread;
        }

        public void Release()
        {
            CheckThread();
            thread = null;
            semaphore.Release();
        }

        public void CheckThread()
        {
            if (thread != Thread.CurrentThread)
                throw new SemaphoreIsNotLockedForCurrentThreadException();
        }

    }
}
