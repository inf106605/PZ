using SkyCrab.Connection.PresentationLayer.MessageConnections;

namespace SkyCrab.Connection.PresentationLayer.Messages.Common.Connections
{
    /// <summary>
    /// <para>Sender: Server &amp; Client</para>
    /// <para>ID: <see cref="MessageId.SHUTDOWN"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Passible answers: [none]</para>
    /// </summary>
    public sealed class ShutdownMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.SHUTDOWN; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            return null;
        }

        public static void AsyncPostShutdown(MessageConnection connection)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
            };
            connection.PostMessage(MessageId.SHUTDOWN, messageProcedure);
        }

    }
}
