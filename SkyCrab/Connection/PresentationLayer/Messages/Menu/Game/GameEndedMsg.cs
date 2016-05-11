using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using System;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.InRooms
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.GAME_ENDED"/></para>
    /// <para>Data type: <see cref="UInt32"/> (winner ID)</para>
    /// <para>Possible answers: [none]</para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class GameEndedMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.GAME_ENDED; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            UInt32 winnerId = UInt32Transcoder.Get.Read(connection);
            return winnerId;
        }

        public static void AsyncPost(MessageConnection connection, UInt32 winnerId)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
                    UInt32Transcoder.Get.Write(connection, writingBlock, winnerId);
            connection.PostNewMessage(MessageId.GAME_ENDED, messageProcedure);
        }
    }
}
