using SkyCrab.Common_classes;
using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.AplicationLayer;
using SkyCrab.Connection.PresentationLayer.MessageConnections;
using SkyCrab.Connection.PresentationLayer.Messages;
using System.Threading.Tasks;

namespace SkyCrab
{
    class ClientConnection : AbstractClientConnection
    {
        public ClientConnection(string host, int port, int readTimeout) :
           base(host, port, readTimeout)
        {

        }

        protected override void ProcessMessages()
        {
            foreach (MessageInfo messageInfo in messages.GetConsumingEnumerable())
            {
                processingMessagesOk = true;

                switch(messageInfo.messageId)
                {
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

                    case MessageId.PLAYER_JOINED:
                        {
                           lock(SkyCrabGlobalVariables.roomLock)
                                SkyCrabGlobalVariables.room.AddPlayer((Player)messageInfo.message);
                            break;
                        }

                    default:
                        {
                            DisplayMessageBox("Otrzymano nieobsługiwany komunikat od serwera (" + messageInfo.messageId.ToString() + ")!");
                            throw new SkyCrabException("Unsuported message: " + messageInfo.messageId.ToString());
                        }
                }
            }
        }

        private void DisplayMessageBox(string message)
        {
            Task.Factory.StartNew(()=>System.Windows.MessageBox.Show(message));
        }

        protected override void DoDispose()
        {
            if (!disconectedOnItsOwn)
                DisplayMessageBox("Serwer zakończył pracę");
            base.DoDispose();
        }

    }
}
