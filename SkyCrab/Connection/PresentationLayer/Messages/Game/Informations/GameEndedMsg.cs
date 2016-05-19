namespace SkyCrab.Connection.PresentationLayer.Messages.Game.Informations
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.GAME_ENDED"/></para>
    /// <para>Data type: [none]</para>
    /// <para>Possible answers: [none]</para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class GameEndedMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.GAME_ENDED; }
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
            connection.PostNewMessage(MessageId.GAME_ENDED, null);
        }
    }
}
