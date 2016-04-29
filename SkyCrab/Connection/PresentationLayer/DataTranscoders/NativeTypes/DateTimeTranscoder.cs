using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes
{
    internal sealed class DateTimeTranscoder : ITranscoder<DateTime>
    {

        private static readonly DateTimeTranscoder instance = new DateTimeTranscoder();
        public static DateTimeTranscoder Get
        {
            get { return instance; }
        }


        private DateTimeTranscoder()
        {
        }

        public DateTime Read(EncryptedConnection connection)
        {
            Int64 binary = Int64Transcoder.Get.Read(connection);
            DateTime data = DateTime.FromBinary(binary);
            return data;
        }

        public void Write(EncryptedConnection connection, object writingBlock, DateTime data)
        {
            Int64 binary = data.ToBinary();
            Int64Transcoder.Get.Write(connection, writingBlock, binary);
        }

    }
}
