using SkyCrab.Common_classes;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using System;
using System.Text;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes
{
    internal sealed class LimitedStringTranscoder : AbstractTranscoder<String>
    {

        private static LimitedStringTranscoder instance;
        public static LimitedStringTranscoder Get(LengthLimit limit)
        {
            if (instance == null)
                instance = new LimitedStringTranscoder(limit);
            return instance;
        }

        private LengthLimit limit;


        private LimitedStringTranscoder(LengthLimit limit)
        {
            this.limit = limit;
        }

        public override String Read(EncryptedConnection connection)
        {
            UInt16 length = UInt16Transcoder.Get.Read(connection);
            if (length == 0)
                return "";
            limit.CheckAndThrow(length);
            byte[] bytes = connection.SyncReadBytes(length);
            string data = Encoding.UTF8.GetString(bytes);
            return data;
        }

        internal static string Get(object lenghtLimit)
        {
            throw new NotImplementedException();
        }

        public override void Write(EncryptedConnection connection, object writingBlock, String data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            UInt16 lenght = (UInt16)bytes.Length;
            limit.CheckAndThrow(lenght);
            UInt16Transcoder.Get.Write(connection, writingBlock, lenght);
            if (lenght != 0)
                connection.AsyncWriteBytes(writingBlock, bytes);
        }

    }
}
