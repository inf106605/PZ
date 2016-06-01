using SkyCrab.Common_classes.Players;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Common.Errors;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Accounts;
using SkyCrabServer.Connactions;
using SkyCrabServer.Databases;
using System;

namespace SkyCrabServer.ServerLogics
{
    sealed class ServerPlayer
    {

        private const string DEFAULT_NICK = "Crab";

        public readonly ServerConnection connection;
        public Player player;
        public readonly ServerFriend serverFriend;
        public readonly ServerRoom serverRoom;
        public readonly ServerGameResult serverGameResult;
        public readonly ServerGame serverGame;


        public bool LoggedAsGuest
        {
            get
            {
                if (player == null)
                    return false;
                else
                    return player.Profile == null;
            }
        }

        public bool LoggedAnyway
        {
            get
            {
                return player != null;
            }
        }

        public bool LoggedNormally
        {
            get
            {
                if (player == null)
                    return false;
                else
                    return player.Profile != null;
            }
        }


        public ServerPlayer(ServerConnection connection)
        {
            this.connection = connection;
            this.serverFriend = new ServerFriend(this);
            this.serverRoom = new ServerRoom(this);
            this.serverGameResult = new ServerGameResult(this);
            this.serverGame = new ServerGame(this, serverRoom);
        }

        public void LoginAsGuest(Int16 id)
        {
            if (LoggedNormally)
            {
                ErrorMsg.AsyncPost(id, connection, ErrorCode.SESSION_ALREADY_LOGGED);
                return;
            }
            if (!LoggedAsGuest)
            {
                Globals.dataLock.AcquireWriterLock(-1);
                try
                {
                    UInt32 playerId = PlayerTable.Create();
                    player = new Player(playerId, false, DEFAULT_NICK + '#' + playerId);
                    Globals.players.TryAdd(playerId, this);
                }
                finally
                {
                    Globals.dataLock.ReleaseWriterLock();
                }
            }
            LoginOkMsg.AsyncPost(id, connection, player);
        }

        public void Login(Int16 id, PlayerProfile playerProfile)
        {
            if (LoggedNormally)
            {
                ErrorMsg.AsyncPost(id, connection, ErrorCode.SESSION_ALREADY_LOGGED2);
                return;
            }
            if (LoggedAsGuest)
                OnLogout();
            Globals.dataLock.AcquireWriterLock(-1);
            try
            {
                Player newPlayer = PlayerProfileTable.GetLongByLogin(playerProfile.Login);
                if (newPlayer == null)
                {
                    ErrorMsg.AsyncPost(id, connection, ErrorCode.WRONG_LOGIN_OR_PASSWORD);
                    return;
                }
                {
                    string passwordHash = PlayerProfileTable.GetPasswordHash(newPlayer.Id);
                    string decoratedPassword = DecoratePassword(playerProfile);
                    playerProfile.Password = null;
                    if (!BCrypt.CheckPassword(decoratedPassword, passwordHash))
                    {
                        ErrorMsg.AsyncPost(id, connection, ErrorCode.WRONG_LOGIN_OR_PASSWORD);
                        return;
                    }
                }
                if (!Globals.players.TryAdd(newPlayer.Id, this))
                {
                    ErrorMsg.AsyncPost(id, connection, ErrorCode.USER_ALREADY_LOGGED);
                    return;
                }
                player = newPlayer;
                LoginOkMsg.AsyncPost(id, connection, player);
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
        }

        public void Logout(Int16 id)
        {
            if (!LoggedAnyway)
            {
                ErrorMsg.AsyncPost(id, connection, ErrorCode.NOT_LOGGED);
                return;
            }
            OnLogout();
            OkMsg.AsyncPost(id, connection);
        }

        public void Register(Int16 id, PlayerProfile playerProfile)
        {
            lock (PlayerProfileTable._lock)
            {
                if (LoggedNormally)
                {
                    ErrorMsg.AsyncPost(id, connection, ErrorCode.SESSION_ALREADY_LOGGED3);
                    return;
                }
                if (LoggedAsGuest)
                    OnLogout();
                if (PlayerProfileTable.LoginExists(playerProfile.Login))
                {
                    ErrorMsg.AsyncPost(id, connection, ErrorCode.LOGIN_OCCUPIED);
                    return;
                }
                if (PlayerProfileTable.EMailExists(playerProfile.EMail, 0))
                {
                    ErrorMsg.AsyncPost(id, connection, ErrorCode.EMAIL_OCCUPIED);
                    return;
                }

                UInt32 playerId;
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
                    playerId = PlayerProfileTable.Create(playerProfile, passwordHash);
                }
                Globals.dataLock.AcquireWriterLock(-1);
                try
                {
                    player = new Player(playerId, playerProfile);
                    Globals.players.TryAdd(player.Id, this);
                }
                finally
                {
                    Globals.dataLock.ReleaseWriterLock();
                }
                LoginOkMsg.AsyncPost(id, connection, player);
            }
        }

        public void EditProfile(Int16 id, PlayerProfile playerProfile)
        {
            if (!LoggedNormally)
            {
                ErrorMsg.AsyncPost(id, connection, ErrorCode.NOT_LOGGED2);
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
                    ErrorMsg.AsyncPost(id, connection, ErrorCode.NICK_IS_TOO_SHITTY);
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
                    ErrorMsg.AsyncPost(id, connection, ErrorCode.EMAIL_OCCUPIED2);
                    return;
                }
            }

            PlayerProfileTable.Modify(player.Id, playerProfile, passwordHash);

            Globals.dataLock.AcquireWriterLock(-1);
            try
            {
                player.Profile.Nick = playerProfile.Nick;
                player.Profile.EMail = playerProfile.EMail;
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
            OkMsg.AsyncPost(id, connection);
        }

        private static bool IsNickShitty(string nick)
        {
            string[] reservedNicks = { "siupa", "kris", "jerzyna" };
            foreach (string reservedNick in reservedNicks)
                if (reservedNick.ToUpper() == nick.ToUpper())
                    return true;
            //TODO censorship and so on
            return false;
        }

        private static String DecoratePassword(PlayerProfile profile)
        {
            const string SALT = "L4RUmhBUM9sIbwg7hPEJMBSPo\\vFxfeJH*c?pt@&Rk0L)0EC obw1s71(!xn<6f$WFdc6,[@lPh%PTYv6iv}DU13 mwYY.hh8tL9h";
            string decoratedPassword = profile.Password + SALT + profile.Login;
            return decoratedPassword;
        }

        public void OnLogout()
        {
            if (!LoggedAnyway)
                return;
            if (LoggedNormally)
                PlayerProfileTable.UpdateLastActivity(player.Id);
            serverRoom.OnLeaveRoom();
            Globals.dataLock.AcquireWriterLock(-1);
            try
            {
                {
                    ServerPlayer temp;
                    Globals.players.TryRemove(player.Id, out temp);
                }
                player = null;
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
        }

    }
}
