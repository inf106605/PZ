using SkyCrab.Connection.PresentationLayer.MessageConnections;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.Accounts
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.LOGIN_AS_GUEST"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Possible answers: <see cref="LoginOkMsg"/>, <see cref="ErrorMsg"/></para>
    /// <para>Error codes: <see cref="ErrorCode.SESSION_ALREADY_LOGGED"/></para>
    /// </summary>
    public sealed class LoginAsGuestMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.LOGIN_AS_GUEST; }
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
            connection.PostNewMessage(MessageId.LOGIN_AS_GUEST, null, callback, state);
        }

    }
}
