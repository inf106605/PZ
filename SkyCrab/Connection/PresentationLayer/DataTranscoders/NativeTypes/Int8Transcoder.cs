namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes
{
    internal sealed class Int8Transcoder : ITranscoder<sbyte>
    {

        private static readonly Int8Transcoder instance = new Int8Transcoder();
        public static Int8Transcoder Get
        {
            get { return instance; }
        }


        private Int8Transcoder()
        {
        }

        public sbyte Read(EncryptedConnection connection)
        {
            byte[] bytes = connection.SyncReadBytes(1);
            sbyte result = (sbyte)bytes[0];
            return result;
        }

        public void Write(EncryptedConnection connection, object writingBlock, sbyte data)
        {
            byte[] bytes = new byte[1] { (byte)data };
            connection.AsyncWriteBytes(writingBlock, bytes);
        }

    }
}
