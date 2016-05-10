using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes
{
    internal sealed class FloatTranscoder : AbstractTranscoder<float>
    {

        private static readonly FloatTranscoder instance = new FloatTranscoder();
        public static FloatTranscoder Get
        {
            get { return instance; }
        }


        private FloatTranscoder()
        {
        }

        public override float Read(EncryptedConnection connection)
        {
            UInt16 integer = UInt16Transcoder.Get.Read(connection);
            float data = (float)integer * 0.001f;
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, float data)
        {
            UInt16 integer = (UInt16)(data * 1000.0f);
            UInt16Transcoder.Get.Write(connection, writingBlock, integer);
        }

    }
}
