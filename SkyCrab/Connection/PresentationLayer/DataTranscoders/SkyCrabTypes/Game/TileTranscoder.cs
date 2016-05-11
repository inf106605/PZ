using SkyCrab.Common_classes.Games.Letters;
using SkyCrab.Common_classes.Games.Tiles;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game
{
    internal sealed class TileTranscoder : AbstractTranscoder<Tile>
    {

        private static readonly TileTranscoder instance = new TileTranscoder();
        public static TileTranscoder Get
        {
            get { return instance; }
        }


        private TileTranscoder()
        {
        }

        public override Tile Read(EncryptedConnection connection)
        {
            bool blank = BoolTranscoder.Get.Read(connection);
            Letter letter = LetterTranscoder.Get.Read(connection);
            Tile data = new Tile(blank, letter);
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, Tile data)
        {
            BoolTranscoder.Get.Write(connection, writingBlock, data.Blank);
            LetterTranscoder.Get.Write(connection, writingBlock, data.Letter);
        }

    }
}
