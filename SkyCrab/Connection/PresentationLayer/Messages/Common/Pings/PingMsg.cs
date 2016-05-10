using SkyCrab.Connection.PresentationLayer.MessageConnections;

namespace SkyCrab.Connection.PresentationLayer.Messages.Common.Pings
{
    /// <summary>
    /// <para>Sender: Server &amp; Client</para>
    /// <para>ID: <see cref="MessageId.PING"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Possible answers: <see cref="PongMsg"/></para>
    /// </summary>
    public sealed class PingMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.PING; }
        }

        internal override bool Answer
        {
            get { return false; }
        }


        internal override object Read(MessageConnection connection)
        {
            return null;
        }

        public static MessageInfo? SyncPost(MessageConnection connection, int timeout)
        {
            return AsyncPostToSyncPost((callback, state) => AsyncPost(connection, callback, state), timeout);
        }

        public static void AsyncPost(MessageConnection connection, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
            };
            connection.PostNewMessage(MessageId.PING, messageProcedure, callback, state);
        }

    }
}
