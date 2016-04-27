using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes;
using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes
{
    internal sealed class BoolTranscoder : ITranscoder<bool>
    {

        private enum Value : byte
        {
            FALSE = 0,
            TRUE = 1
        }


        private static readonly UInt8Transcoder uint8Transcoder = new UInt8Transcoder();


        public bool Read(DataConnection dataConnection)
        {
            byte value = uint8Transcoder.Read(dataConnection);
            switch (value)
            {
                case (byte)Value.TRUE:
                    return true;

                case (byte)Value.FALSE:
                    return false;

                default:
                    throw new ValueIsNotInEnumException();
            }
        }

        public void Write(DataConnection dataConnection, object writingBlock, bool data)
        {
            if (data)
                uint8Transcoder.Write(dataConnection, writingBlock, (byte)Value.TRUE);
            else
                uint8Transcoder.Write(dataConnection, writingBlock, (byte)Value.FALSE);
        }

    }
}
