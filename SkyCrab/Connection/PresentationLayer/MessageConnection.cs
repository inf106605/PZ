//#define SHOW_MESSAGES
#if SHOW_MESSAGES
#warning "Received messages are writed to console!"
#endif

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

        private struct StateWithId
        {
            public Int16 id;
            public object state;
        }


        private static readonly Version version = new Version(11, 0, 1);
        private static readonly Dictionary<MessageId, AbstractMessage> messageTypes = new Dictionary<MessageId, AbstractMessage>();
        private Task listeningTask;
        private Task processingTask;
        private Timer pingTimer;
        protected BlockingCollection<MessageInfo> messages = new BlockingCollection<MessageInfo>(new ConcurrentQueue<MessageInfo>());
        private AnswerQueue answerQueue = new AnswerQueue();
        protected bool processingMessagesOk;
        private volatile bool closingListeningTask = false;
        private Semaphore disconnectSemaphore = new Semaphore(0, 1);
        protected bool disconectedOnItsOwn = true;
        private readonly ISequence messageIdSequence;


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
            AddMessage(new NewRoomOwnerMsg());
            AddMessage(new ChatMsg());

            //--- Gane ---
            AddMessage(new GameStartedMsg());
            AddMessage(new NextTurnMsg());
            AddMessage(new NewTilesMsg());
            AddMessage(new LossTilesMsg());
            AddMessage(new GainPointsMsg());
            AddMessage(new ReorderRackTilesMsg());
            AddMessage(new TurnTimeoutMsg());
            AddMessage(new PlaceTilesMsg());
            AddMessage(new ExchangeTilesMsg());
            AddMessage(new PassMsg());
            AddMessage(new GameEndedMsg());
        }

        private static void AddMessage(AbstractMessage message)
        {
            messageTypes.Add(message.Id, message);
        }

        protected MessageConnection(TcpClient tcpClient, int readTimeout, bool server) :
            base(tcpClient, readTimeout)
        {
            messageIdSequence = server ? (ISequence)new SecondHalfSequence() : (ISequence)new FirstHalfSequence();
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
                    if (closingListeningTask)
                        break;
                    if (!TryReadMessage())
                        break;
                }
            }
            catch (Exception e)
            {
                OnException(e);
            }
            finally
            {
                EndReadingBlock();
            }
        }

        private bool TryReadMessage()
        {
            try
            {
                Int16 id = Int16Transcoder.Get.Read(this);
                MessageId messageId = MessageIdTranscoder.Get.Read(this);
                #if SHOW_MESSAGES
                Console.WriteLine("<-" + RemoteEndPoint.Port + " - " + messageId.ToString() + " [" + id + "]");
                #endif
                switch (messageId)
                {
                    case MessageId.DISCONNECT:
                        if (!closing)
                            disconectedOnItsOwn = false;
                        disconnectSemaphore.Release();
                        AsyncDispose();
                        break;

                    case MessageId.SHUTDOWN:
                        return false;

                    default:
                        AbstractMessage message;
                        if (!messageTypes.TryGetValue(messageId, out message))
                            throw new UnknownMessageException();
                        object messageData = null;
                        messageData = message.Read(this);
                        EnqueueMessage(id, message, messageData);
                        break;
                }
            }
            catch (ReadTimeoutException)
            {
            }
            return true;
        }

        private void EnqueueMessage(Int16 id, AbstractMessage message, object messageData)
        {
            MessageInfo messageInfo = new MessageInfo();
            messageInfo.id = id;
            messageInfo.messageId = message.Id;
            messageInfo.message = messageData;
            if (message.Answer)
                answerQueue.AddAnswer(id, messageInfo);
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
                OnException(e);
            }
        }

        protected abstract void ProcessMessages();

        private void PingTaskBody(object state)
        {
            lock (pingTimer)
            {
                try
                {
                    if (closing)
                        return;
                    MessageInfo? messageInfo = PingMsg.SyncPost(this, PingTimeout);
                    if (!messageInfo.HasValue)
                    {
                        if (closing)
                            return;
                        NoPongMsg noPongMsg = new NoPongMsg();
                        MessageInfo noPongMessageInfo = new MessageInfo();
                        noPongMessageInfo.messageId = noPongMsg.Id;
                        messages.Add(noPongMessageInfo);
                    }
                    else if (messageInfo.Value.messageId != MessageId.PONG)
                    {
                        throw new SkyCrabConnectionException("Wrong answer to ping! (" + messageInfo.Value.messageId.ToString() + ")");
                    }
                }
                catch (Exception e)
                {
                    OnException(e);
                }
            }
        }

        protected void AnswerPing(Int16 id, object message)
        {
            PongMsg.AsyncPost(id, this);
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

        internal void PostNewMessage(MessageId messageId, MessageProcedure messageProcedure, AnswerCallback answerCallback = null, object state = null)
        {
            Int16 id = messageIdSequence.Value;
            PostMessage(id, messageId, messageProcedure, answerCallback, state);
        }

        internal void PostAnswerMessage(Int16 id, MessageId messageId, MessageProcedure messageProcedure, AnswerCallback answerCallback = null, object state = null)
        {
            PostMessage(id, messageId, messageProcedure, answerCallback, state);
        }

        private void PostMessage(Int16 id, MessageId messageId, MessageProcedure messageProcedure, AnswerCallback answerCallback = null, object state = null)
        {
            #if SHOW_MESSAGES
            Console.WriteLine("->" + RemoteEndPoint.Port + " - " + messageId.ToString() + " [" + id + "]");
            #endif
            object writingBlock = BeginWritingBlock();
            Int16Transcoder.Get.Write(this, writingBlock, id);
            MessageIdTranscoder.Get.Write(this, writingBlock, messageId);
            if (messageProcedure != null)
                messageProcedure.Invoke(writingBlock);
            if (answerCallback != null)
                SetAnswerCallback(writingBlock, id, answerCallback, state);
            EndWritingBlock(writingBlock);
        }

        private void SetAnswerCallback(object writingBlock, Int16 id, AnswerCallback answerCallback, object state)
        {
            StateWithId stateWithId = new StateWithId();
            stateWithId.id = id;
            stateWithId.state = state;
            AnswerCallbackWithState answerCallbackWithState = new AnswerCallbackWithState();
            answerCallbackWithState.answerCallback = answerCallback;
            answerCallbackWithState.state = stateWithId;
            AsyncWriteBytes(writingBlock, null, AddAnswerCallbackToQueue, answerCallbackWithState);
        }

        private void AddAnswerCallbackToQueue(object state)
        {
            AnswerCallbackWithState answerCallbackWithState = (AnswerCallbackWithState)state;
            StateWithId stateWithId = (StateWithId)answerCallbackWithState.state;
            answerCallbackWithState.state = stateWithId.state;
            answerQueue.AddRequest(stateWithId.id, answerCallbackWithState);
        }

        protected override void StopCreatingMessages()
        {
            StopPingTimer();
            base.StopCreatingMessages();
        }

        private void StopPingTimer()
        {
            pingTimer.Dispose();
            lock (pingTimer) { }
        }

        protected override void DoDispose()
        {
            ExchangeDisconnectMessages();
            CloseListeningTask();
            CloseProcessingTask();
            listeningTask.Dispose();
            processingTask.Dispose();
            answerQueue.Dispose();
            base.DoDispose();
        }

        private void ExchangeDisconnectMessages()
        {
            DisconnectMsg.AsyncPost(this);
            disconnectSemaphore.WaitOne(ReadTimeout * 10);
            ShutdownMsg.AsyncPost(this);
        }

        private void CloseListeningTask()
        {
            closingListeningTask = true;
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
