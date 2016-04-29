using SkyCrab.Common_classes;
using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes
{
    internal sealed class PlayerProfileTranscoder : AbstractTranscoder<PlayerProfile>
    {

        private static readonly PlayerProfileTranscoder instance = new PlayerProfileTranscoder();
        public static PlayerProfileTranscoder Get
        {
            get { return instance; }
        }


        private PlayerProfileTranscoder()
        {
        }

        public override PlayerProfile Read(EncryptedConnection connection)
        {
            PlayerProfile playerProfile = new PlayerProfile();
            playerProfile.Login = LimitedStringTranscoder.Get(LengthLimit.Login).NullableRead(connection);
            playerProfile.Password = LimitedStringTranscoder.Get(LengthLimit.Password).NullableRead(connection);
            playerProfile.Nick = LimitedStringTranscoder.Get(LengthLimit.Nick).NullableRead(connection);
            playerProfile.EMail = LimitedStringTranscoder.Get(LengthLimit.EMail).NullableRead(connection);
            playerProfile.Registration = DateTimeTranscoder.Get.Read(connection);
            playerProfile.LastActivity = DateTimeTranscoder.Get.Read(connection);
            return playerProfile;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, PlayerProfile data)
        {
            LimitedStringTranscoder.Get(LengthLimit.Login).NullableWrite(connection, writingBlock, data.Login);
            LimitedStringTranscoder.Get(LengthLimit.Password).NullableWrite(connection, writingBlock, data.Password);
            LimitedStringTranscoder.Get(LengthLimit.Nick).NullableWrite(connection, writingBlock, data.Nick);
            LimitedStringTranscoder.Get(LengthLimit.EMail).NullableWrite(connection, writingBlock, data.EMail);
            DateTimeTranscoder.Get.Write(connection, writingBlock, data.Registration);
            DateTimeTranscoder.Get.Write(connection, writingBlock, data.LastActivity);
        }

    }
}
