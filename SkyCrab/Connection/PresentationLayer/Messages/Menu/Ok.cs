namespace SkyCrab.Connection.PresentationLayer.Messages.Menu
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.OK"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Passible answers: [none]</para>
    /// </summary>
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

        public static void AsyncPostOk(MessageConnection connection)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
            };
            connection.PostMessage(MessageId.OK, messageProcedure);
        }

    }
}
