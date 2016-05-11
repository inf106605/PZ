using SkyCrab.Connection.PresentationLayer.Messages;
using System;

namespace SkyCrab.Connection.PresentationLayer.MessageConnections
{
    public struct MessageInfo
    {
        public Int16 id;
        public MessageId messageId;
        public object message;
    }
}
