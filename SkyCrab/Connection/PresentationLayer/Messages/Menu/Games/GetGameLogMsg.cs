using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Connection.PresentationLayer.MessageConnections;
using System;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.Games
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.GET_GAME_LOG"/></para>
    /// <para>Data type: <see cref="UInt32"/> (game ID)</para>
    /// <para>Possible answers: <see cref="GameLogMsg"/>, <see cref="ErrorMsg"/></para>
    /// <para>Error codes: <see cref="ErrorCode.NO_SUCH_GAME"/>, <see cref="ErrorCode.GAME_NOT_ENDED"/></para>
    /// </summary>
    public sealed class GetGameLogMsg : AbstractMessage
    {
        
        public override MessageId Id
        {
            get { return MessageId.GET_GAME_LOG; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            UInt32 gameId = UInt32Transcoder.Get.Read(connection);
            return gameId;
        }

        public static MessageInfo? SyncPost(MessageConnection connection, UInt32 gameId, int timeout)
        {
            return AsyncPostToSyncPost((callback, state) => AsyncPost(connection, gameId, callback, state), timeout);
        }

        public static void AsyncPost(MessageConnection connection, UInt32 gameId, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProc = (writingBlock) =>
                UInt32Transcoder.Get.Write(connection, writingBlock, gameId);
            connection.PostNewMessage(MessageId.GET_GAME_LOG, messageProc, callback, state);
        }

    }
}
