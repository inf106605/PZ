using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes
{
    internal sealed class Int32Transcoder : ITranscoder<Int32>
    {

        private static readonly Int32Transcoder instance = new Int32Transcoder();
        public static Int32Transcoder Get
        {
            get { return instance; }
        }


        private Int32Transcoder()
        {
        }

        public int Read(DataConnection dataConnection)
        {
            UInt32 udata = UInt32Transcoder.Get.Read(dataConnection);
            Int32 data = (Int32)udata;
            return data;
        }

        public void Write(DataConnection dataConnection, object writingBlock, int data)
        {
            UInt32 udata = (UInt32)data;
            UInt32Transcoder.Get.Write(dataConnection, writingBlock, udata);
        }

    }
}
