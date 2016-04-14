using SkyCrab.Connection.PresentationLayer;
using System;
using System.Threading;

namespace SkyCrab.Connection.Utils
{
    internal sealed class AnswerSynchronizer : IDisposable
    {

        private Semaphore semaphore = new Semaphore(0, 1);
        private MessageConnection.MessageInfo? answer;



        public MessageConnection.MessageInfo? Answer
        {
            get
            {
                return answer;
            }
        }


        public static MessageConnection.AnswerCallback Callback
        {
            get
            {
                return RunSyncWriteCallbackBody;
            }
        }


        public void Wait()
        {
            semaphore.WaitOne();
        }

        private static void RunSyncWriteCallbackBody(MessageConnection.MessageInfo? answer, object state)
        {
            AnswerSynchronizer synchronizer = (AnswerSynchronizer)state;
            synchronizer.answer = answer;
            Semaphore semaphore = synchronizer.semaphore;
            semaphore.Release();
        }

        public void Dispose()
        {
            semaphore.Dispose();
        }

    }
}
