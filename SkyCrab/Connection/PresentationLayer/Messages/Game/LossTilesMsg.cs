using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game;
using System.Collections.Generic;

namespace SkyCrab.Connection.PresentationLayer.Messages.Game
{
    /// <summary>
    /// <para>Sender: Server</para>
    /// <para>ID: <see cref="MessageId.LOSS_TILES"/></para>
    /// <para>Data type: <see cref="List{T}"/> of <see cref="TileWithNumber"/>s</para>
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
            List<TileWithNumber> tiles = ListTranscoder<TileWithNumber>.Get(TileWithNumberTranscoder.Get).Read(connection);
            return tiles;
        }

        public static void AsyncPost(MessageConnection connection, List<TileWithNumber> tiles)
        {
            MessageConnection.MessageProcedure messageProcedure = (writingBlock) =>
                    ListTranscoder<TileWithNumber>.Get(TileWithNumberTranscoder.Get).Write(connection, writingBlock, tiles);
            connection.PostNewMessage(MessageId.LOSS_TILES, messageProcedure);
        }
    }
}
