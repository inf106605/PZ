namespace SkyCrab.Connection.PresentationLayer.Messages.Game.Informations
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.PLAYER_PASSED"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Possible answers: [none]</para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class PlayerPassedMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.PLAYER_PASSED; }
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
            connection.PostNewMessage(MessageId.PLAYER_PASSED, null);
        }
    }
}
