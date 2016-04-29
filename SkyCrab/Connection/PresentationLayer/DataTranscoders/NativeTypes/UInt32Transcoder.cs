using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes
{
    internal sealed class UInt32Transcoder : ITranscoder<UInt32>
    {

        private static readonly UInt32Transcoder instance = new UInt32Transcoder();
        public static UInt32Transcoder Get
        {
            get { return instance; }
        }


        private UInt32Transcoder()
        {
        }

        public UInt32 Read(EncryptedConnection connection)
        {
            byte[] bytes = connection.SyncReadBytes(4);
            UInt32 data = ((UInt32)bytes[0]) << 24 |
                            ((UInt32)bytes[1]) << 16 |
                            ((UInt32)bytes[2]) << 8 |
                            ((UInt32)bytes[3]) << 0;
            return data;
        }

        public void Write(EncryptedConnection connection, object writingBlock, UInt32 data)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(data >> 24);
            bytes[1] = (byte)(data >> 16);
            bytes[2] = (byte)(data >> 8);
            bytes[3] = (byte)(data >> 0);
            connection.AsyncWriteBytes(writingBlock, bytes);
        }

    }
}
