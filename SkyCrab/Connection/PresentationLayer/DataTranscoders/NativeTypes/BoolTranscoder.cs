using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes
{
    internal sealed class BoolTranscoder : AbstractTranscoder<bool>
    {

        private enum Value : byte
        {
            FALSE = 0,
            TRUE = 1
        }


        private static readonly BoolTranscoder instance = new BoolTranscoder();
        public static BoolTranscoder Get
        {
            get { return instance; }
        }


        private BoolTranscoder()
        {
        }


        public override bool Read(EncryptedConnection connection)
        {
            byte value = UInt8Transcoder.Get.Read(connection);
            switch (value)
            {
                case (byte)Value.TRUE:
                    return true;

                case (byte)Value.FALSE:
                    return false;

                default:
                    throw new ValueIsNotInEnumException();
            }
        }

        public override void Write(EncryptedConnection connection, object writingBlock, bool data)
        {
            if (data)
                UInt8Transcoder.Get.Write(connection, writingBlock, (byte)Value.TRUE);
            else
                UInt8Transcoder.Get.Write(connection, writingBlock, (byte)Value.FALSE);
        }

    }
}
