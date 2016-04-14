namespace SkyCrab.Connection.PresentationLayer.Messages
{
    class Ping : AbstractMessage
    {

        public override MessageId Id
        {
            get
            {
                return MessageId.PING;
            }
        }

        internal override bool Answer
        {
            get
            {
                return false;
            }
        }


        internal override object Read(MessageConnection connection)
        {
            byte number = connection.SyncReadData(MessageConnection.uint8Transcoder);
            return number;
        }

        public static void PostPing(MessageConnection connection, byte number, MessageConnection.AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProcedure = (object writingBlock) =>
            {
                connection.AsyncWriteData(MessageConnection.uint8Transcoder, writingBlock, number);
                connection.SetAnswerCallback(writingBlock, callback, state);
            };
            connection.PostMessage(MessageId.PING, messageProcedure); //TODO don't use constant
        }

    }
}
