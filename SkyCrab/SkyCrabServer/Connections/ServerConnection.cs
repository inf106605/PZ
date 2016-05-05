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
                switch (messageInfo.messageId)
                {
                    //COMMON

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

                    //MENU

                    //accounts

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

                    //friends

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

                    //room

                    case MessageId.CREATE_ROOM:
                        CreateRoom((Room)messageInfo.message);
                        break;

                    case MessageId.FIND_ROOMS:
                        FindRooms((Room)messageInfo.message);
                        break;

                    case MessageId.GET_FRIEND_ROOMS:
                        GetFriendRooms();
                        break;

                    default:
                        throw new UnsuportedMessageException();

                    //in rooms

                    case MessageId.JOIN_ROOM:
                        JoinRoom((UInt32)messageInfo.message);
                        break;

                    case MessageId.LEAVE_ROOM:
                        LeaveRoom();
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
            string info = "Client disconnected. (" + ClientAuthority + ")";
            Globals.serverConsole.WriteLine(info);
        }

        private void Login(PlayerProfile playerProfile)
        {
            if (Logged)
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
            ServerPlayer serverPlayer = new ServerPlayer(this, player);
            if (!Globals.players.TryAdd(player.Id, serverPlayer))
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.USER_ALREADY_LOGGED);
                return;
            }

            this.serverPlayer = serverPlayer;
            LoginOkMsg.AsyncPostLoginOk(this, player);
        }

        private void Logout()
        {
            if (!Logged)
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.NOT_LOGGED);
                return;

            }

            OnLogout();
            OkMsg.AsyncPostOk(this);
        }

        private void OnLogout()
        {
            if (serverPlayer != null)
            {
                {
                    ServerPlayer temp;
                    Globals.players.TryRemove(serverPlayer.player.Id, out temp);
                }
                if (InRoom)
                    OnLeaveRoom();
                serverPlayer = null;
            }
        }

        private void Register(PlayerProfile playerProfile)
        {
            lock (PlayerProfileTable._lock)
            {
                if (this.serverPlayer != null)
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

                serverPlayer = new ServerPlayer(this, new Player(id, playerProfile));
                Globals.players.TryAdd(serverPlayer.player.Id, serverPlayer);
                LoginOkMsg.AsyncPostLoginOk(this, serverPlayer.player);
            }
        }

        private void EditProfile(PlayerProfile playerProfile)
        {
            if (!Logged)
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.NOT_LOGGED2);
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
                    ErrorMsg.AsyncPostError(this, ErrorCode.NICK_IS_TOO_SHITTY);
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
                    ErrorMsg.AsyncPostError(this, ErrorCode.EMAIL_OCCUPIED2);
                    return;
                }
            }

            PlayerProfileTable.Modify(serverPlayer.player.Id, playerProfile, passwordHash);

            serverPlayer.player.Profile.Nick = playerProfile.Nick;
            serverPlayer.player.Profile.EMail = playerProfile.EMail;
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

        private static String DecoratePassword(PlayerProfile profile)
        {
            const string SALT = "L4RUmhBUM9sIbwg7hPEJMBSPo\\vFxfeJH*c?pt@&Rk0L)0EC obw1s71(!xn<6f$WFdc6,[@lPh%PTYv6iv}DU13 mwYY.hh8tL9h";
            string decoratedPassword = profile.Password + SALT + profile.Login;
            return decoratedPassword;
        }

        private void GetFriends()
        {
            if (!Logged)
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.NOT_LOGGED3);
                return;
            }
            List<UInt32> friendIds = FriendTable.GetByPlayerId(serverPlayer.player.Id);
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
            if (!Logged)
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.NOT_LOGGED4);
                return;
            }
            if (serverPlayer.player.Id == friendId)
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.FOREVER_ALONE);
                return;
            }
            if (!PlayerProfileTable.IdExists(friendId))
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.NO_SUCH_PLAYER);
                return;
            }
            if (FriendTable.Exists(serverPlayer.player.Id, friendId))
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.FRIEND_ALREADY_ADDED);
                return;
            }

            FriendTable.Create(serverPlayer.player.Id, friendId);
            OkMsg.AsyncPostOk(this);
        }

        private void RemoveFriend(UInt32 friendId)
        {
            if (!Logged)
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.NOT_LOGGED5);
                return;
            }
            if (!FriendTable.Exists(serverPlayer.player.Id, friendId))
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.NO_SUCH_FRIEND);
                return;
            }

            FriendTable.Delete(serverPlayer.player.Id, friendId);
            OkMsg.AsyncPostOk(this);
        }

        private void CreateRoom(Room room)
        {
            MakeValidPlayer();
            if (InRoom)
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.ALREADY_IN_ROOM);
                return;
            }
            bool rulesAreValid = room.Rules.maxRoundTime.value <= 3600 &&
                    room.Rules.maxPlayerCount.value <= 4;
            if (!rulesAreValid)
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.INVALID_RULES);
                return;
            }
            room.Id = Globals.roomIdSequence.Value;
            room.Owner = serverPlayer.player;
            room.AddPlayer(serverPlayer.player);
            Globals.rooms.TryAdd(room.Id, room);
            serverPlayer.room = room;
            RoomMsg.AsyncPostRoomList(this, room);
            PlayerJoinedMsg.asycnPostJoined(this, serverPlayer.player);
        }

        private void OnLeaveRoom()
        {
            serverPlayer.room.RemovePlayer(serverPlayer.player.Id);
            if (serverPlayer.room.Players.Count == 0)
            {
                serverPlayer.room.Owner = null;
                Room tmp;
                Globals.rooms.TryRemove(serverPlayer.room.Id, out tmp);
            }
            else
            {
                if (serverPlayer.room.Owner.Id == serverPlayer.player.Id)
                {
                    serverPlayer.room.Owner = serverPlayer.room.Players.First.Value.Player;
                    foreach (PlayerInRoom playerInRoom in serverPlayer.room.Players)
                    {
                        ServerPlayer otherServerPlayer;
                        Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                        NewRoomOwnerMsg.AsyncPostNewOwner(this, serverPlayer.room.Owner.Id);
                    }
                }
                foreach (PlayerInRoom playerInRoom in serverPlayer.room.Players)
                {
                    ServerPlayer otherServerPlayer;
                    Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                    PlayerLeavedMsg.AsyncPostLeave(this, serverPlayer.player.Id);
                }
            }
            serverPlayer.room = null;
        }

        private void FindRooms(Room roomFilter)
        {
            List<Room> foundRooms = new List<Room>();
            foreach (KeyValuePair<UInt32, Room> pair in Globals.rooms)
            {
                if (RoomMath(pair.Value, roomFilter))
                {
                    foundRooms.Add(pair.Value);
                    if (foundRooms.Count == ListTranscoder<object>.MAX_COUNT)
                        break;
                }
            }
            RoomListMsg.AsyncPostRoomList(this, foundRooms);
        }

        private void GetFriendRooms()
        {
            if (!Logged)
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.NOT_LOGGED6);
                return;
            }
            List<UInt32> friendIds = FriendTable.GetByPlayerId(serverPlayer.player.Id);
            List<Room> foundRooms = new List<Room>();
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
            RoomListMsg.AsyncPostRoomList(this, foundRooms);
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
            if (room.RoomType == RoomType.FRIENDS && FriendTable.Exists(room.Owner.Id, serverPlayer.player.Id))
                return false;
            return true;
        }

        private void JoinRoom(UInt32 roomId)
        {
            if (InRoom)
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.ALREADY_IN_ROOM2);
                return;
            }
            Room room;
            if (!Globals.rooms.TryGetValue(roomId, out room))
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.NO_SUCH_ROOM);
                return;
            }
            if (room.Players.Count >= room.Rules.maxPlayerCount.value)
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.ROOM_IS_FULL);
                return;
            }
            serverPlayer.room = room;
            room.AddPlayer(serverPlayer.player);
            RoomMsg.AsyncPostRoomList(this, room);
            foreach (PlayerInRoom playerInRoom in serverPlayer.room.Players)
            {
                ServerPlayer otherServerPlayer;
                Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                PlayerJoinedMsg.asycnPostJoined(this, serverPlayer.player);
            }
        }

        private void LeaveRoom()
        {
            if (!InRoom)
            {
                ErrorMsg.AsyncPostError(this, ErrorCode.NOT_IN_ROOM);
                return;
            }
            OkMsg.AsyncPostOk(this);
            OnLeaveRoom();
        }

        private void PlayerReady()
        {
            if (!InRoom)
                return;
            foreach (PlayerInRoom playerInRoom in serverPlayer.room.Players)
            {
                ServerPlayer otherServerPlayer;
                Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                PlayerReadyMsg.AsyncPostReady(this, serverPlayer.player.Id);
            }
        }

        private void PlayerNotReady()
        {
            if (!InRoom)
                return;
            foreach (PlayerInRoom playerInRoom in serverPlayer.room.Players)
            {
                ServerPlayer otherServerPlayer;
                Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                PlayerNotReadyMsg.AsyncPostNotReady(this, serverPlayer.player.Id);
            }
        }

        private void Chat(ChatMessage chatMessage)
        {
            chatMessage.PlayerId = serverPlayer.player.Id;
            if (!InRoom)
                return;
            foreach (PlayerInRoom playerInRoom in serverPlayer.room.Players)
            {
                ServerPlayer otherServerPlayer;
                Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                ChatMsg.AsyncPostChat(this, chatMessage);
            }
        }

        private void MakeValidPlayer()
        {
            if (serverPlayer == null)
            {
                UInt32 id = PlayerTable.Create();
                serverPlayer = new ServerPlayer(this, new Player(id, false, DEFAULT_NICK+'#'+id));
            }
        }

        protected override void DoDispose()
        {
            OnLogout();
            base.DoDispose();
        }

    }
}
