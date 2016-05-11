using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Connection.PresentationLayer.MessageConnections;
using System;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.Friends
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.ADD_FRIEND"/></para>
    /// <para>Data type: <see cref="UInt32"/> (player ID)</para>
    /// <para>Possible answers: <see cref="OkMsg"/>, <see cref="ErrorMsg"/></para>
    /// <para>Error codes: <see cref="ErrorCode.NOT_LOGGED4"/>, <see cref="ErrorCode.FRIEND_ALREADY_ADDED"/>, <see cref="ErrorCode.FOREVER_ALONE"/>, <see cref="ErrorCode.NO_SUCH_PLAYER"/></para>
    /// </summary>
    public sealed class AddFriendMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.ADD_FRIEND; }
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
            connection.PostNewMessage(MessageId.ADD_FRIEND, messageProcedure, callback, state);
        }

    }
}
