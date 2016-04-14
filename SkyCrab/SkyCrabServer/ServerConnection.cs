using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.AplicationLayer;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Menu;
using System;
using System.Net.Sockets;

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

                    default:
                        throw new UnsuportedMessageException();
                }
            }
            Console.WriteLine("Client disconnected.\n"); //TODO more info
        }

        private void Login(PlayerProfile playerProfile)
        {
            //TODO undummy this method
            if (RandBool)
            {
                playerProfile.password = null;
                if (RandBool)
                    playerProfile.nick = "SPEJS0N";
                else
                    playerProfile.nick = "._//Seba14\\\\_.";
                playerProfile.eMail = "lol.omg@naszeosiedle.pl";
                playerProfile.registration = DateTime.Now.AddDays(-16);
                playerProfile.lastActivity = DateTime.Now;
                Player player = new Player((uint)random.Next(), false, playerProfile.nick);
                LoginOk.PostLoginOk(this, player);
            } else
            {
                Error.PostError(this, (ErrorCode)random.Next(ushort.MaxValue)); //TODO just wrong
            }
        }

        private void Logout()
        {
            //TODO undummy this method
            if (RandBool)
                Ok.PostOk(this);
            else
                Error.PostError(this, (ErrorCode)random.Next(ushort.MaxValue)); //TODO just wrong
        }

        private void Register(PlayerProfile playerProfile)
        {
            //TODO undummy this method
            if (RandBool)
            {
                playerProfile.password = null;
                playerProfile.nick = playerProfile.login;
                playerProfile.registration = DateTime.Now;
                playerProfile.lastActivity = DateTime.Now;
                Player player = new Player((uint)random.Next(), false, playerProfile.nick);
                LoginOk.PostLoginOk(this, player);
            }
            else
            {
                Error.PostError(this, (ErrorCode)random.Next(ushort.MaxValue)); //TODO just wrong
            }
        }

        private void EditProfile(PlayerProfile playerProfile)
        {
            //TODO undummy this method
            if (RandBool)
                Ok.PostOk(this);
            else
                Error.PostError(this, (ErrorCode)random.Next(ushort.MaxValue)); //TODO just wrong
        }

        private bool RandBool //TODO remove when will be not used
        {
            get { return random.NextDouble() > 0.5; }
        }

    }
}
