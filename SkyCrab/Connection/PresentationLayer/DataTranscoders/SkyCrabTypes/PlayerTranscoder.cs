using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes
{
    internal sealed class PlayerTranscoder : ITranscoder<Player>
    {

        private static readonly PlayerTranscoder instance = new PlayerTranscoder();
        public static PlayerTranscoder Get
        {
            get { return instance; }
        }


        private PlayerTranscoder()
        {
        }

        public Player Read(DataConnection dataConnection)
        {
            uint id = UInt32Transcoder.Get.Read(dataConnection);
            PlayerProfile playerProfile = PlayerProfileTranscoder.Get.Read(dataConnection);
            Player player = new Player(id, playerProfile);
            return player;
        }

        public void Write(DataConnection dataConnection, object writingBlock, Player data)
        {
            UInt32Transcoder.Get.Write(dataConnection, writingBlock, data.Id);
            PlayerProfileTranscoder.Get.Write(dataConnection, writingBlock, data.Profile);
        }

    }
}
