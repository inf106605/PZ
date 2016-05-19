using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game;

namespace SkyCrab.Connection.PresentationLayer.Messages.Game.Informations
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.LOSS_TILES"/></para>
    /// <para>Data type: <see cref="LostLetters"/>s</para>
    /// <para>Possible answers: [none]</para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class LossTilesMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.LOSS_TILES; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            LostLetters lostLetters = LostTilesTranscoder.Get.Read(connection);
            return lostLetters;
        }

        public static void AsyncPost(MessageConnection connection, LostLetters lostLetters)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
                    LostTilesTranscoder.Get.Write(connection, writingBlock, lostLetters);
            connection.PostNewMessage(MessageId.LOSS_TILES, messageProcedure);
        }
    }
}
