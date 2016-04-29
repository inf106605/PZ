using SkyCrab.Common_classes.Rooms;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes;
using System.Collections.Generic;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.ROOM_LIST"/></para>
    /// <para>Data type: <see cref="List{T}"/> of <see cref="Room"/>s</para>
    /// <para>Passible answers: [none]</para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class RoomListMsg : AbstractMessage
    {
        
        public override MessageId Id
        {
            get { return MessageId.ROOM_LIST; }
        }

        internal override bool Answer
        {
            get { return true; }
        }

        internal override object Read(MessageConnection connection)
        {
            List<Room> rooms = ListTranscoder<Room>.Get(RoomTranscoder.Get).Read(connection);
            return rooms;
        }

        public static void AsyncPostRoomList(MessageConnection connection, List<Room> rooms)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
                ListTranscoder<Room>.Get(RoomTranscoder.Get).Write(connection, writingBlock, rooms);
            };
            connection.PostMessage(MessageId.ROOM_LIST, messageProcedure);
        }

    }
}
