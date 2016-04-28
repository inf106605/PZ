using System;
using System.Text;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes
{
    internal sealed class StringTranscoder : ITranscoder<String>
    {

        private static readonly StringTranscoder instance = new StringTranscoder();
        public static StringTranscoder Get
        {
            get { return instance; }
        }


        private StringTranscoder()
        {
        }

        public String Read(EncryptedConnection connection)
        {
            UInt16 length = UInt16Transcoder.Get.Read(connection);
            if (length == 0)
                return "";
            byte[] bytes = connection.SyncReadBytes(length);
            string data = Encoding.UTF8.GetString(bytes);
            return data;
        }

        public void Write(EncryptedConnection connection, object writingBlock, String data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            UInt16 lenght = (UInt16)bytes.Length;
            UInt16Transcoder.Get.Write(connection, writingBlock, lenght);
            if (lenght != 0)
                connection.AsyncWriteBytes(writingBlock, bytes);
        }

    }
}
