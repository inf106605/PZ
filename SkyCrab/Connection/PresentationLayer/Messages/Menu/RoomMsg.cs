using SkyCrab.Common_classes.Rooms;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Rooms;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.ROOM"/></para>
    /// <para>Data type: <see cref="Room"/></para>
    /// <para>Passible answers: [none]</para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class RoomMsg : AbstractMessage
    {
        
        public override MessageId Id
        {
            get { return MessageId.ROOM; }
        }

        internal override bool Answer
        {
            get { return true; }
        }

        internal override object Read(MessageConnection connection)
        {
            Room room = RoomTranscoder.Get.Read(connection);
            return room;
        }

        public static void AsyncPostRoomList(MessageConnection connection, Room room)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
                RoomTranscoder.Get.Write(connection, writingBlock, room);
            };
            connection.PostMessage(MessageId.ROOM, messageProcedure);
        }

    }
}
