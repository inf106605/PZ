using SkyCrab.Connection.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SkyCrab.Connection.PresentationLayer.MessageConnections
{
    internal sealed class AnswerQueue : IDisposable
    {

        private struct RequestWithId
        {
            public Int16 id;
            public AnswerCallbackWithState request;
        }

        private struct AnswerWithId
        {
            public Int16 id;
            public MessageInfo answer;
        }


        private Queue<RequestWithId?> requests = new Queue<RequestWithId?>();
        private Queue<AnswerWithId?> answers = new Queue<AnswerWithId?>();
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
                RequestWithId? requestWithId;
                lock (requests)
                    requestWithId = requests.Dequeue();
                if (!requestWithId.HasValue)
                    return;
                answerSemaphore.WaitOne();
                AnswerWithId? answerWithId;
                lock (answers)
                    answerWithId = answers.Dequeue();
                if (!answerWithId.HasValue)
                {
                    SendDummyAnswer(requestWithId.Value.request);
                    return;
                }
                if (requestWithId.Value.id == answerWithId.Value.id)
                    Task.Factory.StartNew(() => requestWithId.Value.request.answerCallback.Invoke(answerWithId.Value.answer, requestWithId.Value.request.state));
                else if (HalfLess(requestWithId.Value.id, answerWithId.Value.id))
                    Console.Error.WriteLine("Received answer with id " + answerWithId.Value.id + " but not answer with ID " + requestWithId.Value.id + "!"); //TODO something more?
                else
                    Console.Error.WriteLine("Answer with ID " + answerWithId.Value.id + " is duplicated!"); //TODO something more?
            }
        }

        public void AddRequest(Int16 id, AnswerCallbackWithState request)
        {
            RequestWithId requestWithId = new RequestWithId();
            requestWithId.id = id;
            requestWithId.request = request;
            lock (requests)
                requests.Enqueue(requestWithId);
            requestSemaphore.Release();
        }

        public void AddAnswer(Int16 id, MessageInfo answer)
        {
            AnswerWithId messageInfoWithId = new AnswerWithId();
            messageInfoWithId.id = id;
            messageInfoWithId.answer = answer;
            lock (answers)
                answers.Enqueue(messageInfoWithId);
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
                foreach (RequestWithId? requestWithId in requests)
                    if (requestWithId.HasValue)
                        SendDummyAnswer(requestWithId.Value.request);
            requestSemaphore.Dispose();
            answerSemaphore.Dispose();
            task.Dispose();
        }

        private void SendDummyAnswer(AnswerCallbackWithState request)
        {
            Task.Factory.StartNew(() => request.answerCallback.Invoke(null, request.state));
        }

        private static bool HalfLess(Int16 x, Int16 y)
        {
            UInt16 a = (UInt16)((UInt16)(x < 0 ? -x : x) | 0x8000);
            UInt16 b = (UInt16)((UInt16)(y < 0 ? -y : y) | 0x8000);
            a -= b;
            return a >= UInt16.MaxValue / 2;
        }

    }
}
