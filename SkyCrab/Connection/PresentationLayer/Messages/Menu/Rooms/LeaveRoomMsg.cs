using SkyCrab.Connection.PresentationLayer.MessageConnections;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.Rooms
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.LEAVE_ROOM"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Possible answers: <see cref="OkMsg"/>, <see cref="ErrorMsg"/></para>
    /// <para>Error codes: <see cref="ErrorCode.NOT_IN_ROOM"/></para>
    /// </summary>
    public sealed class LeaveRoomMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.LEAVE_ROOM; }
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
            connection.PostNewMessage(MessageId.LEAVE_ROOM, null, callback, state);
        }
    }
}
