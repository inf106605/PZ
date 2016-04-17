using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes
{
    internal sealed class Int32Transcoder : ITranscoder<Int32>
    {

        private static readonly UInt32Transcoder uint32Transcoder = new UInt32Transcoder();


        public int Read(DataConnection dataConnection)
        {
            UInt32 udata = uint32Transcoder.Read(dataConnection);
            Int32 data = (Int32)udata;
            return data;
        }

        public void Write(DataConnection dataConnection, object writingBlock, int data)
        {
            UInt32 udata = (UInt32)data;
            uint32Transcoder.Write(dataConnection, writingBlock, udata);
        }

    }
}
