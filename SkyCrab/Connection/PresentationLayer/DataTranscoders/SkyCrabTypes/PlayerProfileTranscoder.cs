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

        public PlayerProfile Read(EncryptedConnection connection)
        {
            PlayerProfile playerProfile = new PlayerProfile();
            playerProfile.Login = LimitedStringTranscoder.Get(LengthLimit.Login).Read(connection);
            playerProfile.Password = LimitedStringTranscoder.Get(LengthLimit.Password).Read(connection);
            playerProfile.Nick = LimitedStringTranscoder.Get(LengthLimit.Nick).Read(connection);
            playerProfile.EMail = LimitedStringTranscoder.Get(LengthLimit.EMail).Read(connection);
            playerProfile.Registration = DateTimeTranscoder.Get.Read(connection);
            playerProfile.LastActivity = DateTimeTranscoder.Get.Read(connection);
            return playerProfile;
        }

        public void Write(EncryptedConnection connection, object writingBlock, PlayerProfile data)
        {
            LimitedStringTranscoder.Get(LengthLimit.Login).Write(connection, writingBlock, data.Login);
            LimitedStringTranscoder.Get(LengthLimit.Password).Write(connection, writingBlock, data.Password);
            LimitedStringTranscoder.Get(LengthLimit.Nick).Write(connection, writingBlock, data.Nick);
            LimitedStringTranscoder.Get(LengthLimit.EMail).Write(connection, writingBlock, data.EMail);
            DateTimeTranscoder.Get.Write(connection, writingBlock, data.Registration);
            DateTimeTranscoder.Get.Write(connection, writingBlock, data.LastActivity);
        }

    }
}
