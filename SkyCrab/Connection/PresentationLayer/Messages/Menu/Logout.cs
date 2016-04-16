namespace SkyCrab.Connection.PresentationLayer.Messages.Menu
{
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
        
        public static MessageConnection.MessageInfo? SyncPostLogout(MessageConnection connection)
        {
            return SyncPost((callback, state) => AsyncPostLogout(connection, callback, state));
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
