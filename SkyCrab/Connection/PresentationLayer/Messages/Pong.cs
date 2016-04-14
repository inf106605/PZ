namespace SkyCrab.Connection.PresentationLayer.Messages
{
    class Pong : AbstractMessage
    {

        public override MessageId Id
        {
            get
            {
                return MessageId.PONG;
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
            byte number = connection.SyncReadData(MessageConnection.uint8Transcoder);
            return number;
        }

        public static void PostPong(MessageConnection connection, byte number)
        {
            MessageConnection.MessageProcedure messageProcedure = (object writingBlock) =>
            {
                connection.AsyncWriteData(MessageConnection.uint8Transcoder, writingBlock, number);
            };
            connection.PostMessage(MessageId.PONG, messageProcedure); //TODO don't use constant
        }

    }
}
