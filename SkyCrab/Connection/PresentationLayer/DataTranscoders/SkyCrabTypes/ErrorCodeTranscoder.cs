using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Connection.PresentationLayer.Messages;
using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes
{
    internal sealed class ErrorCodeTranscoder : ITranscoder<ErrorCode>
    {

        private static readonly ErrorCodeTranscoder instance = new ErrorCodeTranscoder();
        public static ErrorCodeTranscoder Get
        {
            get { return instance; }
        }


        private ErrorCodeTranscoder()
        {
        }

        public ErrorCode Read(DataConnection dataConnection)
        {
            UInt16 code = UInt16Transcoder.Get.Read(dataConnection);
            if (!Enum.IsDefined(typeof(ErrorCode), code))
                throw new ValueIsNotInEnumException();
            ErrorCode data = (ErrorCode)code;
            return data;
        }

        public void Write(DataConnection dataConnection, object writingBlock, ErrorCode data)
        {
            UInt16 code = (UInt16)data;
            UInt16Transcoder.Get.Write(dataConnection, writingBlock, code);
        }

    }
}
