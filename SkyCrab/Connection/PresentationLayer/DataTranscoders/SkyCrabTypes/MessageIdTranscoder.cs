using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Connection.PresentationLayer.Messages;
using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes
{
    internal sealed class MessageIdTranscoder : ITranscoder<MessageId>
    {

        private static readonly MessageIdTranscoder instance = new MessageIdTranscoder();
        public static MessageIdTranscoder Get
        {
            get { return instance; }
        }


        private MessageIdTranscoder()
        {
        }

        public MessageId Read(DataConnection dataConnection)
        {
            byte id = UInt8Transcoder.Get.Read(dataConnection);
            if (!Enum.IsDefined(typeof(MessageId), id))
                throw new ValueIsNotInEnumException();
            MessageId data = (MessageId)id;
            return data;
        }

        public void Write(DataConnection dataConnection, object writingBlock, MessageId data)
        {
            byte id = (byte)data;
            UInt8Transcoder.Get.Write(dataConnection, writingBlock, id);
        }

    }
}
