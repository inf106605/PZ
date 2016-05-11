using SkyCrab.Common_classes.Games.Boards;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game
{
    internal sealed class PositionOnBoardTranscoder : AbstractTranscoder<PositionOnBoard>
    {

        private static readonly PositionOnBoardTranscoder instance = new PositionOnBoardTranscoder();
        public static PositionOnBoardTranscoder Get
        {
            get { return instance; }
        }


        private PositionOnBoardTranscoder()
        {
        }

        public override PositionOnBoard Read(EncryptedConnection connection)
        {
            PositionOnBoard positionOnBoard = new PositionOnBoard();
            positionOnBoard.x = Int32Transcoder.Get.Read(connection);
            positionOnBoard.y = Int32Transcoder.Get.Read(connection);
            return positionOnBoard;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, PositionOnBoard data)
        {
            Int32Transcoder.Get.Write(connection, writingBlock, data.x);
            Int32Transcoder.Get.Write(connection, writingBlock, data.y);
        }

    }
}
