using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders
{
    internal sealed class Int64Transcoder : ITranscoder<Int64>
    {

        private static readonly UInt64Transcoder uint64Transcoder = new UInt64Transcoder();


        public long Read(DataConnection dataConnection)
        {
            UInt64 udata = uint64Transcoder.Read(dataConnection);
            Int64 data = (Int64)udata;
            return data;
        }

        public void Write(DataConnection dataConnection, object writingBlock, long data)
        {
            UInt64 udata = (UInt64)data;
            uint64Transcoder.Write(dataConnection, writingBlock, udata);
        }

    }
}
