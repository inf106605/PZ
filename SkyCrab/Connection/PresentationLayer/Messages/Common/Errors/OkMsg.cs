using System;

namespace SkyCrab.Connection.PresentationLayer.Messages.Common.Errors
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.OK"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Possible answers: [none]</para>
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

        public static void AsyncPost(Int16 id, MessageConnection connection)
        {
            connection.PostAnswerMessage(id, MessageId.OK, null);
        }

    }
}
