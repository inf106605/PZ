namespace SkyCrab.Connection.PresentationLayer.Messages.Menu
{
    public sealed class Ok : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.OK; }
        }

        internal override bool Answer
        {
            get { return true; }
        }


        internal override object Read(MessageConnection connection)
        {
            return null;
        }

        public static void PostOk(MessageConnection connection)
        {
            MessageConnection.MessageProcedure messageProcedure = (object writingBlock) =>
            {
            };
            connection.PostMessage(MessageId.OK, messageProcedure);
        }

    }
}
