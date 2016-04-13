using System;
using System.Text;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders
{
    internal sealed class StringTranscoder : ITranscoder<String>
    {
        private readonly UInt16Transcoder uint16Transcoder;

        public StringTranscoder(UInt16Transcoder uint16Transcoder)
        {
            this.uint16Transcoder = uint16Transcoder;
        }

        public String Read(DataConnection dataConnection)
        {
            UInt16 length = uint16Transcoder.Read(dataConnection);
            byte[] bytes = dataConnection.SyncReadBytes(length);
            string data = Encoding.UTF8.GetString(bytes);
            return data;
        }

        public void Write(DataConnection dataConnection, object writingBlock, String data, DataConnection.Callback callback, object state)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            UInt16 lenght = (UInt16)bytes.Length;
            uint16Transcoder.Write(dataConnection, writingBlock, lenght, null, null);
            dataConnection.AsyncWriteBytes(writingBlock, bytes, callback, state);
        }
    }
}
