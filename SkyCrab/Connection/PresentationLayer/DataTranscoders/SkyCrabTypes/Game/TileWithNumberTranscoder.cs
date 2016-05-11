using SkyCrab.Common_classes.Games.Racks;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game
{
    internal sealed class TileWithNumberTranscoder : AbstractTranscoder<TileWithNumber>
    {

        private static readonly TileWithNumberTranscoder instance = new TileWithNumberTranscoder();
        public static TileWithNumberTranscoder Get
        {
            get { return instance; }
        }


        private TileWithNumberTranscoder()
        {
        }

        public override TileWithNumber Read(EncryptedConnection connection)
        {
            TileWithNumber data = new TileWithNumber();
            data.tile = TileTranscoder.Get.Read(connection);
            data.number = UInt8Transcoder.Get.Read(connection);
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, TileWithNumber data)
        {
            TileTranscoder.Get.Write(connection, writingBlock, data.tile);
            UInt8Transcoder.Get.Write(connection, writingBlock, data.number);
        }

    }
}
