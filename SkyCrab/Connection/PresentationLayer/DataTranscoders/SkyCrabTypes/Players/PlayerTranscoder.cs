using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Players
{
    internal sealed class PlayerTranscoder : AbstractTranscoder<Player>
    {

        private static readonly PlayerTranscoder instance = new PlayerTranscoder();
        public static PlayerTranscoder Get
        {
            get { return instance; }
        }


        private PlayerTranscoder()
        {
        }

        public override Player Read(EncryptedConnection connection)
        {
            uint id = UInt32Transcoder.Get.Read(connection);
            PlayerProfile playerProfile = PlayerProfileTranscoder.Get.Read(connection);
            Player player = new Player(id, playerProfile);
            return player;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, Player data)
        {
            UInt32Transcoder.Get.Write(connection, writingBlock, data.Id);
            PlayerProfileTranscoder.Get.Write(connection, writingBlock, data.Profile);
        }

    }
}
