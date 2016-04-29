using SkyCrab.Connection.Utils;
using System.Collections.Generic;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes
{
    public sealed class TooLongListException : SkyCrabConnectionException
    {
    }

    internal sealed class ListTranscoder<T> : ITranscoder<List<T>>
    {

        private static readonly Dictionary<ITranscoder<T>, ListTranscoder<T>> instances = new Dictionary<ITranscoder<T>, ListTranscoder<T>>();
        public static ListTranscoder<T> Get(ITranscoder<T> tTranscoder)
        {
            ListTranscoder<T> instance;
            if (instances.TryGetValue(tTranscoder, out instance))
                return instance;
            instance = new ListTranscoder<T>(tTranscoder);
            instances.Add(tTranscoder, instance);
            return instance;
        }

        private ITranscoder<T> tTranscoder;


        private ListTranscoder(ITranscoder<T> tTranscoder)
        {
            this.tTranscoder = tTranscoder;
        }

        public List<T> Read(EncryptedConnection connection)
        {
            List<T> data = new List<T>();
            byte size = UInt8Transcoder.Get.Read(connection);
            for (; size != 0; --size)
            {
                T elem = tTranscoder.Read(connection);
                data.Add(elem);
            }
            return data;
        }

        public void Write(EncryptedConnection connection, object writingBlock, List<T> data)
        {
            if (data.Count > byte.MaxValue)
                throw new TooLongListException();
            byte size = (byte)data.Count;
            UInt8Transcoder.Get.Write(connection, writingBlock, size);
            foreach (T elem in data)
                tTranscoder.Write(connection, writingBlock, elem);
        }

    }
}
