using System;

namespace SkyCrab.Connection.PresentationLayer.Messages.Common.Pings
{
    /// <summary>
    /// <para>Sender: Server &amp; Client</para>
    /// <para>ID: <see cref="MessageId.PONG"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Possible answers: [none]</para>
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

        public static void AsyncPost(UInt16 id, MessageConnection connection)
        {
            connection.PostAnswerMessage(id, MessageId.PONG, null);
        }

    }
}
