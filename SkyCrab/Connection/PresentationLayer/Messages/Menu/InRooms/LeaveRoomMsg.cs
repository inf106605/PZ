using SkyCrab.Connection.PresentationLayer.MessageConnections;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.InRooms
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.LEAVE_ROOM"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Passible answers: <see cref="OkMsg"/>, <see cref="ErrorMsg"/></para>
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
        
        public static MessageInfo? SyncPostLeaveRoom(MessageConnection connection, int timeout)
        {
            return SyncPost((callback, state) => AsyncPostLeaveRoom(connection, callback, state), timeout);
        }

        public static void AsyncPostLeaveRoom(MessageConnection connection, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
            };
            connection.PostMessage(MessageId.LEAVE_ROOM, messageProcedure, callback, state);
        }
    }
}
