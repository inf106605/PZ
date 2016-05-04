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

        private const string DEFAULT_NICK = "Crab";
        
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
            if (this.player != null)
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.SESSION_ALREADY_LOGGED);
                return;
            }
            Player player = PlayerProfileTable.GetLongByLogin(playerProfile.Login);
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
            if (!Globals.players.TryAdd(player.Id, player))
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.USER_ALREADY_LOGGED);
                return;
            }

            this.player = player;
            LoginOkMsg.AsyncPostLoginOk(this, player);
        }

        private void Logout()
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
            if (player != null)
            {
                {
                    Player temp;
                    Globals.players.TryRemove(player.Id, out temp);
                }
                //TODO leave current room
                player = null;
            }
        }

        private void Register(PlayerProfile playerProfile)
        {
            lock (PlayerProfileTable._lock)
            {
                if (this.player != null)
                {
                    ErrorMsg.AsyncPostError(this, ErrorCode.SESSION_ALREADY_LOGGED2);
                    return;
                }
                if (PlayerProfileTable.LoginExists(playerProfile.Login))
                {
                    ErrorMsg.AsyncPostError(this, ErrorCode.LOGIN_OCCUPIED);
                    return;
                }
                if (PlayerProfileTable.EMailExists(playerProfile.EMail, 0))
                {
                    ErrorMsg.AsyncPostError(this, ErrorCode.EMAIL_OCCUPIED);
                    return;
                }

                UInt32 id;
                {
                    string decoratedPassword = DecoratePassword(playerProfile);
                    string passwordHash = BCrypt.HashPassword(decoratedPassword, BCrypt.GenerateSalt(12));
                    playerProfile.Password = null;
                    if (IsNickShitty(playerProfile.Login))
                        playerProfile.Nick = DEFAULT_NICK;
                    else
                        playerProfile.Nick = playerProfile.Login;
                    playerProfile.Registration = DateTime.Now;
                    playerProfile.LastActivity = DateTime.Now;
                    id = PlayerProfileTable.Create(playerProfile, passwordHash);
                }

                player = new Player(id, playerProfile);
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
                if (PlayerProfileTable.EMailExists(playerProfile.EMail, player.Id))
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
            string[] reservedNicks = { "siupa" , "kris", "jerzyna" };
            foreach (string reservedNick in reservedNicks)
                if (reservedNick.ToUpper() == nick.ToUpper())
                    return true;
            //TODO censorship and so on
            return false;
        }

        private void GetFriends()
        {
            if (player == null)
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.NOT_LOGGED3);
                return;
            }
            List<UInt32> friendIds = FriendTable.GetByPlayerId(player.Id);
            List<Player> friends = new List<Player>();
            foreach (UInt32 friendId in friendIds)
                friends.Add(PlayerProfileTable.GetShortById(friendId));
            PlayerListMsg.AsyncPostPlayerList(this, friends);
        }

        private void FindPlayers(string searchPhraze)
        {
            List<Player> players = PlayerProfileTable.FindShort(searchPhraze);
            PlayerListMsg.AsyncPostPlayerList(this, players);
        }

        private void AddFriend(UInt32 friendId)
        {
            if (player == null)
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.NOT_LOGGED4);
                return;
            }
            if (player.Id == friendId)
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.FOREVER_ALONE);
                return;
            }
            if (!PlayerProfileTable.IdExists(friendId))
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.NO_SUCH_PLAYER);
                return;
            }
            if (FriendTable.Exists(player.Id, friendId))
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.FRIEND_ALREADY_ADDED);
                return;
            }

            FriendTable.Create(player.Id, friendId);
            OkMsg.AsyncPostOk(this);
        }

        private void RemoveFriend(UInt32 friendId)
        {
            if (player == null)
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.NOT_LOGGED5);
                return;
            }
            if (!FriendTable.Exists(player.Id, friendId))
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.NO_SUCH_FRIEND);
                return;
            }

            FriendTable.Delete(player.Id, friendId);
            OkMsg.AsyncPostOk(this);
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
