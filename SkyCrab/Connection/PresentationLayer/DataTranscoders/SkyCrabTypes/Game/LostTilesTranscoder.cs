using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game
{
    internal sealed class LostTilesTranscoder : AbstractTranscoder<LostLetters>
    {

        private static readonly LostTilesTranscoder instance = new LostTilesTranscoder();
        public static LostTilesTranscoder Get
        {
            get { return instance; }
        }


        private LostTilesTranscoder()
        {
        }

        public override LostLetters Read(EncryptedConnection connection)
        {
            LostLetters data = new LostLetters();
            data.playerId = UInt32Transcoder.Get.Read(connection);
            data.letters = ListTranscoder<TileWithNumber>.Get(TileWithNumberTranscoder.Get).Read(connection);
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, LostLetters data)
        {
            UInt32Transcoder.Get.Write(connection, writingBlock, data.playerId);
            ListTranscoder<TileWithNumber>.Get(TileWithNumberTranscoder.Get).Write(connection, writingBlock, data.letters);
        }

    }
}
