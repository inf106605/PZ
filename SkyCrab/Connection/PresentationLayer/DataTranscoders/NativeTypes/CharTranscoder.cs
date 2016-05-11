using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes
{
    internal sealed class CharTranscoder : AbstractTranscoder<Char>
    {

        private static readonly CharTranscoder instance = new CharTranscoder();
        public static CharTranscoder Get
        {
            get { return instance; }
        }


        private CharTranscoder()
        {
        }

        public override char Read(EncryptedConnection connection)
        {
            byte[] bytes = new byte[2];
            bytes[0] = UInt8Transcoder.Get.Read(connection);
            bytes[1] = UInt8Transcoder.Get.Read(connection);
            Char data = BitConverter.ToChar(bytes, 0);
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, char data)
        {
            byte[] bytes = BitConverter.GetBytes(data);
            UInt8Transcoder.Get.Write(connection, writingBlock, bytes[0]);
            UInt8Transcoder.Get.Write(connection, writingBlock, bytes[1]);
        }

    }
}
