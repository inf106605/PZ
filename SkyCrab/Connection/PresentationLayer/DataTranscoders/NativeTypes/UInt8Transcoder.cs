namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes
{
    internal sealed class UInt8Transcoder : ITranscoder<byte>
    {

        private static readonly UInt8Transcoder instance = new UInt8Transcoder();
        public static UInt8Transcoder Get
        {
            get { return instance; }
        }


        private UInt8Transcoder()
        {
        }

        public byte Read(EncryptedConnection connection)
        {
            byte[] bytes = connection.SyncReadBytes(1);
            byte data = bytes[0];
            return data;
        }

        public void Write(EncryptedConnection connection, object writingBlock, byte data)
        {
            byte[] bytes = new byte[1] { data };
            connection.AsyncWriteBytes(writingBlock, bytes);
        }

    }
}
