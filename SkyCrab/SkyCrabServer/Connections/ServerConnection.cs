using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.AplicationLayer;
using SkyCrab.Connection.PresentationLayer.MessageConnections;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Common.Errors;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Accounts;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Friends;
using SkyCrabServer.Databases;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SkyCrabServer.Connactions
{
    sealed class UnsuportedMessageException : SkyCrabServerException
    {
    }

    class ServerConnection : AbstractServerConnection
    {

        private Random random = new Random(); //TODO remove when will be not used
        private Player player;


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
                    //common

                    case MessageId.DISCONNECT:
                        AnswerDisconnect(messageInfo.message);
                        break;

                    case MessageId.PING:
                        AnswerPing(messageInfo.message);
                        break;

                    case MessageId.NO_PONG:
                        Globals.serverConsole.WriteLine("No answer to PING! (" + ClientAuthority + ")");
                        AsyncDispose();
                        break;

                    //menu

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

                    case MessageId.GET_FRIENDS:
                        GetFriends();
                        break;

                    case MessageId.FIND_PLAYERS:
                        FindPlayers((string) messageInfo.message);
                        break;

                    case MessageId.ADD_FRIEND:
                        AddFriend((UInt32) messageInfo.message);
                        break;

                    case MessageId.REMOVE_FRIEND:
                        RemoveFriend((UInt32)messageInfo.message);
                        break;
                    
                    //unknown
                    
                    default:
                        throw new UnsuportedMessageException();
                }
            }
            string info = "Client disconnected. (" + ClientAuthority + ")";
            Globals.serverConsole.WriteLine(info);
        }

        private void Login(PlayerProfile playerProfile)
        {
            Player player = PlayerProfileTable.GetByLogin(playerProfile.Login);
            if (player == null)
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.WRONG_LOGIN_OR_PASSWORD);
                return;
            }
            {
                string passwordHash = PlayerProfileTable.GetPasswordHash(player.Id);
                string decoratedPassword = DecoratePassword(playerProfile);
                playerProfile.Password = null;
                if (!BCrypt.CheckPassword(decoratedPassword, passwordHash))
                {
                    ErrorMsg.AsyncPostError(this, ErrorCode.WRONG_LOGIN_OR_PASSWORD);
                    return;
                }
            }
            if (this.player != null)
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.SESSION_ALREADY_LOGGED);
                return;
            }
            if (!Globals.players.TryAdd(player.Id, player))
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.USER_ALREADY_LOGGED);
                return;
            }

            this.player = player;
            LoginOkMsg.AsyncPostLoginOk(this, player);
        }

        private void Logout() //TODO use this during disposing
        {
            if (player == null)
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.NOT_LOGGED);
                return;

            }

            OnLogout();
            OkMsg.AsyncPostOk(this);
        }

        private void OnLogout()
        {
            {
                Player temp;
                Globals.players.TryRemove(player.Id, out temp);
            }
            //TODO leave current room
            player = null;
        }

        private void Register(PlayerProfile playerProfile)
        {
            lock (PlayerProfileTable._lock)
            {
                if (PlayerProfileTable.CheckLoginExists(playerProfile.Login))
                {
                    ErrorMsg.AsyncPostError(this, ErrorCode.LOGIN_OCCUPIED);
                    return;
                }
                if (PlayerProfileTable.CheckEMailExists(playerProfile.EMail, 0))
                {
                    ErrorMsg.AsyncPostError(this, ErrorCode.EMAIL_OCCUPIED);
                    return;
                }

                UInt32 id;
                {
                    string decoratedPassword = DecoratePassword(playerProfile);
                    string passwordHash = BCrypt.HashPassword(decoratedPassword, BCrypt.GenerateSalt(12));
                    playerProfile.Password = null;
                    playerProfile.Nick = playerProfile.Login;
                    playerProfile.Registration = DateTime.Now;
                    playerProfile.LastActivity = DateTime.Now;
                    id = PlayerProfileTable.Create(playerProfile, passwordHash);
                }

                Player player = new Player(id, playerProfile);
                Globals.players.TryAdd(player.Id, player);
                LoginOkMsg.AsyncPostLoginOk(this, player);
            }
        }

        private void EditProfile(PlayerProfile playerProfile)
        {
            if (player == null)
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.NOT_LOGGED2);
                return;
            }
            string passwordHash;
            if (playerProfile.Password == null)
            {
                passwordHash = PlayerProfileTable.GetPasswordHash(player.Id);
            }
            else
            {
                string decoratedPassword = DecoratePassword(playerProfile);
                passwordHash = BCrypt.HashPassword(decoratedPassword, BCrypt.GenerateSalt(12));
                playerProfile.Password = null;
            }
            if (playerProfile.Nick == null)
            {
                playerProfile.Nick = player.Nick;
            }
            else
            {
                if (IsNickShitty(playerProfile.Nick))
                {
                    ErrorMsg.AsyncPostError(this, ErrorCode.NICK_IS_TOO_SHITTY);
                    return;
                }
            }
            if (playerProfile.EMail == null)
            {
                playerProfile.EMail = player.Profile.EMail;
            }
            else
            {
                if (PlayerProfileTable.CheckEMailExists(playerProfile.EMail, player.Id))
                {
                    ErrorMsg.AsyncPostError(this, ErrorCode.EMAIL_OCCUPIED2);
                    return;
                }
            }

            PlayerProfileTable.Modify(player.Id, playerProfile, passwordHash);

            player.Profile.Nick = playerProfile.Nick;
            player.Profile.EMail = playerProfile.EMail;
            //TODO should I do something more to inform program about changes? maybe.
            OkMsg.AsyncPostOk(this);
        }

        private static bool IsNickShitty(string nick)
        {
            string[] reservedNicks = { "siupa" };
            foreach (string reservedNick in reservedNicks)
                if (reservedNick.ToUpper() == nick.ToUpper())
                    return true;
            //TODO censorship and so on
            return false;
        }

        private void GetFriends()
        {
            //TODO undummy this method
            if (RandBool)
            {
                List<Player> players = new List<Player>();
                players.Add(new Player((uint)random.Next(), false, RandBool ? "Korwin Krul" : "Może Bałtydzkie"));
                players.Add(new Player((uint)random.Next(), false, RandBool ? "LOLCAT ;-)" : "20000000 koni mechanicznych"));
                players.Add(new Player((uint)random.Next(), false, RandBool ? "LOLCAT ;-)" : "Korwin Krul"));
                PlayerListMsg.AsyncPostPlayerList(this, players);
            }
            else
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.NOT_LOGGED3);
            }
        }

        private void FindPlayers(string searchPhrase)
        {
            //TODO undummy this method
            List<Player> players = new List<Player>();
            players.Add(new Player((uint)random.Next(), false, RandBool ? "Ania26" : "Skrablenator 5000"));
            players.Add(new Player((uint)random.Next(), false, RandBool ? "Liter" : "Roman"));
            players.Add(new Player((uint)random.Next(), false, RandBool ? "Roman" : "Skrablenator 5000"));
            PlayerListMsg.AsyncPostPlayerList(this, players);
        }

        private void AddFriend(UInt32 idFriend)
        {
            //TODO undummy this method
            if (RandBool)
                OkMsg.AsyncPostOk(this);
            else
                ErrorMsg.AsyncPostError(this, RandErrorCode(ErrorCode.NOT_LOGGED4, ErrorCode.FRIEND_ALREADY_ADDED, ErrorCode.FOREVER_ALONE, ErrorCode.NO_SUCH_PLAYER));
        }

        private void RemoveFriend(UInt32 idFriend)
        {
            //TODO undummy this method
            if (RandBool)
                OkMsg.AsyncPostOk(this);
            else
                ErrorMsg.AsyncPostError(this, RandErrorCode(ErrorCode.NOT_LOGGED5, ErrorCode.NO_SUCH_FRIEND));
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

        private static String DecoratePassword(PlayerProfile profile)
        {
            const string SALT = "L4RUmhBUM9sIbwg7hPEJMBSPo\\vFxfeJH*c?pt@&Rk0L)0EC obw1s71(!xn<6f$WFdc6,[@lPh%PTYv6iv}DU13 mwYY.hh8tL9h";
            string decoratedPassword = profile.Password + SALT + profile.Login;
            return decoratedPassword;
        }

        protected override void DoDispose()
        {
            OnLogout();
            base.DoDispose();
        }

    }
}
