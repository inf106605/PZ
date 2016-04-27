namespace SkyCrab.Connection.PresentationLayer.DataTranscoders
{
    internal interface ITranscoder<T>
    {
        T Read(EncryptedConnection connection);
        void Write(EncryptedConnection connection, object writingBlock, T data);
    }
}
