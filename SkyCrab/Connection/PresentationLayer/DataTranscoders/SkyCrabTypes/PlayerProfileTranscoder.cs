using SkyCrab.Common_classes;
using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes
{
    internal sealed class PlayerProfileTranscoder : ITranscoder<PlayerProfile>
    {

        private static readonly PlayerProfileTranscoder instance = new PlayerProfileTranscoder();
        public static PlayerProfileTranscoder Get
        {
            get { return instance; }
        }


        private PlayerProfileTranscoder()
        {
        }

        public PlayerProfile Read(DataConnection dataConnection)
        {
            PlayerProfile playerProfile = new PlayerProfile();
            playerProfile.Login = LimitedStringTranscoder.Get(LengthLimit.Login).Read(dataConnection);
            playerProfile.Password = LimitedStringTranscoder.Get(LengthLimit.Password).Read(dataConnection);
            playerProfile.Nick = LimitedStringTranscoder.Get(LengthLimit.Nick).Read(dataConnection);
            playerProfile.EMail = LimitedStringTranscoder.Get(LengthLimit.EMail).Read(dataConnection);
            playerProfile.Registration = DateTimeTranscoder.Get.Read(dataConnection);
            playerProfile.LastActivity = DateTimeTranscoder.Get.Read(dataConnection);
            return playerProfile;
        }

        public void Write(DataConnection dataConnection, object writingBlock, PlayerProfile data)
        {
            LimitedStringTranscoder.Get(LengthLimit.Login).Write(dataConnection, writingBlock, data.Login);
            LimitedStringTranscoder.Get(LengthLimit.Password).Write(dataConnection, writingBlock, data.Password);
            LimitedStringTranscoder.Get(LengthLimit.Nick).Write(dataConnection, writingBlock, data.Nick);
            LimitedStringTranscoder.Get(LengthLimit.EMail).Write(dataConnection, writingBlock, data.EMail);
            DateTimeTranscoder.Get.Write(dataConnection, writingBlock, data.Registration);
            DateTimeTranscoder.Get.Write(dataConnection, writingBlock, data.LastActivity);
        }

    }
}
