using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes
{
    internal sealed class Int16Transcoder : AbstractTranscoder<Int16>
    {

        private static readonly Int16Transcoder instance = new Int16Transcoder();
        public static Int16Transcoder Get
        {
            get { return instance; }
        }


        private Int16Transcoder()
        {
        }

        public override Int16 Read(EncryptedConnection connection)
        {
            UInt16 udata = UInt16Transcoder.Get.Read(connection);
            Int16 data = (Int16)udata;
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, Int16 data)
        {
            UInt16 udata = (UInt16)data;
            UInt16Transcoder.Get.Write(connection, writingBlock, udata);
        }

    }
}
