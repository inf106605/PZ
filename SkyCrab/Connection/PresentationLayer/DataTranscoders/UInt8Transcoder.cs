namespace SkyCrab.Connection.PresentationLayer.DataTranscoders
{
    internal sealed class UInt8Transcoder : ITranscoder<byte>
    {
        public byte Read(DataConnection dataConnection)
        {
            byte[] bytes = dataConnection.SyncReadBytes(1);
            byte data = bytes[0];
            return data;
        }

        public void Write(DataConnection dataConnection, object writingBlock, byte data, DataConnection.Callback callback, object state)
        {
            byte[] bytes = new byte[1] { data };
            dataConnection.AsyncWriteBytes(writingBlock, bytes, callback, state);
        }
    }
}
