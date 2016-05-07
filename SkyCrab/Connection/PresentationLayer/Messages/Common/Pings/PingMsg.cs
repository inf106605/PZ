using SkyCrab.Connection.PresentationLayer.MessageConnections;

namespace SkyCrab.Connection.PresentationLayer.Messages.Common.Pings
{
    /// <summary>
    /// <para>Sender: Server &amp; Client</para>
    /// <para>ID: <see cref="MessageId.PING"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Passible answers: <see cref="PongMsg"/></para>
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

        public static MessageInfo? SyncPostPing(MessageConnection connection, int timeout)
        {
            return SyncPost((callback, state) => AsyncPostPing(connection, callback, state), timeout);
        }

        public static void AsyncPostPing(MessageConnection connection, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
            };
            connection.PostNewMessage(MessageId.PING, messageProcedure, callback, state);
        }

    }
}
