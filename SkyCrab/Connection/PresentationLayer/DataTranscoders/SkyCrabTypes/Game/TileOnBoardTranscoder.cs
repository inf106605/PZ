using SkyCrab.Common_classes.Games.Boards;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game
{
    internal sealed class TileOnBoardTranscoder : AbstractTranscoder<TileOnBoard>
    {

        private static readonly TileOnBoardTranscoder instance = new TileOnBoardTranscoder();
        public static TileOnBoardTranscoder Get
        {
            get { return instance; }
        }


        private TileOnBoardTranscoder()
        {
        }

        public override TileOnBoard Read(EncryptedConnection connection)
        {
            TileOnBoard data = new TileOnBoard();
            data.tile = TileTranscoder.Get.Read(connection);
            data.position = PositionOnBoardTranscoder.Get.Read(connection);
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, TileOnBoard data)
        {
            TileTranscoder.Get.Write(connection, writingBlock, data.tile);
            PositionOnBoardTranscoder.Get.Write(connection, writingBlock, data.position);
        }

    }
}
