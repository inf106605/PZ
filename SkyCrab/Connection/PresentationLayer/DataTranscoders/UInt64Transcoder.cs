using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders
{
    internal sealed class UInt64Transcoder : ITranscoder<UInt64>
    {

        public ulong Read(DataConnection dataConnection)
        {
            byte[] bytes = dataConnection.SyncReadBytes(4);
            UInt64 data = ((UInt64)bytes[0]) << 56 |
                            ((UInt64)bytes[1]) << 48 |
                            ((UInt64)bytes[2]) << 40 |
                            ((UInt64)bytes[3]) << 32 |
                            ((UInt64)bytes[4]) << 24 |
                            ((UInt64)bytes[5]) << 16 |
                            ((UInt64)bytes[6]) << 8 |
                            ((UInt64)bytes[7]) << 0;
            return data;
        }

        public void Write(DataConnection dataConnection, object writingBlock, ulong data)
        {
            byte[] bytes = new byte[8];
            bytes[0] = (byte)(data >> 56);
            bytes[1] = (byte)(data >> 48);
            bytes[2] = (byte)(data >> 40);
            bytes[3] = (byte)(data >> 32);
            bytes[4] = (byte)(data >> 24);
            bytes[5] = (byte)(data >> 16);
            bytes[6] = (byte)(data >> 8);
            bytes[7] = (byte)(data >> 0);
            dataConnection.AsyncWriteBytes(writingBlock, bytes);
        }

    }
}
