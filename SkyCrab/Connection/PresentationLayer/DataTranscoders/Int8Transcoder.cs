namespace SkyCrab.Connection.PresentationLayer.DataTranscoders
{
    internal sealed class Int8Transcoder : ITranscoder<sbyte>
    {

        public sbyte Read(DataConnection dataConnection)
        {
            byte[] bytes = dataConnection.SyncReadBytes(1);
            sbyte result = (sbyte)bytes[0];
            return result;
        }

        public void Write(DataConnection dataConnection, object writingBlock, sbyte data)
        {
            byte[] bytes = new byte[1] { (byte)data };
            dataConnection.AsyncWriteBytes(writingBlock, bytes);
        }

    }
}
