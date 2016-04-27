using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes
{
    internal sealed class UInt16Transcoder : ITranscoder<UInt16>
    {

        private static readonly UInt16Transcoder instance = new UInt16Transcoder();
        public static UInt16Transcoder Get
        {
            get { return instance; }
        }


        private UInt16Transcoder()
        {
        }

        public UInt16 Read(DataConnection dataConnection)
        {
            byte[] bytes = dataConnection.SyncReadBytes(2);
            UInt16 data = (UInt16)(((UInt16)bytes[0]) << 8 |
                            ((UInt16)bytes[1]) << 0);
            return data;
        }

        public void Write(DataConnection dataConnection, object writingBlock, UInt16 data)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)(data >> 8);
            bytes[1] = (byte)(data >> 0);
            dataConnection.AsyncWriteBytes(writingBlock, bytes);
        }

    }
}
