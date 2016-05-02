using SkyCrab.Connection.PresentationLayer.MessageConnections;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.Friends
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.GET_FRIENDS"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Passible answers: <see cref="PlayerListMsg"/>, <see cref="ErrorMsg"/></para>
    /// <para>Error codes: <see cref="ErrorCode.NOT_LOGGED3"/></para>
    /// </summary>
    public sealed class GetFriendsMsg : AbstractMessage
    {
        
        public override MessageId Id
        {
            get { return MessageId.GET_FRIENDS; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            return null;
        }

        public static MessageInfo? SyncPostGetFriends(MessageConnection connection, int timeout)
        {
            return SyncPost((callback, state) => AsyncPostGetFriends(connection, callback, state), timeout);
        }

        public static void AsyncPostGetFriends(MessageConnection connection, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProc = (object writingBlock) =>
            {
                connection.SetAnswerCallback(writingBlock, callback, state);
            };
            connection.PostMessage(MessageId.GET_FRIENDS, messageProc);
        }

    }
}
