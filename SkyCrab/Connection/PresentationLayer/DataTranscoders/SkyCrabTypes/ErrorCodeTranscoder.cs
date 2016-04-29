using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Connection.PresentationLayer.Messages;
using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes
{
    internal sealed class ErrorCodeTranscoder : AbstractTranscoder<ErrorCode>
    {

        private static readonly ErrorCodeTranscoder instance = new ErrorCodeTranscoder();
        public static ErrorCodeTranscoder Get
        {
            get { return instance; }
        }


        private ErrorCodeTranscoder()
        {
        }

        public override ErrorCode Read(EncryptedConnection connection)
        {
            UInt16 code = UInt16Transcoder.Get.Read(connection);
            if (!Enum.IsDefined(typeof(ErrorCode), code))
                throw new ValueIsNotInEnumException();
            ErrorCode data = (ErrorCode)code;
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, ErrorCode data)
        {
            UInt16 code = (UInt16)data;
            UInt16Transcoder.Get.Write(connection, writingBlock, code);
        }

    }
}
