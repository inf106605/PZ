using System;

namespace SkyCrab.Connection.PresentationLayer.Messages
{
    public sealed class Error : AbstractMessage
    {

        public override MessageId Id
        {
            get
            {
                return MessageId.ERROR;
            }
        }

        internal override bool Answer
        {
            get
            {
                return true;
            }
        }


        internal override object Read(MessageConnection connection)
        {
            UInt16 errorCode = connection.SyncReadData(MessageConnection.uint16Transcoder);
            return errorCode;
        }

        public static void PostError(MessageConnection connection, UInt16 errorCode)
        {
            MessageConnection.MessageProcedure messageProcedure = (object writingBlock) =>
            {
                connection.AsyncWriteData(MessageConnection.uint16Transcoder, writingBlock, errorCode);
            };
            connection.PostMessage(MessageId.ERROR, messageProcedure);
        }

    }
}
