using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using System;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.InRooms
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.NEW_ROOM_OWNER"/></para>
    /// <para>Data type: <see cref="UInt32"/> (player ID)</para>
    /// <para>Passible answers: [none]</para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class NewRoomOwnerMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.NEW_ROOM_OWNER; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            UInt32 newOwnerId = UInt32Transcoder.Get.Read(connection);
            return newOwnerId;
        }

        public static void AsyncPostNewOwner(MessageConnection connection, UInt32 newOwnerId)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
                UInt32Transcoder.Get.Write(connection, writingBlock, newOwnerId);
            };
            connection.PostNewMessage(MessageId.NEW_ROOM_OWNER, messageProcedure);
        }
    }
}
