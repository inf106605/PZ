using SkyCrab.Common_classes.Games.Boards;
using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game
{
    internal sealed class TilesToPlaceTranscoder : AbstractTranscoder<TilesToPlace>
    {

        private static readonly TilesToPlaceTranscoder instance = new TilesToPlaceTranscoder();
        public static TilesToPlaceTranscoder Get
        {
            get { return instance; }
        }


        private TilesToPlaceTranscoder()
        {
        }

        public override TilesToPlace Read(EncryptedConnection connection)
        {
            TilesToPlace data = new TilesToPlace();
            data.tilesFromRack = ListTranscoder<TileWithNumber>.Get(TileWithNumberTranscoder.Get).Read(connection);
            data.tilesToPlace = ListTranscoder<TileOnBoard>.Get(TileOnBoardTranscoder.Get).Read(connection);
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, TilesToPlace data)
        {
            ListTranscoder<TileWithNumber>.Get(TileWithNumberTranscoder.Get).Write(connection, writingBlock, data.tilesFromRack);
            ListTranscoder<TileOnBoard>.Get(TileOnBoardTranscoder.Get).Write(connection, writingBlock, data.tilesToPlace);
        }

    }
}
