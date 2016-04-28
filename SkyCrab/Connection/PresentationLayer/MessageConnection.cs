using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes;
using SkyCrab.Connection.PresentationLayer.MessageConnections;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Menu;
using SkyCrab.Connection.SessionLayer;
using SkyCrab.Connection.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SkyCrab.Connection.PresentationLayer
{
    public class SkyCrabConnectionProtocolVersionException : SkyCrabConnectionException
    {

        public SkyCrabConnectionProtocolVersionException(String message) :
            base(message)
        {
        }

    }

    public class UnknownMessageException : SkyCrabConnectionException
    {
    }

    //TODO proper disconnecting
    public abstract class MessageConnection : EncryptedConnection
    {

        private static readonly Version version = new Version(3, 1, 0);
        private static readonly Dictionary<MessageId, AbstractMessage> messageTypes = new Dictionary<MessageId, AbstractMessage>();
        private Task listeningTask;
        private Task processingTask;
        private Timer pingTimer;
        protected BlockingCollection<MessageInfo> messages = new BlockingCollection<MessageInfo>(new ConcurrentQueue<MessageInfo>());
        private AnswerQueue answerQueue = new AnswerQueue();
        protected bool processingMessagesOk;


        static MessageConnection()
        {
            addMessage(new DisconnectMsg());
            addMessage(new PingMsg());
            addMessage(new PongMsg());

            addMessage(new OkMsg());
            addMessage(new ErrorMsg());
            addMessage(new LoginMsg());
            addMessage(new LoginOkMsg());
            addMessage(new LogoutMsg());
            addMessage(new RegisterMsg());
            addMessage(new EditProfileMsg());
            addMessage(new GetFriendsMsg());
            addMessage(new FindPlayerMsg());
            addMessage(new PlayerListMsg());
            addMessage(new AddFriendMsg());
            addMessage(new RemoveFriendMsg());
            //TODO more MORE!!!
        }

        private static void addMessage(AbstractMessage message)
        {
            messageTypes.Add(message.Id, message);
        }

        protected MessageConnection(TcpClient tcpClient, int readTimeout) :
            base(tcpClient, readTimeout)
        {
            CheckVersion();
            StartTasks();
        }

        private void StartTasks()
        {
            listeningTask = Task.Factory.StartNew(RunListeningTaskBody, TaskCreationOptions.LongRunning);
            processingTask = Task.Factory.StartNew(ProcessMessages, TaskCreationOptions.LongRunning);
            pingTimer = new Timer(PingTaskBody, null, 5000, 5000);
        }

        private void RunListeningTaskBody()
        {
            BeginReadingBlock();
            while (true)
            {
                if (disposed)
                    break;
                TryReadMessage();
            }
            EndReadingBlock();
        }

        private void TryReadMessage()
        {
            try
            {
                MessageId messageId = MessageIdTranscoder.Get.Read(this);
                AbstractMessage message;
                if (!messageTypes.TryGetValue(messageId, out message))
                    throw new UnknownMessageException();
                object messageData = null;
                messageData = message.Read(this);
                EnqueueMessage(message, messageData);
            }
            catch (ReadTimeoutException)
            {
            }
        }

        private void EnqueueMessage(AbstractMessage message, object messageData)
        {
            MessageInfo messageInfo = new MessageInfo();
            messageInfo.messageId = message.Id;
            messageInfo.message = messageData;
            if (message.Answer)
                answerQueue.AddAnswer(messageInfo);
            else
                messages.Add(messageInfo);
        }

        protected abstract void ProcessMessages();

        private void PingTaskBody(object state)
        {
            lock (pingTimer)
            {
                if (disposed)
                    return;
                MessageInfo? messageInfo = PingMsg.SyncPostPing(this, 1000);
                if (!messageInfo.HasValue)
                {
                    if (disposed)
                        return;
                    NoPongMsg noPongMsg = new NoPongMsg();
                    MessageInfo noPongMessageInfo = new MessageInfo();
                    noPongMessageInfo.messageId = noPongMsg.Id;
                    messages.Add(noPongMessageInfo);
                }
            }
        }

        protected void AnswerPing(object message)
        {
            PongMsg.AsyncPostPong(this);
        }

        internal void SetAnswerCallback(object writingBlock, AnswerCallback answerCallback, object state)
        {
            AnswerCallbackWithState answerCallbackWithState = new AnswerCallbackWithState();
            answerCallbackWithState.answerCallback = answerCallback;
            answerCallbackWithState.state = state;
            AsyncWriteBytes(writingBlock, null, AddAnswerCallbackToQueue, answerCallbackWithState);
        }

        private void AddAnswerCallbackToQueue(object state)
        {
            AnswerCallbackWithState answerCallbackWithState = (AnswerCallbackWithState)state;
            answerQueue.AddRequest(answerCallbackWithState);
        }

        private void CheckVersion()
        {
            object writingBlock = BeginWritingBlock();
            VersionTranscoder.Get.Write(this, writingBlock, version);
            EndWritingBlock(writingBlock);
            BeginReadingBlock();
            Version otherVersion = VersionTranscoder.Get.Read(this);
            EndReadingBlock();
            if (version.Major > otherVersion.Major)
                throw new SkyCrabConnectionProtocolVersionException("The other side of the connection has too old version of the protocol!");
            else if (version.Major < otherVersion.Major)
                throw new SkyCrabConnectionProtocolVersionException("The other side of the connection has too new version of the protocol!");
            else if (version.Minor > otherVersion.Minor)
                Console.WriteLine("The other side of the connection has an older version of the protocol. It should be updated.");
            else if (version.Minor < otherVersion.Minor)
                Console.WriteLine("The other side of the connection has a newer version of the protocol. Update is recomended.");
            else if (version.Build != otherVersion.Build)
                Console.WriteLine("The other side of the connection has another version of the protocol but it should work.");
        }

        public delegate void MessageProcedure(object writingBlock);

        internal void PostMessage(MessageId messageId, MessageProcedure messageProcedure)
        {
            object writingBlock = BeginWritingBlock();
            MessageIdTranscoder.Get.Write(this, writingBlock, messageId);
            messageProcedure.Invoke(writingBlock);
            EndWritingBlock(writingBlock);
        }

        protected override void DoDispose()
        {
            pingTimer.Dispose();
            lock (pingTimer) {}
            CloseListeningTask();
            CloseProcessingTask();
            listeningTask.Dispose();
            processingTask.Dispose();
            answerQueue.Dispose();
            base.Dispose();
        }

        private void CloseListeningTask()
        {
            if (!listeningTask.Wait(1000))
                throw new TaskIsNotRespondingException();
        }

        private void CloseProcessingTask()
        {
            messages.CompleteAdding();
            while (true)
            {
                processingMessagesOk = false;
                if (processingTask.Wait(1000))
                    break;
                if (!processingMessagesOk)
                    throw new TaskIsNotRespondingException();
            }
        }

    }
}
