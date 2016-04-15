using SkyCrab.Connection.AplicationLayer;

namespace DummyClient
{
    class ClientConnection : AbstractClientConnection
    {

        public ClientConnection(string host, int readTimeout) :
            base(host, readTimeout)
        {
        }

        protected override void ProcessMessages()
        {
            foreach (MessageInfo messageInfo in messages.GetConsumingEnumerable())
            {
                processingMessagesOk = true;
            }
        }

    }
}
