using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using System;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.InRooms
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.PLAYER_LEAVED"/></para>
    /// <para>Data type: <see cref="UInt32"/> (player ID)</para>
    /// <para>Passible answers: [none]</para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class PlayerLeavedMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.PLAYER_LEAVED; }
        }

        internal override bool Answer
        {
            get { return true; }
        }

        internal override object Read(MessageConnection connection)
        {
            UInt32 roomId = UInt32Transcoder.Get.Read(connection);
            return roomId;
        }

        public static void AsyncPostLogout(MessageConnection connection, UInt32 roomId)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
                UInt32Transcoder.Get.Write(connection, writingBlock, roomId);
            };
            connection.PostMessage(MessageId.PLAYER_LEAVED, messageProcedure);
        }
    }
}
