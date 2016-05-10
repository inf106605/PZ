using SkyCrab.Common_classes.Chats;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Rooms;
using SkyCrab.Connection.PresentationLayer.MessageConnections;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.InRooms
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.CHAT"/></para>
    /// <para>Data type: <see cref="ChatMessage"/></para>
    /// <para>Possible answers: <see cref="OkMsg"/>, <see cref="ErrorMsg"/></para>
    /// <para>Error codes: <see cref="ErrorCode.NOT_IN_ROOM4"/></para>
    /// </summary>
    public sealed class ChatMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.CHAT; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            ChatMessage chatMessage = ChatMessageTranscoder.Get.Read(connection);
            return chatMessage;
        }

        public static MessageInfo? SyncPost(MessageConnection connection, ChatMessage chatMessage, int timeout)
        {
            return AsyncPostToSyncPost((callback, state) => AsyncPost(connection, chatMessage, callback, state), timeout);
        }

        public static void AsyncPost(MessageConnection connection, ChatMessage chatMessage, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
                ChatMessageTranscoder.Get.Write(connection, writingBlock, chatMessage);
            };
            connection.PostNewMessage(MessageId.CHAT, messageProcedure, callback, state);
        }
    }
}
