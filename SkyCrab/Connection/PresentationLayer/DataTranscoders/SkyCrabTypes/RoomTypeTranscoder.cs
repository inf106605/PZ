using SkyCrab.Common_classes.Rooms;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes
{
    internal sealed class RoomTypeTranscoder : AbstractTranscoder<RoomType>
    {

        private static readonly RoomTypeTranscoder instance = new RoomTypeTranscoder();
        public static RoomTypeTranscoder Get
        {
            get { return instance; }
        }


        private RoomTypeTranscoder()
        {
        }

        public override RoomType Read(EncryptedConnection connection)
        {
            byte id = UInt8Transcoder.Get.Read(connection);
            if (!Enum.IsDefined(typeof(RoomType), id))
                throw new ValueIsNotInEnumException();
            RoomType data = (RoomType)id;
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, RoomType data)
        {
            byte id = (byte)data;
            UInt8Transcoder.Get.Write(connection, writingBlock, id);
        }

    }
}
