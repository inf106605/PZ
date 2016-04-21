using SkyCrab.Connection.AplicationLayer;
using SkyCrab.Connection.PresentationLayer.Messages;
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

                switch(messageInfo.messageId)
                {
                    case MessageId.DISCONNECT:
                        {
                            System.Windows.MessageBox.Show("Serwer zakończył pracę!");
                            break;
                        }
                }
            }
        }
    }
}
