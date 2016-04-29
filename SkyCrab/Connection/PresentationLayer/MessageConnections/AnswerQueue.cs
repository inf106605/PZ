using SkyCrab.Connection.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SkyCrab.Connection.PresentationLayer.MessageConnections
{
    internal sealed class AnswerQueue : IDisposable
    {
        private Queue<AnswerCallbackWithState?> requests = new Queue<AnswerCallbackWithState?>();
        private Queue<MessageInfo?> answers = new Queue<MessageInfo?>();
        private Semaphore requestSemaphore = new Semaphore(0, int.MaxValue);
        private Semaphore answerSemaphore = new Semaphore(0, int.MaxValue);
        private Task task;


        public AnswerQueue()
        {
            StartTask();
        }

        private void StartTask()
        {
            task = Task.Factory.StartNew(RunTaskBody, TaskCreationOptions.LongRunning);
        }

        private void RunTaskBody()
        {
            while (true)
            {
                requestSemaphore.WaitOne();
                AnswerCallbackWithState? request;
                lock (requests)
                    request = requests.Dequeue();
                if (!request.HasValue)
                    return;
                answerSemaphore.WaitOne();
                MessageInfo? answer;
                lock (answers)
                    answer = answers.Dequeue();
                if (!answer.HasValue)
                {
                    SendDummyAnswer(request.Value);
                    return;
                }
                Task.Factory.StartNew(() => request.Value.answerCallback.Invoke(answer.Value, request.Value.state)); //TODO detach
            }
        }

        public void AddRequest(AnswerCallbackWithState request)
        {
            lock (requests)
                requests.Enqueue(request);
            requestSemaphore.Release();
        }

        public void AddAnswer(MessageInfo answer)
        {
            lock (answers)
                answers.Enqueue(answer);
            answerSemaphore.Release();
        }

        public void Dispose()
        {
            lock (requests)
                requests.Enqueue(null);
            requestSemaphore.Release();
            lock (answers)
                answers.Enqueue(null);
            answerSemaphore.Release();
            if (!task.Wait(100))
                throw new TaskIsNotRespondingException();
            lock (requests)
                foreach (AnswerCallbackWithState? request in requests)
                    if (request.HasValue)
                        SendDummyAnswer(request.Value);
            requestSemaphore.Dispose();
            answerSemaphore.Dispose();
            task.Dispose();
        }

        private void SendDummyAnswer(AnswerCallbackWithState request)
        {
            Task.Factory.StartNew(() => request.answerCallback.Invoke(null, request.state)); //TODO detach
        }

    }
}
