using SkyCrab.Common_classes.Chats;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Common_classes;

namespace SkyCrab.Connection.PresentationLayer.DataTranscoders.SkyCrabTypes.Rooms
{
    internal sealed class ChatMessageTranscoder : AbstractTranscoder<ChatMessage>
    {

        private static readonly ChatMessageTranscoder instance = new ChatMessageTranscoder();
        public static ChatMessageTranscoder Get
        {
            get { return instance; }
        }


        private ChatMessageTranscoder()
        {
        }

        public override ChatMessage Read(EncryptedConnection connection)
        {
            ChatMessage data = new ChatMessage();
            data.PlayerId = UInt32Transcoder.Get.Read(connection);
            data.Message = LimitedStringTranscoder.Get(LengthLimit.ChatMessage).Read(connection);
            return data;
        }

        public override void Write(EncryptedConnection connection, object writingBlock, ChatMessage data)
        {
            UInt32Transcoder.Get.Write(connection, writingBlock, data.PlayerId);
            LimitedStringTranscoder.Get(LengthLimit.ChatMessage).Write(connection, writingBlock, data.Message);
        }

    }
}
