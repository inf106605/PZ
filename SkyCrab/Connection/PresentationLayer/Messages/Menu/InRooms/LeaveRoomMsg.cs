﻿using SkyCrab.Connection.PresentationLayer.MessageConnections;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.InRooms
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.LEAVE_ROOM"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Passible answers: <see cref="OkMsg"/>, <see cref="ErrorMsg"/></para>
    /// <para>Error codes: <see cref="ErrorCode.NOT_IN_ROOM"/></para>
    /// </summary>
    public sealed class LeaveRoomMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.LEAVE_ROOM; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            return null;
        }
        
        public static MessageInfo? SyncPostLogout(MessageConnection connection, int timeout)
        {
            return SyncPost((callback, state) => AsyncPostLogout(connection, callback, state), timeout);
        }

        public static void AsyncPostLogout(MessageConnection connection, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
                connection.SetAnswerCallback(writingBlock, callback, state);
            };
            connection.PostMessage(MessageId.LEAVE_ROOM, messageProcedure);
        }
    }
}