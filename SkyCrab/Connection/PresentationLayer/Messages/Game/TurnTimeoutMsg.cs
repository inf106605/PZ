namespace SkyCrab.Connection.PresentationLayer.Messages.Game
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.TURN_TIMEOUT"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Possible answers: [none]</para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class TurnTimeoutMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.TURN_TIMEOUT; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            return null;
        }

        public static void AsyncPost(MessageConnection connection)
        {
            connection.PostNewMessage(MessageId.TURN_TIMEOUT, null);
        }
    }
}
