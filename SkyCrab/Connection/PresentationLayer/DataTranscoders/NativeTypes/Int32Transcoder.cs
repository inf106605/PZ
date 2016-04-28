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

        public int Read(EncryptedConnection connection)
        {
            UInt32 udata = UInt32Transcoder.Get.Read(connection);
            Int32 data = (Int32)udata;
            return data;
        }

        public void Write(EncryptedConnection connection, object writingBlock, int data)
        {
            UInt32 udata = (UInt32)data;
            UInt32Transcoder.Get.Write(connection, writingBlock, udata);
        }

    }
}
