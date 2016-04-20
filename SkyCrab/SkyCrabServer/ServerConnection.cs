using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.AplicationLayer;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Menu;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SkyCrabServer
{
    sealed class UnsuportedMessageException : SkyCrabServerException
    {
    }

    class ServerConnection : AbstractServerConnection
    {

        private Random random = new Random(); //TODO remove when will be not used


        public ServerConnection(TcpClient tcpClient, int readTimeout) :
            base(tcpClient, readTimeout)
        {
        }

        //TODO do something smart if exception occured
        protected override void ProcessMessages()
        {
            foreach (MessageInfo messageInfo in messages.GetConsumingEnumerable())
            {
                processingMessagesOk = true;
                switch (messageInfo.messageId)
                {
                    case MessageId.LOGIN:
                        Login((PlayerProfile)messageInfo.message);
                        break;

                    case MessageId.LOGOUT:
                        Logout();
                        break;

                    case MessageId.REGISTER:
                        Register((PlayerProfile)messageInfo.message);
                        break;

                    case MessageId.EDIT_PROFILE:
                        EditProfile((PlayerProfile)messageInfo.message);
                        break;

                    case MessageId.DISCONNECT:
                        Task.Factory.StartNew(CloseThreadBody);
                        break;

                    default:
                        throw new UnsuportedMessageException();
                }
            }
            string info = "Client disconnected. (" + ClientEndPoint.Port + ")\n";
            if (Listener.serverConsole == null)
                Console.WriteLine(info);
            else
                Listener.serverConsole.Write(info);
        }

        private void Login(PlayerProfile playerProfile)
        {
            //TODO undummy this method
            if (RandBool)
            {
                playerProfile.Password = null;
                if (RandBool)
                    playerProfile.Nick = "SPEJS0N";
                else
                    playerProfile.Nick = "._//Seba14\\\\_.";
                playerProfile.EMail = "lol.omg@naszeosiedle.pl";
                playerProfile.Registration = DateTime.Now.AddDays(-16);
                playerProfile.LastActivity = DateTime.Now;
                Player player = new Player((uint)random.Next(), playerProfile);
                LoginOkMsg.AsyncPostLoginOk(this, player);
            }
            else
            {
                ErrorMsg.AsyncPostError(this, RandErrorCode(ErrorCode.WRONG_LOGIN_OR_PASSWORD, ErrorCode.USER_ALREADY_LOGGED, ErrorCode.SESSION_ALREADY_LOGGED));
            }
        }

        private void Logout()
        {
            //TODO undummy this method
            if (RandBool)
                OkMsg.AsyncPostOk(this);
            else
                ErrorMsg.AsyncPostError(this, ErrorCode.NOT_LOGGED);
        }

        private void Register(PlayerProfile playerProfile)
        {
            //TODO undummy this method
            if (RandBool)
            {
                playerProfile.Password = null;
                playerProfile.Nick = playerProfile.Login;
                playerProfile.Registration = DateTime.Now;
                playerProfile.LastActivity = DateTime.Now;
                Player player = new Player((uint)random.Next(), playerProfile);
                LoginOkMsg.AsyncPostLoginOk(this, player);
            }
            else
            {
                ErrorMsg.AsyncPostError(this, RandErrorCode(ErrorCode.LOGIN_OCCUPIED, ErrorCode.PASSWORD_TOO_SHORT, ErrorCode.EMAIL_OCCUPIED));
            }
        }

        private void EditProfile(PlayerProfile playerProfile)
        {
            //TODO undummy this method
            if (RandBool)
                OkMsg.AsyncPostOk(this);
            else
                ErrorMsg.AsyncPostError(this, RandErrorCode(ErrorCode.NICK_IS_TOO_SHITTY, ErrorCode.PASSWORD_TOO_SHORT2, ErrorCode.EMAIL_OCCUPIED2));
        }

        private ErrorCode RandErrorCode(params ErrorCode[] errorCodes) //TODO remove when will be not used
        {
            int index = random.Next(errorCodes.Length);
            return errorCodes[index];
        }

        private bool RandBool //TODO remove when will be not used
        {
            get { return random.NextDouble() > 0.5; }
        }

        private void CloseThreadBody()
        {
            ConnectionManager.Close(this);
        }

    }
}
