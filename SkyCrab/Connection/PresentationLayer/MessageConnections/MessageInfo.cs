using SkyCrab.Connection.PresentationLayer.Messages;
using System;

namespace SkyCrab.Connection.PresentationLayer.MessageConnections
{
    public struct MessageInfo
    {
        public UInt16 id;
        public MessageId messageId;
        public object message;
    }
}
