using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Connection.PresentationLayer.MessageConnections;
using System;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.Rooms
{
    /// <summary>
    /// <para>Sender: Client &amp; Server</para>
    /// <para>ID: <see cref="MessageId.PLAYER_READY"/></para>
    /// <para>Data type: <see cref="UInt32"/> (player ID)</para>
    /// <para>Possible answers: <see cref="OkMsg"/>, <see cref="ErrorMsg"/></para>
    /// <para>Error codes: <see cref="ErrorCode.NOT_IN_ROOM2"/></para>
    /// </summary>
    public sealed class PlayerReadyMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.PLAYER_READY; }
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

        public static MessageInfo? SyncPost(MessageConnection connection, UInt32 playerId, int timeout)
        {
            return AsyncPostToSyncPost((callback, state) => AsyncPost(connection, playerId, callback, state), timeout);
        }

        public static void AsyncPost(MessageConnection connection, UInt32 playerId, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
                    UInt32Transcoder.Get.Write(connection, writingBlock, playerId);
            connection.PostNewMessage(MessageId.PLAYER_READY, messageProcedure, callback, state);
        }
    }
}
