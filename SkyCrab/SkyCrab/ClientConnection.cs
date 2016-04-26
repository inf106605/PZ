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
                            DisplayMessageBox("Serwer zakończył pracę!");
                            Disconnect();
                            break;
                        }

                    case MessageId.PING:
                        {
                            AnswerPing(messageInfo.message);
                            break;
                        }
                    case MessageId.NO_PONG:
                        {
                            DisplayMessageBox("Serwer nie odpowiada!");
                            DisconnectMsg.AsyncPostDisconnect(this);
                            Disconnect();
                            break;
                        }
                }
            }
        }

        private void DisconnectTask()
        {
            App.clientConn = null;
            Dispose();
        }

        private void DisplayMessageBox(string message)
        {
            Task.Factory.StartNew(()=>System.Windows.MessageBox.Show(message));
        }

        private void Disconnect()
        {
            Task.Factory.StartNew(DisconnectTask);
        }

    }
}
