using System;

namespace SkyCrab.Common_classes.Chats
{
    public sealed class ChatMessage
    {

        private UInt32 playerId;
        private string message;

        public UInt32 PlayerId
        {
            get { return playerId; }
            set { playerId = value; }
        }

        public string Message
        {
            get { return message; }
            set
            {
                LengthLimit.ChatMessage.CheckAndThrow(value);
                message = value;
            }
        }

    }
}
