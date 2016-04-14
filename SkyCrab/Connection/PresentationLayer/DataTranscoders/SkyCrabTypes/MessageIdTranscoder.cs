using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Connection.PresentationLayer.Messages;
using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes
{
    internal sealed class MessageIdTranscoder : ITranscoder<MessageId>
    {

        private static readonly UInt8Transcoder uint8Transcoder = new UInt8Transcoder();


        public MessageId Read(DataConnection dataConnection)
        {
            byte id = uint8Transcoder.Read(dataConnection);
            if (!Enum.IsDefined(typeof(MessageId), id))
                throw new ValueIsNotInEnumException();
            MessageId data = (MessageId)id;
            return data;
        }

        public void Write(DataConnection dataConnection, object writingBlock, MessageId data)
        {
            byte id = (byte)data;
            uint8Transcoder.Write(dataConnection, writingBlock, id);
        }

    }
}
