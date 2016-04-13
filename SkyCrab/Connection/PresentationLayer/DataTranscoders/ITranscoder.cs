namespace SkyCrab.Connection.PresentationLayer.DataTranscoders
{
    internal interface ITranscoder<T>
    {
        T Read(DataConnection dataConnection);
        void Write(DataConnection dataConnection, object writingBlock, T data, DataConnection.Callback callback, object state);
    }
}
