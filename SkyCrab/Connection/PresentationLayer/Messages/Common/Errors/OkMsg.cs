using System;

namespace SkyCrab.Connection.PresentationLayer.Messages.Common.Errors
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.OK"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Passible answers: [none]</para>
    /// </summary>
    public sealed class OkMsg : AbstractMessage
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

        public static void AsyncPost(UInt16 id, MessageConnection connection)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
            };
            connection.PostAnswerMessage(id, MessageId.OK, messageProcedure);
        }

    }
}
