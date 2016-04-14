using SkyCrab.Connection.PresentationLayer.Messages;
using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders
{
    internal sealed class ErrorCodeTranscoder : ITranscoder<ErrorCode>
    {

        private static readonly UInt16Transcoder uint16Transcoder = new UInt16Transcoder();


        public ErrorCode Read(DataConnection dataConnection)
        {
            UInt16 code = uint16Transcoder.Read(dataConnection);
            if (!Enum.IsDefined(typeof(ErrorCode), code))
                throw new ValueIsNotInEnumException();
            ErrorCode data = (ErrorCode)code;
            return data;
        }

        public void Write(DataConnection dataConnection, object writingBlock, ErrorCode data)
        {
            UInt16 code = (UInt16)data;
            uint16Transcoder.Write(dataConnection, writingBlock, code);
        }

    }
}
