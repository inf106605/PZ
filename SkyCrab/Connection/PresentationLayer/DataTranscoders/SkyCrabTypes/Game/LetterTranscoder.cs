using SkyCrab.Common_classes.Games.Letters;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game
{
    internal sealed class LetterTranscoder : AbstractTranscoder<Letter>
    {

        private static readonly LetterTranscoder instance = new LetterTranscoder();
        public static LetterTranscoder Get
        {
            get { return instance; }
        }


        private LetterTranscoder()
        {
        }

        public override Letter Read(EncryptedConnection connection)
        {
            char character = CharTranscoder.Get.Read(connection);
            UInt32 points = UInt32Transcoder.Get.Read(connection);
            Letter data = new Letter(character, points);
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, Letter data)
        {
            CharTranscoder.Get.Write(connection, writingBlock, data.character);
            UInt32Transcoder.Get.Write(connection, writingBlock, data.points);
        }

    }
}
