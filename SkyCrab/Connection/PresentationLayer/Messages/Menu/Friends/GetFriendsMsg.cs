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

        public static MessageInfo? SyncPost(MessageConnection connection, int timeout)
        {
            return AsyncPostToSyncPost((callback, state) => AsyncPost(connection, callback, state), timeout);
        }

        public static void AsyncPost(MessageConnection connection, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProc = (object writingBlock) =>
            {
            };
            connection.PostNewMessage(MessageId.GET_FRIENDS, messageProc, callback, state);
        }

    }
}
