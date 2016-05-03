namespace SkyCrab.Connection.PresentationLayer.Messages.Common.Connections
{
    /// <summary>
    /// <para>Sender: Server &amp; Client</para>
    /// <para>ID: <see cref="MessageId.OK_DISCONNECT"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Passible answers: [none]</para>
    /// </summary>
    public sealed class OkDisconnectMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.OK_DISCONNECT; }
        }

        internal override bool Answer
        {
            get { return true; }
        }

        internal override object Read(MessageConnection connection)
        {
            return null;
        }

        public static void AsyncPostOkDisconnect(MessageConnection connection)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
            };
            connection.PostMessage(MessageId.OK_DISCONNECT, messageProcedure);
        }

    }
}
