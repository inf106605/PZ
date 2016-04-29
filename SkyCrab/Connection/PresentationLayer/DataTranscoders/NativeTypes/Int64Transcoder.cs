using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes
{
    internal sealed class Int64Transcoder : AbstractTranscoder<Int64>
    {

        private static readonly Int64Transcoder instance = new Int64Transcoder();
        public static Int64Transcoder Get
        {
            get { return instance; }
        }


        private Int64Transcoder()
        {
        }

        public override long Read(EncryptedConnection connection)
        {
            UInt64 udata = UInt64Transcoder.Get.Read(connection);
            Int64 data = (Int64)udata;
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, long data)
        {
            UInt64 udata = (UInt64)data;
            UInt64Transcoder.Get.Write(connection, writingBlock, udata);
        }

    }
}
