using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game
{
    internal sealed class LetterWithNumberTranscoder : AbstractTranscoder<LetterWithNumber>
    {

        private static readonly LetterWithNumberTranscoder instance = new LetterWithNumberTranscoder();
        public static LetterWithNumberTranscoder Get
        {
            get { return instance; }
        }


        private LetterWithNumberTranscoder()
        {
        }

        public override LetterWithNumber Read(EncryptedConnection connection)
        {
            LetterWithNumber data = new LetterWithNumber();
            data.letter = LetterTranscoder.Get.Read(connection);
            data.number = UInt8Transcoder.Get.Read(connection);
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, LetterWithNumber data)
        {
            LetterTranscoder.Get.Write(connection, writingBlock, data.letter);
            UInt8Transcoder.Get.Write(connection, writingBlock, data.number);
        }

    }
}
