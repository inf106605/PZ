using SkyCrab.Common_classes;
using SkyCrab.Common_classes.Games.Boards;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game
{
    internal sealed class WordOnBoardTranscoder : AbstractTranscoder<WordOnBoard>
    {

        private static readonly WordOnBoardTranscoder instance = new WordOnBoardTranscoder();
        public static WordOnBoardTranscoder Get
        {
            get { return instance; }
        }


        private WordOnBoardTranscoder()
        {
        }

        public override WordOnBoard Read(EncryptedConnection connection)
        {
            WordOnBoard data = new WordOnBoard();
            data.horizonatal = BoolTranscoder.Get.Read(connection);
            data.position = PositionOnBoardTranscoder.Get.Read(connection);
            data.word = LimitedStringTranscoder.Get(LengthLimit.Word).Read(connection);
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, WordOnBoard data)
        {
            BoolTranscoder.Get.Write(connection, writingBlock, data.horizonatal);
            PositionOnBoardTranscoder.Get.Write(connection, writingBlock, data.position);
            LimitedStringTranscoder.Get(LengthLimit.Word).Write(connection, writingBlock, data.word);
        }

    }
}
