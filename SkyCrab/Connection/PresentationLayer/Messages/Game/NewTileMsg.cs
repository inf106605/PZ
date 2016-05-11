using SkyCrab.Common_classes.Games.Letters;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game;
using System.Collections.Generic;

namespace SkyCrab.Connection.PresentationLayer.Messages.Game
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.NEW_TILES"/></para>
    /// <para>Data type: <see cref="List{T}"/> of <see cref="Letter"/>s</para>
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
            List<Letter> letters = ListTranscoder<Letter>.Get(LetterTranscoder.Get).Read(connection);
            return letters;
        }

        public static void AsyncPost(MessageConnection connection, List<Letter> letters)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
                    ListTranscoder<Letter>.Get(LetterTranscoder.Get).Write(connection, writingBlock, letters);
            connection.PostNewMessage(MessageId.NEW_TILES, messageProcedure);
        }
    }
}
