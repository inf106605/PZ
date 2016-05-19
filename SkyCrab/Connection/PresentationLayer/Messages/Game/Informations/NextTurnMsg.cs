using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using System;

namespace SkyCrab.Connection.PresentationLayer.Messages.Game.Informations
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.NEXT_TURN"/></para>
    /// <para>Data type: <see cref="UInt32"/> (player ID)</para>
    /// <para>Possible answers: [none]</para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class NextTurnMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.NEXT_TURN; }
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

        public static void AsyncPost(MessageConnection connection, UInt32 playerId)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
                    UInt32Transcoder.Get.Write(connection, writingBlock, playerId);
            connection.PostNewMessage(MessageId.NEXT_TURN, messageProcedure);
        }
    }
}
