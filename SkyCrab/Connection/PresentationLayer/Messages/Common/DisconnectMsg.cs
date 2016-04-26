namespace SkyCrab.Connection.PresentationLayer.Messages
{
    /// <summary>
    /// <para>Sender: Server &amp; Client</para>
    /// <para>ID: <see cref="MessageId.DISCONNECT"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Passible answers: [none]</para>
    /// </summary>
    public sealed class DisconnectMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.DISCONNECT; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            return null;
        }

        public static void AsyncPostDisconnect(MessageConnection connection)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
            };
            connection.PostMessage(MessageId.DISCONNECT, messageProcedure);
        }

    }
}
