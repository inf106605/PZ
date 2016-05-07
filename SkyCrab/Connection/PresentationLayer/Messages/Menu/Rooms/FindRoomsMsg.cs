using SkyCrab.Common_classes.Rooms;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Rooms;
using SkyCrab.Connection.PresentationLayer.MessageConnections;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.Rooms
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.FIND_ROOMS"/></para>
    /// <para>Data type: <see cref="Room"/> (room filter)</para>
    /// <para>Passible answers: <see cref="RoomListMsg"/></para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class FindRoomsMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.FIND_ROOMS; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            Room roomFilter = RoomTranscoder.Get.Read(connection);
            return roomFilter;
        }

        public static MessageInfo? SyncPostFindRooms(MessageConnection connection, Room roomFilter, int timeout)
        {
            return SyncPost((callback, state) => AsyncPostFindRooms(connection, roomFilter, callback, state), timeout);
        }

        public static void AsyncPostFindRooms(MessageConnection connection, Room roomFilter, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProc = (object writingBlock) =>
            {
                RoomTranscoder.Get.Write(connection, writingBlock, roomFilter);
            };
            connection.PostMessage(MessageId.FIND_ROOMS, messageProc, callback, state);
        }

    }
}
