using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game;
using SkyCrab.Connection.PresentationLayer.MessageConnections;
using SkyCrab.Connection.PresentationLayer.Messages.Common.Errors;
using System.Collections.Generic;

namespace SkyCrab.Connection.PresentationLayer.Messages.Game
{
    /// <summary>
    /// <para>Sender: Client</para>
    /// <para>ID: <see cref="MessageId.EXCHANGE_TILES"/></para>
    /// <para>Data type: <see cref="List{T}"/> of <see cref="TileWithNumber"/>s</para>
    /// <para>Possible answers: <see cref="OkMsg"/>, <see cref="ErrorMsg"/></para>
    /// <para>Error codes: <see cref="ErrorCode.NOT_IN_GAME3"/>, <see cref="ErrorCode.NOT_YOUR_TURN2"/>, <see cref="ErrorCode.TOO_LESS_TILES2"/>, <see cref="ErrorCode.LETTERS_NOT_FROM_RACK2"/>, <see cref="ErrorCode.TOO_LESS_POUCH_LETTERS"/>, <see cref="ErrorCode.RESTR_EXCH_VIOLATION"/></para>
    /// </summary>
    public sealed class ExchangeTilesMsg : AbstractMessage
    {

        public override MessageId Id
        {
            get { return MessageId.EXCHANGE_TILES; }
        }

        internal override bool Answer
        {
            get { return false; }
        }

        internal override object Read(MessageConnection connection)
        {
            List<LetterWithNumber> tiles = ListTranscoder<LetterWithNumber>.Get(LetterWithNumberTranscoder.Get).Read(connection);
            return tiles;
        }

        public static void AsyncPost(MessageConnection connection, List<LetterWithNumber> letters, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
                    ListTranscoder<LetterWithNumber>.Get(LetterWithNumberTranscoder.Get).Write(connection, writingBlock, letters);
            connection.PostNewMessage(MessageId.EXCHANGE_TILES, messageProcedure,callback,state);
        }

        public static MessageInfo? SyncPost(MessageConnection connection, List<LetterWithNumber> letters, int timeout)
        {
            return AsyncPostToSyncPost((callback, state) => AsyncPost(connection, letters, callback, state), timeout);
        }

    }
}
