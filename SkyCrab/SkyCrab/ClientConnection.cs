using SkyCrab.Connection.AplicationLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCrab
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
