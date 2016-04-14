using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders
{
    internal sealed class DateTimeTranscoder : ITranscoder<DateTime>
    {

        private static readonly Int64Transcoder int64Transcoder = new Int64Transcoder();


        public DateTime Read(DataConnection dataConnection)
        {
            Int64 binary = int64Transcoder.Read(dataConnection);
            DateTime data = DateTime.FromBinary(binary);
            return data;
        }

        public void Write(DataConnection dataConnection, object writingBlock, DateTime data)
        {
            Int64 binary = data.ToBinary();
            int64Transcoder.Write(dataConnection, writingBlock, binary);
        }

    }
}
