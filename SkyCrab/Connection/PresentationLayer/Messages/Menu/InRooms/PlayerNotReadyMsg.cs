using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using System;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.InRooms
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.PLAYER_NOT_READY"/></para>
    /// <para>Data type: <see cref="UInt32"/> (player ID)</para>
    /// <para>Passible answers: [none]</para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class PlayerNotReadyMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.PLAYER_NOT_READY; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            UInt32 playerId = UInt32Transcoder.Get.Read(connection);
            return playerId;
        }

        public static void AsyncPostLogout(MessageConnection connection, UInt32 playerId)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
                UInt32Transcoder.Get.Write(connection, writingBlock, playerId);
            };
            connection.PostMessage(MessageId.PLAYER_NOT_READY, messageProcedure);
        }
    }
}
