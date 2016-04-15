using System;
using System.Text;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes
{
    internal sealed class StringTranscoder : ITranscoder<String>
    {

        private static readonly UInt16Transcoder uint16Transcoder = new UInt16Transcoder();


        public String Read(DataConnection dataConnection)
        {
            UInt16 length = uint16Transcoder.Read(dataConnection);
            if (length == 0)
                return "";
            byte[] bytes = dataConnection.SyncReadBytes(length);
            string data = Encoding.UTF8.GetString(bytes);
            return data;
        }

        public void Write(DataConnection dataConnection, object writingBlock, String data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            UInt16 lenght = (UInt16)bytes.Length;
            uint16Transcoder.Write(dataConnection, writingBlock, lenght);
            if (lenght != 0)
                dataConnection.AsyncWriteBytes(writingBlock, bytes);
        }

    }
}
