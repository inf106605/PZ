using SkyCrab.Common_classes;
using System;
using System.Text;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes
{
    internal sealed class LimitedStringTranscoder : ITranscoder<String>
    {

        private static readonly BoolTranscoder boolTranscoder = new BoolTranscoder();
        private static readonly UInt16Transcoder uint16Transcoder = new UInt16Transcoder();

        private readonly LengthLimit limit;


        public LimitedStringTranscoder(LengthLimit limit)
        {
            this.limit = limit;
        }

        public String Read(DataConnection dataConnection)
        {
            bool isNotNull = boolTranscoder.Read(dataConnection);
            if (!isNotNull)
                return null;
            UInt16 length = uint16Transcoder.Read(dataConnection);
            limit.checkAndThrow(length);
            byte[] bytes = dataConnection.SyncReadBytes(length);
            string data = Encoding.UTF8.GetString(bytes);
            return data;
        }

        public void Write(DataConnection dataConnection, object writingBlock, String data)
        {
            if (limit.Min != 0 && data == null)
            {
                boolTranscoder.Write(dataConnection, writingBlock, false);
                return;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            UInt16 lenght = (UInt16)bytes.Length;
            limit.checkAndThrow(lenght);
            boolTranscoder.Write(dataConnection, writingBlock, true);
            uint16Transcoder.Write(dataConnection, writingBlock, lenght);
            if (lenght != 0)
                dataConnection.AsyncWriteBytes(writingBlock, bytes);
        }

    }
}
