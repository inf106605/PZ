using SkyCrab.Common_classes.Games.Players;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Game
{
    internal sealed class PlayerPointsTranscoder : AbstractTranscoder<PlayerPoints>
    {

        private static readonly PlayerPointsTranscoder instance = new PlayerPointsTranscoder();
        public static PlayerPointsTranscoder Get
        {
            get { return instance; }
        }


        private PlayerPointsTranscoder()
        {
        }

        public override PlayerPoints Read(EncryptedConnection connection)
        {
            PlayerPoints data = new PlayerPoints();
            data.playerId = UInt32Transcoder.Get.Read(connection);
            data.points = Int32Transcoder.Get.Read(connection);
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, PlayerPoints data)
        {
            UInt32Transcoder.Get.Write(connection, writingBlock, data.playerId);
            Int32Transcoder.Get.Write(connection, writingBlock, data.points);
        }

    }
}
