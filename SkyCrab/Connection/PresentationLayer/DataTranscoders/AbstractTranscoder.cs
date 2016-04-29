using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders
{
    internal abstract class AbstractTranscoder<T>
    {

        public abstract T Read(EncryptedConnection connection);

        public abstract void Write(EncryptedConnection connection, object writingBlock, T data);

        public T NullableRead(EncryptedConnection connection)
        {
            bool isNotNull = BoolTranscoder.Get.Read(connection);
            if (isNotNull)
                return Read(connection);
            else
                return default(T);
        }

        public void NullableWrite(EncryptedConnection connection, object writingBlock, T data)
        {
            bool isNotNull = data != null;
            BoolTranscoder.Get.Write(connection, writingBlock, isNotNull);
            if (isNotNull)
                Write(connection, writingBlock, data);
        }

    }
}
