using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes;
using SkyCrab.Connection.PresentationLayer.MessageConnections;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Common.Connections;
using SkyCrab.Connection.PresentationLayer.Messages.Common.Errors;
using SkyCrab.Connection.PresentationLayer.Messages.Common.Pings;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Accounts;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Friends;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.InRooms;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Rooms;
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
    
    public abstract class MessageConnection : EncryptedConnection
    {

        private static readonly Version version = new Version(6, 0, 0);
        private static readonly Dictionary<MessageId, AbstractMessage> messageTypes = new Dictionary<MessageId, AbstractMessage>();
        private Task listeningTask;
        private Task processingTask;
        private Timer pingTimer;
        protected BlockingCollection<MessageInfo> messages = new BlockingCollection<MessageInfo>(new ConcurrentQueue<MessageInfo>());
        private AnswerQueue answerQueue = new AnswerQueue();
        protected bool processingMessagesOk;


        private int PingTimeout
        {
            get
            {
                int result = ReadTimeout * 10;
                if (result > 20000)
                    result = 20000;
                if (result < 1000)
                    result = 1000;
                if (result < ReadTimeout * 2)
                    result = ReadTimeout * 2;
                return result;
            }
        }


        static MessageConnection()
        {
            //--- Common ---
            //Connections
            AddMessage(new DisconnectMsg());
            AddMessage(new OkDisconnectMsg());
            //Pings
            AddMessage(new PingMsg());
            AddMessage(new PongMsg());
            //Errors
            AddMessage(new OkMsg());
            AddMessage(new ErrorMsg());

            //--- Menu ---
            //Accounts
            AddMessage(new LoginMsg());
            AddMessage(new LoginOkMsg());
            AddMessage(new LogoutMsg());
            AddMessage(new RegisterMsg());
            AddMessage(new EditProfileMsg());
            //Friends
            AddMessage(new GetFriendsMsg());
            AddMessage(new FindPlayersMsg());
            AddMessage(new PlayerListMsg());
            AddMessage(new AddFriendMsg());
            AddMessage(new RemoveFriendMsg());
            //Rooms
            AddMessage(new GetFriendRoomsMsg());
            AddMessage(new FindRoomsMsg());
            AddMessage(new RoomListMsg());
            AddMessage(new CreateRoomMsg());
            AddMessage(new RoomMsg());
            //InRooms
            AddMessage(new JoinRoomMsg());
            AddMessage(new LeaveRoomMsg());
            AddMessage(new PlayerJoinedMsg());
            AddMessage(new PlayerLeavedMsg());
            AddMessage(new PlayerReadyMsg());
            AddMessage(new PlayerNotReadyMsg());
            AddMessage(new ChatMsg());
        }

        private static void AddMessage(AbstractMessage message)
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
            processingTask = Task.Factory.StartNew(ProcessMessagesTaskBody, TaskCreationOptions.LongRunning);
            pingTimer = new Timer(PingTaskBody, null, PingTimeout * 3, PingTimeout * 3);
        }

        private void RunListeningTaskBody()
        {
            try
            {
                BeginReadingBlock();
                while (true)
                {
                    if (isDisposed)
                        break;
                    TryReadMessage();
                }
                EndReadingBlock();
            }
            catch (Exception e)
            {
                StoreException(e);
                AsyncDispose();
            }
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

        private void ProcessMessagesTaskBody()
        {
            try
            {
                ProcessMessages();
            }
            catch (Exception e)
            {
                StoreException(e);
                AsyncDispose();
            }
        }

        protected abstract void ProcessMessages();

        private void PingTaskBody(object state)
        {
            try
            {
                lock (pingTimer)
                {
                    if (isDisposing)
                        return;
                    MessageInfo? messageInfo = PingMsg.SyncPostPing(this, PingTimeout);
                    if (!messageInfo.HasValue)
                    {
                        if (isDisposing)
                            return;
                        NoPongMsg noPongMsg = new NoPongMsg();
                        MessageInfo noPongMessageInfo = new MessageInfo();
                        noPongMessageInfo.messageId = noPongMsg.Id;
                        messages.Add(noPongMessageInfo);
                    }
                }
            }
            catch (Exception e)
            {
                StoreException(e);
                AsyncDispose();
            }
        }

        protected void AnswerPing(object message)
        {
            PongMsg.AsyncPostPong(this);
        }

        protected void AnswerDisconnect(object message)
        {
            Task.Factory.StartNew(() => AnswerDisconnectTaskBody(message));
        }

        private void AnswerDisconnectTaskBody(object message)
        {
            PrepareForDispose(true);
            OkDisconnectMsg.AsyncPostOkDisconnect(this);
            AsyncDispose();
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
            if (isDisposed)
                throw new ObjectDisposedException("connection");
            object writingBlock = BeginWritingBlock();
            MessageIdTranscoder.Get.Write(this, writingBlock, messageId);
            messageProcedure.Invoke(writingBlock);
            EndWritingBlock(writingBlock);
        }

        protected override void DoPrepareForDispose(bool answeringToDisposeMsg)
        {
            pingTimer.Dispose();
            lock (pingTimer) { }
            base.DoPrepareForDispose(answeringToDisposeMsg);
            if (!answeringToDisposeMsg)
                DisconnectMsg.SyncPostDisconnect(this, ReadTimeout * 10);
        }

        protected override void DoDispose()
        {
            CloseListeningTask();
            CloseProcessingTask();
            listeningTask.Dispose();
            processingTask.Dispose();
            answerQueue.Dispose();
            base.DoDispose();
        }

        private void CloseListeningTask()
        {
            if (!listeningTask.Wait(ReadTimeout * 10))
                throw new TaskIsNotRespondingException();
        }

        private void CloseProcessingTask()
        {
            messages.CompleteAdding();
            while (true)
            {
                processingMessagesOk = false;
                if (processingTask.Wait(ReadTimeout * 10))
                    break;
                if (!processingMessagesOk)
                    throw new TaskIsNotRespondingException();
            }
        }

    }
}
