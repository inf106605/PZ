using System;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.ERROR"/></para>
    /// <para>Data type: <see cref="ErrorCode"/></para>
    /// <para>Passible answers: [none]</para>
    /// </summary>
    public sealed class Error : AbstractMessage
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
            ErrorCode errorCode = connection.SyncReadData(MessageConnection.errorCodeTranscoder);
            return errorCode;
        }

        public static void AsyncPostError(MessageConnection connection, ErrorCode errorCode)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
                connection.AsyncWriteData(MessageConnection.errorCodeTranscoder, writingBlock, errorCode);
            };
            connection.PostMessage(MessageId.ERROR, messageProcedure);
        }

    }
}
