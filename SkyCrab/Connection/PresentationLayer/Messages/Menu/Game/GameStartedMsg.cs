using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using System;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.InRooms
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.GAME_STARTED"/></para>
    /// <para>Data type: <see cref="UInt32"/> (room ID)</para>
    /// <para>Possible answers: [none]</para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class GameStartedMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.GAME_STARTED; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            UInt32 roomId = UInt32Transcoder.Get.Read(connection);
            return roomId;
        }

        public static void AsyncPost(MessageConnection connection, UInt32 roomId)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
                UInt32Transcoder.Get.Write(connection, writingBlock, roomId);
            };
            connection.PostNewMessage(MessageId.GAME_STARTED, messageProcedure);
        }
    }
}
