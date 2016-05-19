using SkyCrab.Common_classes.Games.Pouches;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game;

namespace SkyCrab.Connection.PresentationLayer.Messages.Game.Informations
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.NEW_TILES"/></para>
    /// <para>Data type: <see cref="DrawedLetters"/>s</para>
    /// <para>Possible answers: [none]</para>
    /// <para>Error codes: [none]</para>
    /// </summary>
    public sealed class NewTilesMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.NEW_TILES; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            DrawedLetters drawedLetters = DrawedTilesTranscoder.Get.Read(connection);
            return drawedLetters;
        }

        public static void AsyncPost(MessageConnection connection, DrawedLetters drawedLetters)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
                    DrawedTilesTranscoder.Get.Write(connection, writingBlock, drawedLetters);
            connection.PostNewMessage(MessageId.NEW_TILES, messageProcedure);
        }
    }
}
