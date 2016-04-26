namespace SkyCrab.Connection.PresentationLayer.Messages.Menu
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

        public static MessageConnection.MessageInfo? SyncPostPing(MessageConnection connection, int timeout)
        {
            return SyncPost((callback, state) => AsyncPostPing(connection, callback, state), timeout);
        }

        public static void AsyncPostPing(MessageConnection connection, MessageConnection.AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
                connection.SetAnswerCallback(writingBlock, callback, state);
            };
            connection.PostMessage(MessageId.PING, messageProcedure);
        }

    }
}
