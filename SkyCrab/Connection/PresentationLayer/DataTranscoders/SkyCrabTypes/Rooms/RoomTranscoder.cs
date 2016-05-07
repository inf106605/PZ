using SkyCrab.Common_classes;
using SkyCrab.Common_classes.Players;
using SkyCrab.Common_classes.Rooms;
using SkyCrab.Common_classes.Rooms.Rules;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Players;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Rooms.Rules;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Rooms
{
    internal sealed class RoomTranscoder : AbstractTranscoder<Room>
    {

        private static readonly RoomTranscoder instance = new RoomTranscoder();
        public static RoomTranscoder Get
        {
            get { return instance; }
        }


        private RoomTranscoder()
        {
        }

        public override Room Read(EncryptedConnection connection)
        {
            uint id = UInt32Transcoder.Get.Read(connection);
            string name = LimitedStringTranscoder.Get(LengthLimit.RoomName).Read(connection);
            RoomType roomType = RoomTypeTranscoder.Get.Read(connection);
            RuleSet rules = RuleSetTranscoder.Get.Read(connection);
            Room data = new Room(id, name, roomType, rules);
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, Room data)
        {
            UInt32Transcoder.Get.Write(connection, writingBlock, data.Id);
            LimitedStringTranscoder.Get(LengthLimit.RoomName).Write(connection, writingBlock, data.Name);
            RoomTypeTranscoder.Get.Write(connection, writingBlock, data.RoomType);
            RuleSetTranscoder.Get.Write(connection, writingBlock, data.Rules);
        }

    }
}
