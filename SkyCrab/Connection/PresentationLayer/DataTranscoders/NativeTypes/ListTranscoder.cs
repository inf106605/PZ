using SkyCrab.Connection.Utils;
using System.Collections.Generic;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes
{
    public sealed class TooLongListException : SkyCrabConnectionException
    {
    }

    internal sealed class ListTranscoder<T> : AbstractTranscoder<List<T>>
    {

        private static ListTranscoder<T> instance;
        public static ListTranscoder<T> Get(AbstractTranscoder<T> tTranscoder)
        {
            if (instance == null)
                instance = new ListTranscoder<T>(tTranscoder);
            return instance;
        }

        private AbstractTranscoder<T> tTranscoder;


        private ListTranscoder(AbstractTranscoder<T> tTranscoder)
        {
            this.tTranscoder = tTranscoder;
        }

        public override List<T> Read(EncryptedConnection connection)
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

        public override void Write(EncryptedConnection connection, object writingBlock, List<T> data)
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
