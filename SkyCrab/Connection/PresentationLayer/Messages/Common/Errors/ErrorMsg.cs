using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes;
using System;

namespace SkyCrab.Connection.PresentationLayer.Messages.Common.Errors
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.ERROR"/></para>
    /// <para>Data type: <see cref="ErrorCode"/></para>
    /// <para>Passible answers: [none]</para>
    /// </summary>
    public sealed class ErrorMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.ERROR; }
        }

        internal override bool Answer
        {
            get { return true; }
        }

        internal override object Read(MessageConnection connection)
        {
            ErrorCode errorCode = ErrorCodeTranscoder.Get.Read(connection);
            return errorCode;
        }

        public static void AsyncPostError(Int16 id, MessageConnection connection, ErrorCode errorCode)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
                ErrorCodeTranscoder.Get.Write(connection, writingBlock, errorCode);
            };
            connection.PostAnswerMessage(id, MessageId.ERROR, messageProcedure);
        }

    }
}
