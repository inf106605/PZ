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
            data.playerId = UInt32Transcoder.Get.Read(connection);
            data.lettersFromRack = ListTranscoder<LetterWithNumber>.Get(LetterWithNumberTranscoder.Get).Read(connection);
            data.tilesToPlace = ListTranscoder<TileOnBoard>.Get(TileOnBoardTranscoder.Get).Read(connection);
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, TilesToPlace data)
        {
            UInt32Transcoder.Get.Write(connection, writingBlock, data.playerId);
            ListTranscoder<LetterWithNumber>.Get(LetterWithNumberTranscoder.Get).Write(connection, writingBlock, data.lettersFromRack);
            ListTranscoder<TileOnBoard>.Get(TileOnBoardTranscoder.Get).Write(connection, writingBlock, data.tilesToPlace);
        }

    }
}
