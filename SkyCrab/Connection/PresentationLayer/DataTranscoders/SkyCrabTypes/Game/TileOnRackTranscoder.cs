using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Common_classes.Games.Tiles;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game
{
    internal sealed class TileOnRackTranscoder : AbstractTranscoder<TileOnRack>
    {

        private static readonly TileOnRackTranscoder instance = new TileOnRackTranscoder();
        public static TileOnRackTranscoder Get
        {
            get { return instance; }
        }


        private TileOnRackTranscoder()
        {
        }

        public override TileOnRack Read(EncryptedConnection connection)
        {
            Tile tile = TileTranscoder.Get.Read(connection);
            float leftPosition = FloatTranscoder.Get.Read(connection);
            TileOnRack data = new TileOnRack(tile, leftPosition);
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, TileOnRack data)
        {
            TileTranscoder.Get.Write(connection, writingBlock, data.Tile);
            FloatTranscoder.Get.Write(connection, writingBlock, data.LeftPosition);
        }

    }
}
