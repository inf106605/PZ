using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders
{
    internal sealed class UInt32Transcoder : ITranscoder<UInt32>
    {

        public UInt32 Read(DataConnection dataConnection)
        {
            byte[] bytes = dataConnection.SyncReadBytes(4);
            UInt32 data = ((UInt32)bytes[0]) << 24 |
                            ((UInt32)bytes[1]) << 16 |
                            ((UInt32)bytes[2]) << 8 |
                            ((UInt32)bytes[3]) << 0;
            return data;
        }

        public void Write(DataConnection dataConnection, object writingBlock, UInt32 data)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(data >> 24);
            bytes[1] = (byte)(data >> 16);
            bytes[2] = (byte)(data >> 8);
            bytes[3] = (byte)(data >> 0);
            dataConnection.AsyncWriteBytes(writingBlock, bytes);
        }

    }
}
