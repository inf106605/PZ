using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Players;

namespace SkyCrab.Connection.PresentationLayer.Messages.Menu.InRooms
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.PLAYER_JOINED"/></para>
    /// <para>Data type: <see cref="Player"/></para>
    /// <para>Passible answers: [none]</para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class PlayerJoinedMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.PLAYER_JOINED; }
        }

        internal override bool Answer
        {
            get { return true; }
        }

        internal override object Read(MessageConnection connection)
        {
            Player player = PlayerTranscoder.Get.Read(connection);
            return player;
        }

        public static void asycnPostJoined(MessageConnection connection, Player player)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
            {
                PlayerTranscoder.Get.Write(connection, writingBlock, player);
            };
            connection.PostMessage(MessageId.PLAYER_JOINED, messageProcedure);
        }
    }
}
