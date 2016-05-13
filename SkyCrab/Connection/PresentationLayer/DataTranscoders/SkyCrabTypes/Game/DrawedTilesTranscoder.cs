using SkyCrab.Common_classes.Games.Letters;
using SkyCrab.Common_classes.Games.Pouches;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game
{
    internal sealed class DrawedTilesTranscoder : AbstractTranscoder<DrawedLetters>
    {

        private static readonly DrawedTilesTranscoder instance = new DrawedTilesTranscoder();
        public static DrawedTilesTranscoder Get
        {
            get { return instance; }
        }


        private DrawedTilesTranscoder()
        {
        }

        public override DrawedLetters Read(EncryptedConnection connection)
        {
            DrawedLetters data = new DrawedLetters();
            data.playerId = UInt32Transcoder.Get.Read(connection);
            data.letters = ListTranscoder<Letter>.Get(LetterTranscoder.Get).Read(connection);
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, DrawedLetters data)
        {
            UInt32Transcoder.Get.Write(connection, writingBlock, data.playerId);
            ListTranscoder<Letter>.Get(LetterTranscoder.Get).Write(connection, writingBlock, data.letters);
        }

    }
}
