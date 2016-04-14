using System;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu
{
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

        public static void PostError(MessageConnection connection, ErrorCode errorCode)
        {
            MessageConnection.MessageProcedure messageProcedure = (object writingBlock) =>
            {
                connection.AsyncWriteData(MessageConnection.errorCodeTranscoder, writingBlock, errorCode);
            };
            connection.PostMessage(MessageId.ERROR, messageProcedure);
        }

    }
}
