using SkyCrab.Common_classes.Games.Players;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game;

namespace SkyCrab.Connection.PresentationLayer.Messages.Game
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.GAIN_POINTS"/></para>
    /// <para>Data type: <see cref="PlayerPoints"/> (room ID)</para>
    /// <para>Possible answers: [none]</para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class GainPointsMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.GAIN_POINTS; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            PlayerPoints playerPoints = PlayerPointsTranscoder.Get.Read(connection);
            return playerPoints;
        }

        public static void AsyncPost(MessageConnection connection, PlayerPoints playerPoints)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
                    PlayerPointsTranscoder.Get.Write(connection, writingBlock, playerPoints);
            connection.PostNewMessage(MessageId.GAIN_POINTS, messageProcedure);
        }
    }
}
