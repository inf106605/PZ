using SkyCrab.Connection.PresentationLayer.MessageConnections;

namespace SkyCrab.Connection.PresentationLayer.Messages.Common.Connections
{
    /// <summary>
    /// <para>Sender: Server &amp; Client</para>
    /// <para>ID: <see cref="MessageId.DISCONNECT"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Passible answers: <see cref="OkDisconnectMsg"/></para>
    /// </summary>
    public sealed class DisconnectMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.DISCONNECT; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            return null;
        }

        public static MessageInfo? SyncPostDisconnect(MessageConnection connection, int timeout)
        {
            return SyncPost((callback, state) => AsyncPostDisconnect(connection, callback, state), timeout);
        }

        public static void AsyncPostDisconnect(MessageConnection connection, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
                connection.SetAnswerCallback(writingBlock, callback, state);
            };
            connection.PostMessage(MessageId.DISCONNECT, messageProcedure);
        }

    }
}
