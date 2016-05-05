using SkyCrab.Common_classes.Chats;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Rooms;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.InRooms
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.CHAT"/></para>
    /// <para>Data type: <see cref="ChatMessage"/></para>
    /// <para>Passible answers: [none]</para>
    /// <para>Error codes: [none]</para>
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

        public static void AsyncPostChat(MessageConnection connection, ChatMessage chatMessage)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
                ChatMessageTranscoder.Get.Write(connection, writingBlock, chatMessage);
            };
            connection.PostMessage(MessageId.CHAT, messageProcedure);
        }
    }
}
