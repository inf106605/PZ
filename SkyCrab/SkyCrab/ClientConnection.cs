﻿using SkyCrab.Connection.AplicationLayer;
using SkyCrab.Connection.PresentationLayer.MessageConnections;
using SkyCrab.Connection.PresentationLayer.Messages;
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
                    //case MessageId.DISCONNECT:
                        //{
                            //TODO Sorry, you will not longer receive this message. It is handle by another class now. If you want to know, that server is disconnecting, use callback or implement method "DoDispose".
                            //DisplayMessageBox("Serwer zakończył pracę!");
                            //AnswerDisconnect(messageInfo.message);
                            //break;
                        //}

                    case MessageId.PING:
                        {
                            AnswerPing(messageInfo.message);
                            break;
                        }
                    case MessageId.NO_PONG:
                        {
                            DisplayMessageBox("Serwer nie odpowiada!");
                            AsyncDispose();
                            break;
                        }
                }
            }
        }

        private void DisplayMessageBox(string message)
        {
            Task.Factory.StartNew(()=>System.Windows.MessageBox.Show(message));
        }

    }
}
