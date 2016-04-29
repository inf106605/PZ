using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Connection.PresentationLayer.MessageConnections;
using System;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.InRooms
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.JOIN_ROOM"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Passible answers: <see cref="RoomMsg"/>, <see cref="ErrorMsg"/></para>
    /// <para>Error codes: <see cref="ErrorCode.NO_SUCH_ROOM"/>, <see cref="ErrorCode.ALREADY_IN_ROOM2"/>, <see cref="ErrorCode.ROOM_IS_FULL"/></para>
    /// </summary>
    public sealed class JoinRoomMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.JOIN_ROOM; }
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
        
        public static MessageInfo? SyncPostLogout(MessageConnection connection, UInt32 roomId, int timeout)
        {
            return SyncPost((callback, state) => AsyncPostLogout(connection, roomId, callback, state), timeout);
        }

        public static void AsyncPostLogout(MessageConnection connection, UInt32 roomId, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
                UInt32Transcoder.Get.Write(connection, writingBlock, roomId);
                connection.SetAnswerCallback(writingBlock, callback, state);
            };
            connection.PostMessage(MessageId.JOIN_ROOM, messageProcedure);
        }
    }
}
