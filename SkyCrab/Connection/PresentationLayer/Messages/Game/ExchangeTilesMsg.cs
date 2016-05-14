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
    /// <para>Error codes: <see cref="ErrorCode.NOT_IN_GAME3"/>, <see cref="ErrorCode.NOT_YOUR_TURN2"/>, <see cref="ErrorCode.INCORRECT_MOVE3"/></para>
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
            List<TileWithNumber> tiles = ListTranscoder<TileWithNumber>.Get(TileWithNumberTranscoder.Get).Read(connection);
            return tiles;
        }

        public static void AsyncPost(MessageConnection connection, List<TileWithNumber> tiles, AnswerCallback callback, object state = null)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
                    ListTranscoder<TileWithNumber>.Get(TileWithNumberTranscoder.Get).Write(connection, writingBlock, tiles);
            connection.PostNewMessage(MessageId.EXCHANGE_TILES, messageProcedure,callback,state);
        }

        public static MessageInfo? SyncPost(MessageConnection connection, List<TileWithNumber> tiles, int timeout)
        {
            return AsyncPostToSyncPost((callback, state) => AsyncPost(connection, tiles, callback, state), timeout);
        }

    }
}
