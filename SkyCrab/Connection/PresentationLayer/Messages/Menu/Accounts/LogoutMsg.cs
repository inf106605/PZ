using SkyCrab.Connection.PresentationLayer.MessageConnections;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.Accounts
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.LOGOUT"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Possible answers: <see cref="OkMsg"/>, <see cref="ErrorMsg"/></para>
    /// <para>Error codes: <see cref="ErrorCode.NOT_LOGGED"/></para>
    /// </summary>
    public sealed class LogoutMsg : AbstractMessage
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
        
        public static MessageInfo? SyncPost(MessageConnection connection, int timeout)
        {
            return AsyncPostToSyncPost((callback, state) => AsyncPost(connection, callback, state), timeout);
        }

        public static void AsyncPost(MessageConnection connection, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
            };
            connection.PostNewMessage(MessageId.LOGOUT, messageProcedure, callback, state);
        }
    }
}
