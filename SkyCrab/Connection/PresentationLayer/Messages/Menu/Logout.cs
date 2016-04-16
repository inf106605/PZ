namespace SkyCrab.Connection.PresentationLayer.Messages.Menu
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.LOGOUT"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Passible answers: <see cref="Ok"/>, <see cref="Error"/></para>
    /// <para>Error codes: <see cref="ErrorCode.NOT_LOGGED"/></para>
    /// </summary>
    public sealed class Logout : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.LOGOUT; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            return null;
        }
        
        public static MessageConnection.MessageInfo? SyncPostLogout(MessageConnection connection, int timeout)
        {
            return SyncPost((callback, state) => AsyncPostLogout(connection, callback, state), timeout);
        }

        public static void AsyncPostLogout(MessageConnection connection, MessageConnection.AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
                connection.SetAnswerCallback(writingBlock, callback, state);
            };
            connection.PostMessage(MessageId.LOGOUT, messageProcedure);
        }
    }
}
