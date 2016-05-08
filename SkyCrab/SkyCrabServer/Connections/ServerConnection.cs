using SkyCrab.Common_classes.Chats;
using SkyCrab.Common_classes.Players;
using SkyCrab.Common_classes.Rooms;
using SkyCrab.Common_classes.Rooms.Players;
using SkyCrab.Connection.AplicationLayer;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Connection.PresentationLayer.MessageConnections;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Common.Errors;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Accounts;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Friends;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.InRooms;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Rooms;
using SkyCrabServer.Databases;
using SkyCrabServer.ServerClasses;
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
        
        private ServerPlayer serverPlayer;


        private bool Logged
        {
            get
            {
                if (this.serverPlayer == null)
                    return false;
                else
                    return this.serverPlayer.player.Profile != null;
            }
        }

        private bool InRoom
        {
            get
            {
                if (serverPlayer == null)
                    return false;
                if (serverPlayer.room == null)
                    return false;
                return true;
            }
        }

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

                Int16 id = messageInfo.id;
                switch (messageInfo.messageId)
                {
                    //COMMON

                    case MessageId.PING:
                        AnswerPing(id, messageInfo.message);
                        break;

                    case MessageId.NO_PONG:
                        Globals.serverConsole.WriteLine("No answer to PING! (" + ClientAuthority + ")");
                        AsyncDispose();
                        break;

                    //MENU

                    //accounts

                    case MessageId.LOGIN:
                        Login(id, (PlayerProfile)messageInfo.message);
                        break;

                    case MessageId.LOGOUT:
                        Logout(id);
                        break;

                    case MessageId.REGISTER:
                        Register(id, (PlayerProfile)messageInfo.message);
                        break;

                    case MessageId.EDIT_PROFILE:
                        EditProfile(id, (PlayerProfile)messageInfo.message);
                        break;

                    //friends

                    case MessageId.GET_FRIENDS:
                        GetFriends(id);
                        break;

                    case MessageId.FIND_PLAYERS:
                        FindPlayers(id, (string) messageInfo.message);
                        break;

                    case MessageId.ADD_FRIEND:
                        AddFriend(id, (UInt32) messageInfo.message);
                        break;

                    case MessageId.REMOVE_FRIEND:
                        RemoveFriend(id, (UInt32)messageInfo.message);
                        break;

                    //room

                    case MessageId.CREATE_ROOM:
                        CreateRoom(id, (Room)messageInfo.message);
                        break;

                    case MessageId.FIND_ROOMS:
                        FindRooms(id, (Room)messageInfo.message);
                        break;

                    case MessageId.GET_FRIEND_ROOMS:
                        GetFriendRooms(id);
                        break;

                    default:
                        throw new UnsuportedMessageException();

                    //in rooms

                    case MessageId.JOIN_ROOM:
                        JoinRoom(id, (UInt32)messageInfo.message);
                        break;

                    case MessageId.LEAVE_ROOM:
                        LeaveRoom(id);
                        break;

                    case MessageId.PLAYER_READY:
                        PlayerReady();
                        break;

                    case MessageId.PLAYER_NOT_READY:
                        PlayerNotReady();
                        break;

                    case MessageId.CHAT:
                        Chat((ChatMessage)messageInfo.message);
                        break;
                }
            }
        }

        private void Login(Int16 id, PlayerProfile playerProfile)
        {
            if (Logged)
            {
                ErrorMsg.AsyncPostError(id, this, ErrorCode.SESSION_ALREADY_LOGGED);
                return;
            }
            Player player;
            Globals.dataLock.AcquireWriterLock(-1);
            try
            {
                player = PlayerProfileTable.GetLongByLogin(playerProfile.Login);
                if (player == null)
                {
                    ErrorMsg.AsyncPostError(id, this, ErrorCode.WRONG_LOGIN_OR_PASSWORD);
                    return;
                }
                {
                    string passwordHash = PlayerProfileTable.GetPasswordHash(player.Id);
                    string decoratedPassword = DecoratePassword(playerProfile);
                    playerProfile.Password = null;
                    if (!BCrypt.CheckPassword(decoratedPassword, passwordHash))
                    {
                        ErrorMsg.AsyncPostError(id, this, ErrorCode.WRONG_LOGIN_OR_PASSWORD);
                        return;
                    }
                }
                ServerPlayer serverPlayer = new ServerPlayer(this, player);
                if (!Globals.players.TryAdd(player.Id, serverPlayer))
                {
                    ErrorMsg.AsyncPostError(id, this, ErrorCode.USER_ALREADY_LOGGED);
                    return;
                }
                this.serverPlayer = serverPlayer;
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
            LoginOkMsg.AsyncPostLoginOk(id, this, player);
        }

        private void Logout(Int16 id)
        {
            if (!Logged)
            {
                ErrorMsg.AsyncPostError(id, this, ErrorCode.NOT_LOGGED);
                return;
            }
            OnLogout();
            OkMsg.AsyncPostOk(id, this);
        }

        private void OnLogout()
        {
            if (Logged)
            {
                OnLeaveRoom();
                Globals.dataLock.AcquireWriterLock(-1);
                try
                {
                    ServerPlayer temp;
                    Globals.players.TryRemove(serverPlayer.player.Id, out temp);
                    serverPlayer = null;
                }
                finally
                {
                    Globals.dataLock.ReleaseWriterLock();
                }
            }
        }

        private void Register(Int16 id, PlayerProfile playerProfile)
        {
            lock (PlayerProfileTable._lock)
            {
                if (this.serverPlayer != null)
                {
                    ErrorMsg.AsyncPostError(id, this, ErrorCode.SESSION_ALREADY_LOGGED2);
                    return;
                }
                if (PlayerProfileTable.LoginExists(playerProfile.Login))
                {
                    ErrorMsg.AsyncPostError(id, this, ErrorCode.LOGIN_OCCUPIED);
                    return;
                }
                if (PlayerProfileTable.EMailExists(playerProfile.EMail, 0))
                {
                    ErrorMsg.AsyncPostError(id, this, ErrorCode.EMAIL_OCCUPIED);
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
                    serverPlayer = new ServerPlayer(this, new Player(playerId, playerProfile));
                    Globals.players.TryAdd(serverPlayer.player.Id, serverPlayer);
                }
                finally
                {
                    Globals.dataLock.ReleaseWriterLock();
                }
                LoginOkMsg.AsyncPostLoginOk(id, this, serverPlayer.player);
            }
        }

        private void EditProfile(Int16 id, PlayerProfile playerProfile)
        {
            if (!Logged)
            {
                ErrorMsg.AsyncPostError(id, this, ErrorCode.NOT_LOGGED2);
                return;
            }
            string passwordHash;
            if (playerProfile.Password == null)
            {
                passwordHash = PlayerProfileTable.GetPasswordHash(serverPlayer.player.Id);
            }
            else
            {
                string decoratedPassword = DecoratePassword(playerProfile);
                passwordHash = BCrypt.HashPassword(decoratedPassword, BCrypt.GenerateSalt(12));
                playerProfile.Password = null;
            }
            if (playerProfile.Nick == null)
            {
                playerProfile.Nick = serverPlayer.player.Nick;
            }
            else
            {
                if (IsNickShitty(playerProfile.Nick))
                {
                    ErrorMsg.AsyncPostError(id, this, ErrorCode.NICK_IS_TOO_SHITTY);
                    return;
                }
            }
            if (playerProfile.EMail == null)
            {
                playerProfile.EMail = serverPlayer.player.Profile.EMail;
            }
            else
            {
                if (PlayerProfileTable.EMailExists(playerProfile.EMail, serverPlayer.player.Id))
                {
                    ErrorMsg.AsyncPostError(id, this, ErrorCode.EMAIL_OCCUPIED2);
                    return;
                }
            }

            PlayerProfileTable.Modify(serverPlayer.player.Id, playerProfile, passwordHash);

            Globals.dataLock.AcquireWriterLock(-1);
            try
            {
                serverPlayer.player.Profile.Nick = playerProfile.Nick;
                serverPlayer.player.Profile.EMail = playerProfile.EMail;
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
            OkMsg.AsyncPostOk(id, this);
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

        private static String DecoratePassword(PlayerProfile profile)
        {
            const string SALT = "L4RUmhBUM9sIbwg7hPEJMBSPo\\vFxfeJH*c?pt@&Rk0L)0EC obw1s71(!xn<6f$WFdc6,[@lPh%PTYv6iv}DU13 mwYY.hh8tL9h";
            string decoratedPassword = profile.Password + SALT + profile.Login;
            return decoratedPassword;
        }

        private void GetFriends(Int16 id)
        {
            if (!Logged)
            {
                ErrorMsg.AsyncPostError(id, this, ErrorCode.NOT_LOGGED3);
                return;
            }
            List<UInt32> friendIds = FriendTable.GetByPlayerId(serverPlayer.player.Id);
            List<Player> friends = new List<Player>();
            Globals.dataLock.AcquireReaderLock(-1);
            try
            {
                foreach (UInt32 friendId in friendIds) //TODO Do it using database
                    friends.Add(PlayerProfileTable.GetShortById(friendId));
            }
            finally
            {
                Globals.dataLock.ReleaseReaderLock();
            }
            PlayerListMsg.AsyncPostPlayerList(id, this, friends);
        }

        private void FindPlayers(Int16 id, string searchPhraze)
        {
            List<Player> players;
            Globals.dataLock.AcquireReaderLock(-1);
            try
            {
                players = PlayerProfileTable.FindShort(searchPhraze);
            }
            finally
            {
                Globals.dataLock.ReleaseReaderLock();
            }
            PlayerListMsg.AsyncPostPlayerList(id, this, players);
        }

        private void AddFriend(Int16 id, UInt32 friendId)
        {
            if (!Logged)
            {
                ErrorMsg.AsyncPostError(id, this, ErrorCode.NOT_LOGGED4);
                return;
            }
            if (serverPlayer.player.Id == friendId)
            {
                ErrorMsg.AsyncPostError(id, this, ErrorCode.FOREVER_ALONE);
                return;
            }
            if (!PlayerProfileTable.IdExists(friendId))
            {
                ErrorMsg.AsyncPostError(id, this, ErrorCode.NO_SUCH_PLAYER);
                return;
            }
            if (FriendTable.Exists(serverPlayer.player.Id, friendId))
            {
                ErrorMsg.AsyncPostError(id, this, ErrorCode.FRIEND_ALREADY_ADDED);
                return;
            }

            FriendTable.Create(serverPlayer.player.Id, friendId);
            OkMsg.AsyncPostOk(id, this);
        }

        private void RemoveFriend(Int16 id, UInt32 friendId)
        {
            if (!Logged)
            {
                ErrorMsg.AsyncPostError(id, this, ErrorCode.NOT_LOGGED5);
                return;
            }
            if (!FriendTable.Exists(serverPlayer.player.Id, friendId))
            {
                ErrorMsg.AsyncPostError(id, this, ErrorCode.NO_SUCH_FRIEND);
                return;
            }

            FriendTable.Delete(serverPlayer.player.Id, friendId);
            OkMsg.AsyncPostOk(id, this);
        }

        private void CreateRoom(Int16 id, Room room)
        {
            MakeValidPlayer();
            if (InRoom)
            {
                ErrorMsg.AsyncPostError(id, this, ErrorCode.ALREADY_IN_ROOM);
                return;
            }
            bool rulesAreValid = room.Rules.maxRoundTime.value <= 3600 &&
                    room.Rules.maxPlayerCount.value <= 4;
            if (!rulesAreValid)
            {
                ErrorMsg.AsyncPostError(id, this, ErrorCode.INVALID_RULES);
                return;
            }
            room.Id = Globals.roomIdSequence.Value;
            room.AddPlayer(serverPlayer.player);
            room.OwnerId = serverPlayer.player.Id;
            Globals.dataLock.AcquireWriterLock(-1);
            try
            {
                Globals.rooms.TryAdd(room.Id, room);
                serverPlayer.room = room;
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
            RoomMsg.AsyncPostRoom(id, this, room);
            PlayerJoinedMsg.asycnPostJoined(this, serverPlayer.player);
            NewRoomOwnerMsg.AsyncPostNewOwner(this, serverPlayer.player.Id);
        }

        private void OnLeaveRoom()
        {
            Globals.dataLock.AcquireWriterLock(-1);
            try
            {
                if (InRoom)
                {
                    serverPlayer.room.RemovePlayer(serverPlayer.player.Id);
                    if (serverPlayer.room.Players.Count == 0)
                    {
                        Room tmp;
                        Globals.rooms.TryRemove(serverPlayer.room.Id, out tmp);
                    }
                    else
                    {
                        if (serverPlayer.room.OwnerId == 0)
                        {
                            serverPlayer.room.OwnerId = serverPlayer.room.Players.First.Value.Player.Id;
                            foreach (PlayerInRoom playerInRoom in serverPlayer.room.Players)
                            {
                                ServerPlayer otherServerPlayer; //Schrödinger Variable
                                Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                                if (otherServerPlayer == null)  //WTF!?
                                    throw new Exception("Whatever.");
                                NewRoomOwnerMsg.AsyncPostNewOwner(otherServerPlayer.connection, serverPlayer.room.OwnerId);
                            }
                        }
                        foreach (PlayerInRoom playerInRoom in serverPlayer.room.Players)
                        {
                            ServerPlayer otherServerPlayer; //Schrödinger Variable
                            Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                            if (otherServerPlayer == null)  //WTF!?
                                throw new Exception("Whatever.");
                            PlayerLeavedMsg.AsyncPostLeave(otherServerPlayer.connection, serverPlayer.player.Id);
                        }
                    }
                    serverPlayer.room = null;
                }
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
        }

        private void FindRooms(Int16 id, Room roomFilter)
        {
            List<Room> foundRooms = new List<Room>();
            Globals.dataLock.AcquireReaderLock(-1);
            try
            {
                foreach (KeyValuePair<UInt32, Room> pair in Globals.rooms)
                {
                    if (RoomMath(pair.Value, roomFilter))
                    {
                        foundRooms.Add(pair.Value);
                        if (foundRooms.Count == ListTranscoder<object>.MAX_COUNT)
                            break;
                    }
                }
            }
            finally
            {
                Globals.dataLock.ReleaseReaderLock();
            }
            RoomListMsg.AsyncPostRoomList(id, this, foundRooms);
        }

        private void GetFriendRooms(Int16 id)
        {
            if (!Logged)
            {
                ErrorMsg.AsyncPostError(id, this, ErrorCode.NOT_LOGGED6);
                return;
            }
            List<UInt32> friendIds = FriendTable.GetByPlayerId(serverPlayer.player.Id);
            List<Room> foundRooms = new List<Room>();
            Globals.dataLock.AcquireReaderLock(-1);
            try
            {
                foreach (UInt32 friendId in friendIds)
                {
                    ServerPlayer serverFriend;
                    if (Globals.players.TryGetValue(friendId, out serverFriend))
                    {
                        if (serverFriend.room != null)
                        {
                            if (RoomTypeMath(serverFriend.room))
                            {
                                foundRooms.Add(serverFriend.room);
                                if (foundRooms.Count == ListTranscoder<object>.MAX_COUNT)
                                    break;
                            }
                        }
                    }
                }
            }
            finally
            {
                Globals.dataLock.ReleaseReaderLock();
            }
            RoomListMsg.AsyncPostRoomList(id, this, foundRooms);
        }

        private bool RoomMath(Room room, Room roomFilter)
        {
            if (!room.Name.Contains(roomFilter.Name))
                return false;
            if (!room.Rules.Math(roomFilter.Rules))
                return false;
            if (!RoomTypeMath(room))
                return false;
            return true;
        }

        private bool RoomTypeMath(Room room)
        {
            if (room.RoomType == RoomType.PRIVATE)
                return false;
            if (room.RoomType == RoomType.FRIENDS && FriendTable.Exists(room.OwnerId, serverPlayer.player.Id))
                return false;
            return true;
        }

        private void JoinRoom(Int16 id, UInt32 roomId)
        {
            MakeValidPlayer();
            if (InRoom)
            {
                ErrorMsg.AsyncPostError(id, this, ErrorCode.ALREADY_IN_ROOM2);
                return;
            }
            Room room;
            Globals.dataLock.AcquireWriterLock(-1);
            try
            {
                if (!Globals.rooms.TryGetValue(roomId, out room))
                {
                    ErrorMsg.AsyncPostError(id, this, ErrorCode.NO_SUCH_ROOM);
                    return;
                }
                if (room.Players.Count >= room.Rules.maxPlayerCount.value)
                {
                    ErrorMsg.AsyncPostError(id, this, ErrorCode.ROOM_IS_FULL);
                    return;
                }
                serverPlayer.room = room;
                room.AddPlayer(serverPlayer.player);
                RoomMsg.AsyncPostRoom(id, this, room);
                foreach (PlayerInRoom playerInRoom in serverPlayer.room.Players)
                {
                    ServerPlayer otherServerPlayer; //Schrödinger Variable
                    Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                    if (otherServerPlayer == null)  //WTF!?
                        throw new Exception("Whatever.");
                    PlayerJoinedMsg.asycnPostJoined(otherServerPlayer.connection, serverPlayer.player);
					if (serverPlayer.player.Id != otherServerPlayer.player.Id)
						PlayerJoinedMsg.asycnPostJoined(this, otherServerPlayer.player);
                }
                NewRoomOwnerMsg.AsyncPostNewOwner(this, room.OwnerId);
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
        }

        private void LeaveRoom(Int16 id)
        {
            if (!InRoom)
            {
                ErrorMsg.AsyncPostError(id, this, ErrorCode.NOT_IN_ROOM);
                return;
            }
            OkMsg.AsyncPostOk(id, this);
            OnLeaveRoom();
        }

        private void PlayerReady()
        {
            if (!InRoom)
                return;
            Globals.dataLock.AcquireWriterLock(-1);
            try
            {
                foreach (PlayerInRoom playerInRoom in serverPlayer.room.Players)
                {
                    ServerPlayer otherServerPlayer;
                    Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                    PlayerReadyMsg.AsyncPostReady(this, serverPlayer.player.Id);
                }
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
        }

        private void PlayerNotReady()
        {
            if (!InRoom)
                return;
            Globals.dataLock.AcquireWriterLock(-1);
            try
            {
                foreach (PlayerInRoom playerInRoom in serverPlayer.room.Players)
                {
                    ServerPlayer otherServerPlayer;
                    Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                    PlayerNotReadyMsg.AsyncPostNotReady(this, serverPlayer.player.Id);
                }
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
        }

        private void Chat(ChatMessage chatMessage)
        {
            chatMessage.PlayerId = serverPlayer.player.Id;
            if (!InRoom)
                return;
            Globals.dataLock.AcquireReaderLock(-1);
            try
            {
                foreach (PlayerInRoom playerInRoom in serverPlayer.room.Players)
                {
                    ServerPlayer otherServerPlayer;
                    Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                    ChatMsg.AsyncPostChat(this, chatMessage);
                }
            }
            finally
            {
                Globals.dataLock.ReleaseReaderLock();
            }
        }

        private void MakeValidPlayer()
        {
            if (serverPlayer == null)
            {
                Globals.dataLock.AcquireWriterLock(-1);
                try
                {
                    UInt32 id = PlayerTable.Create();
                    serverPlayer = new ServerPlayer(this, new Player(id, false, DEFAULT_NICK + '#' + id));
                    Globals.players.TryAdd(id, serverPlayer);
                }
                finally
                {
                    Globals.dataLock.ReleaseWriterLock();
                }
            }
        }

        protected override void DoDispose()
        {
            OnLeaveRoom();
            OnLogout();
            base.DoDispose();
        }

    }
}
