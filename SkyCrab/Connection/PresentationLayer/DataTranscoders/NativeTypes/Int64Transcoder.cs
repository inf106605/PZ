using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes
{
    internal sealed class Int64Transcoder : ITranscoder<Int64>
    {

        private static readonly Int64Transcoder instance = new Int64Transcoder();
        public static Int64Transcoder Get
        {
            get { return instance; }
        }


        private Int64Transcoder()
        {
        }

        public long Read(DataConnection dataConnection)
        {
            UInt64 udata = UInt64Transcoder.Get.Read(dataConnection);
            Int64 data = (Int64)udata;
            return data;
        }

        public void Write(DataConnection dataConnection, object writingBlock, long data)
        {
            UInt64 udata = (UInt64)data;
            UInt64Transcoder.Get.Write(dataConnection, writingBlock, udata);
        }

    }
}
