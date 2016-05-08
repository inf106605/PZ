using System;

namespace SkyCrab.Connection.PresentationLayer.Messages.Common.Pings
{
    /// <summary>
    /// <para>Sender: Server &amp; Client</para>
    /// <para>ID: <see cref="MessageId.PONG"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Passible answers: [none]</para>
    /// </summary>
    public sealed class PongMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.PONG; }
        }

        internal override bool Answer
        {
            get { return true; }
        }


        internal override object Read(MessageConnection connection)
        {
            return null;
        }

        public static void AsyncPostPong(Int16 id, MessageConnection connection)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
            };
            connection.PostAnswerMessage(id, MessageId.PONG, messageProcedure);
        }

    }
}
