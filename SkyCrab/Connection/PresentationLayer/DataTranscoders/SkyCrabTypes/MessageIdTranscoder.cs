using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Connection.PresentationLayer.Messages;
using System;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes
{
    internal sealed class MessageIdTranscoder : AbstractTranscoder<MessageId>
    {

        private static readonly MessageIdTranscoder instance = new MessageIdTranscoder();
        public static MessageIdTranscoder Get
        {
            get { return instance; }
        }


        private MessageIdTranscoder()
        {
        }

        public override MessageId Read(EncryptedConnection connection)
        {
            byte id = UInt8Transcoder.Get.Read(connection);
            if (!Enum.IsDefined(typeof(MessageId), id))
                throw new ValueIsNotInEnumException();
            MessageId data = (MessageId)id;
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, MessageId data)
        {
            byte id = (byte)data;
            UInt8Transcoder.Get.Write(connection, writingBlock, id);
        }

    }
}
