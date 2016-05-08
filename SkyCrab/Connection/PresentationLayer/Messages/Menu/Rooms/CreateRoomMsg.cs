using SkyCrab.Common_classes.Rooms;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Rooms;
using SkyCrab.Connection.PresentationLayer.MessageConnections;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.Rooms
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.CREATE_ROOM"/></para>
    /// <para>Data type: <see cref="Room"/> (without id, owner and players)</para>
    /// <para>Passible answers: <see cref="RoomMsg"/>, <see cref="ErrorMsg"/></para>
    /// <para>Error codes: <see cref="ErrorCode.ALREADY_IN_ROOM"/>, <see cref="ErrorCode.INVALID_RULES"/></para>
    /// </summary>
    public sealed class CreateRoomMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.CREATE_ROOM; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            Room room = RoomTranscoder.Get.Read(connection);
            return room;
        }
        
        public static MessageInfo? SyncPostCreateRoom(MessageConnection connection, Room room, int timeout)
        {
            return SyncPost((callback, state) => AsyncPostCreateRoom(connection, room, callback, state), timeout);
        }

        public static void AsyncPostCreateRoom(MessageConnection connection, Room room, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProc = (writingBlock) =>
            {
                RoomTranscoder.Get.Write(connection, writingBlock, room);
            };
            connection.PostNewMessage(MessageId.CREATE_ROOM, messageProc, callback, state);
        }

    }
}
