namespace SkyCrab.Connection.PresentationLayer.Messages.Menu
{
    /// <summary>
    /// <para>Sender: <see cref="MessageConnection"/></para>
    /// <para>ID: <see cref="MessageId.NO_PONG"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Passible answers: [none]</para>
    /// <para>It's a pseudo-message which is enqueued by <see cref="MessageConnection"/> when it is no answer to message <see cref="PingMsg"/>.</para>
    /// </summary>
    public sealed class NoPongMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.NO_PONG; }
        }

        internal override bool Answer
        {
            get { return true; }
        }


        internal override object Read(MessageConnection connection)
        {
            return null;
        }

    }
}
