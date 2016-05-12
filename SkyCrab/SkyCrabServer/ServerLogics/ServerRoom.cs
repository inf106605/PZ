using SkyCrab.Common_classes.Chats;
using SkyCrab.Common_classes.Rooms;
using SkyCrab.Common_classes.Rooms.Players;
using SkyCrab.Connection.PresentationLayer.DataTranscoders.NativeTypes;
using SkyCrab.Connection.PresentationLayer.Messages;
using SkyCrab.Connection.PresentationLayer.Messages.Common.Errors;
using SkyCrab.Connection.PresentationLayer.Messages.Menu.Rooms;
using SkyCrabServer.Databases;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SkyCrabServer.ServerLogics
{
    sealed class ServerRoom
    {

        private readonly ServerPlayer serverPlayer;
        public Room room;

        private readonly Semaphore startGameSemaphore = new Semaphore(0, 1);
        private Task startGameTimer;


        public bool InRoom
        {
            get
            {
                return room != null;
            }
        }


        public ServerRoom(ServerPlayer serverPlayer)
        {
            this.serverPlayer = serverPlayer;
        }

        public void CreateRoom(Int16 id, Room newRoom)
        {
            if (!serverPlayer.LoggedAnyway)
            {
                ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NOT_LOGGED7);
                return;
            }
            if (InRoom)
            {
                ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.ALREADY_IN_ROOM);
                return;
            }
            bool rulesAreValid = newRoom.Rules.maxTurnTime.value <= 3600 &&
                    newRoom.Rules.maxPlayerCount.value <= 4;
            if (!rulesAreValid)
            {
                ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.INVALID_RULES);
                return;
            }
            room = newRoom;
            room.Id = Globals.roomIdSequence.Value;
            room.AddPlayer(serverPlayer.player);
            room.OwnerId = serverPlayer.player.Id;
            Globals.dataLock.AcquireWriterLock(-1);
            try
            {
                Globals.rooms.TryAdd(room.Id, this);
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
            RoomMsg.AsyncPost(id, serverPlayer.connection, room);
            PlayerJoinedMsg.asycnPost(serverPlayer.connection, serverPlayer.player);
            NewRoomOwnerMsg.AsyncPost(serverPlayer.connection, serverPlayer.player.Id);
        }

        public void FindRooms(Int16 id, Room roomFilter)
        {
            List<Room> foundRooms = new List<Room>();
            Globals.dataLock.AcquireReaderLock(-1);
            try
            {
                foreach (KeyValuePair<UInt32, ServerRoom> pair in Globals.rooms)
                {
                    ServerRoom foundServerRoom = pair.Value;
                    if (foundServerRoom.serverPlayer.serverGame.InGame)
                        continue;
                    if (!RoomMath(foundServerRoom.room, roomFilter))
                        continue;
                    foundRooms.Add(foundServerRoom.room);
                    if (foundRooms.Count == ListTranscoder<object>.MAX_COUNT)
                        break;
                }
            }
            finally
            {
                Globals.dataLock.ReleaseReaderLock();
            }
            RoomListMsg.AsyncPost(id, serverPlayer.connection, foundRooms);
        }

        public void GetFriendRooms(Int16 id)
        {
            if (!serverPlayer.LoggedNormally)
            {
                ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NOT_LOGGED6);
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
                        if (!serverFriend.serverRoom.InRoom)
                            continue;
                        if (serverFriend.serverGame.InGame)
                            continue;
                        if (!RoomTypeMath(serverFriend.serverRoom.room))
                            continue;
                        foundRooms.Add(serverFriend.serverRoom.room);
                        if (foundRooms.Count == ListTranscoder<object>.MAX_COUNT)
                            break;
                    }
                }
            }
            finally
            {
                Globals.dataLock.ReleaseReaderLock();
            }
            RoomListMsg.AsyncPost(id, serverPlayer.connection, foundRooms);
        }

        public bool RoomMath(Room room, Room roomFilter)
        {
            if (!room.Name.Contains(roomFilter.Name))
                return false;
            if (!room.Rules.Math(roomFilter.Rules))
                return false;
            if (!RoomTypeMath(room))
                return false;
            return true;
        }

        public bool RoomTypeMath(Room room)
        {
            if (room.Type == RoomType.PRIVATE)
                return false;
            if (room.Type == RoomType.FRIENDS && FriendTable.Exists(room.OwnerId, serverPlayer.player.Id))
                return false;
            return true;
        }

        public void JoinRoom(Int16 id, UInt32 roomId)
        {
            if (!serverPlayer.LoggedAnyway)
            {
                ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NOT_LOGGED8);
                return;
            }
            if (InRoom)
            {
                ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.ALREADY_IN_ROOM2);
                return;
            }
            ServerRoom newServerRoom;
            Globals.dataLock.AcquireWriterLock(-1);
            try
            {
                if (!Globals.rooms.TryGetValue(roomId, out newServerRoom))
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NO_SUCH_ROOM);
                    return;
                }
                if (newServerRoom.serverPlayer.serverGame.InGame)
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.GAME_IS_STARTED);
                    return;
                }
                if (newServerRoom.room.Players.Count >= newServerRoom.room.Rules.maxPlayerCount.value)
                {
                    ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.ROOM_IS_FULL);
                    return;
                }
                room = newServerRoom.room;
                room.AddPlayer(serverPlayer.player);
                RoomMsg.AsyncPost(id, serverPlayer.connection, room);
                foreach (PlayerInRoom playerInRoom in serverPlayer.serverRoom.room.Players)
                {
                    ServerPlayer otherServerPlayer; //Schrödinger Variable
                    Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                    if (otherServerPlayer == null)  //WTF!?
                        throw new Exception("Whatever...");
                    PlayerJoinedMsg.asycnPost(otherServerPlayer.connection, serverPlayer.player);
                    if (serverPlayer.player.Id != otherServerPlayer.player.Id)
                        PlayerJoinedMsg.asycnPost(serverPlayer.connection, otherServerPlayer.player);
                }
                NewRoomOwnerMsg.AsyncPost(serverPlayer.connection, room.OwnerId);
                ClearStatuses();
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
        }

        public void LeaveRoom(Int16 id)
        {
            if (!InRoom)
            {
                ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NOT_IN_ROOM);
                return;
            }
            OkMsg.AsyncPost(id, serverPlayer.connection);
            OnLeaveRoom();
        }

        public void PlayerReady(Int16 id)
        {
            if (!InRoom)
            {
                ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NOT_IN_ROOM2);
                return;
            }
            if (serverPlayer.serverGame.InGame)
            {
                ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.IN_GAME);
                return;
            }
            Globals.dataLock.AcquireWriterLock(-1);
            try
            {
                serverPlayer.serverRoom.room.GetPlayer(serverPlayer.player.Id).IsReady = true;
                OkMsg.AsyncPost(id, serverPlayer.connection);
                foreach (PlayerInRoom playerInRoom in serverPlayer.serverRoom.room.Players)
                {
                    ServerPlayer otherServerPlayer; //Schrödinger Variable
                    Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                    if (otherServerPlayer == null)  //WTF!?
                        throw new Exception("Whatever...");
                    PlayerReadyMsg.AsyncPost(otherServerPlayer.connection, serverPlayer.player.Id, null);
                }
                serverPlayer.serverRoom.CheckAllPlayersReady();
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
        }

        public void PlayerNotReady(Int16 id)
        {
            if (!InRoom)
            {
                ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NOT_IN_ROOM);
                return;
            }
            if (serverPlayer.serverGame.InGame)
            {
                ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.IN_GAME2);
                return;
            }
            Globals.dataLock.AcquireWriterLock(-1);
            try
            {
                serverPlayer.serverRoom.room.GetPlayer(serverPlayer.player.Id).IsReady = false;
                OkMsg.AsyncPost(id, serverPlayer.connection);
                foreach (PlayerInRoom playerInRoom in serverPlayer.serverRoom.room.Players)
                {
                    ServerPlayer otherServerPlayer; //Schrödinger Variable
                    Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                    if (otherServerPlayer == null)  //WTF!?
                        throw new Exception("Whatever...");
                    PlayerNotReadyMsg.AsyncPost(otherServerPlayer.connection, serverPlayer.player.Id, null);
                }
                serverPlayer.serverRoom.CancelStartingGame();
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
        }

        public void Chat(Int16 id, ChatMessage chatMessage)
        {
            if (!InRoom)
            {
                ErrorMsg.AsyncPost(id, serverPlayer.connection, ErrorCode.NOT_IN_ROOM);
                return;
            }
            chatMessage.PlayerId = serverPlayer.player.Id;
            Globals.dataLock.AcquireReaderLock(-1);
            try
            {
                OkMsg.AsyncPost(id, serverPlayer.connection);
                foreach (PlayerInRoom playerInRoom in serverPlayer.serverRoom.room.Players)
                {
                    ServerPlayer otherServerPlayer; //Schrödinger Variable
                    Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                    if (otherServerPlayer == null)  //WTF!?
                        throw new Exception("Whatever...");
                    ChatMsg.AsyncPost(otherServerPlayer.connection, chatMessage, null);
                }
            }
            finally
            {
                Globals.dataLock.ReleaseReaderLock();
            }
        }

        private void ClearStatuses()
        {
            foreach (PlayerInRoom playerInRoom in serverPlayer.serverRoom.room.Players)
            {
                ServerPlayer otherServerPlayer; //Schrödinger Variable
                Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                if (otherServerPlayer == null)  //WTF!?
                    throw new Exception("Whatever...");
                foreach (PlayerInRoom playerInRoom2 in serverPlayer.serverRoom.room.Players)
                    PlayerNotReadyMsg.AsyncPost(otherServerPlayer.connection, playerInRoom2.Player.Id, null);
            }
            serverPlayer.serverRoom.CancelStartingGame();
        }

        private void CheckAllPlayersReady()
        {
            if (serverPlayer.serverGame.InGame)
                return;
            if (!room.AllPlayersReady)
                return;
            if (startGameTimer != null)
                return;
            startGameTimer = Task.Factory.StartNew(StartGameTimerTaskBody);
        }

        private void CancelStartingGame()
        {
            if (startGameTimer == null)
                return;
            startGameSemaphore.Release();
            startGameTimer.Wait();
            if (startGameSemaphore.WaitOne(0))
                return;
            startGameTimer = null;
            return;
        }

        private void StartGameTimerTaskBody()
        {
            if (startGameSemaphore.WaitOne(10000))
                return;
            Task.Factory.StartNew(StartGame);
        }

        public void StartGame()
        {
            serverPlayer.serverGame.StartGame();
            startGameTimer = null;
        }

        public void OnLeaveRoom()
        {
            serverPlayer.serverGame.OnQuitGame();
            Globals.dataLock.AcquireWriterLock(-1);
            try
            {
                if (InRoom)
                {
                    serverPlayer.serverRoom.room.RemovePlayer(serverPlayer.player.Id);
                    if (serverPlayer.serverRoom.room.Players.Count == 0)
                    {
                        ServerRoom tmp;
                        Globals.rooms.TryRemove(serverPlayer.serverRoom.room.Id, out tmp);
                    }
                    else
                    {
                        if (serverPlayer.serverRoom.room.OwnerId == 0)
                        {
                            serverPlayer.serverRoom.room.OwnerId = serverPlayer.serverRoom.room.Players.First.Value.Player.Id;
                            foreach (PlayerInRoom playerInRoom in serverPlayer.serverRoom.room.Players)
                            {
                                ServerPlayer otherServerPlayer; //Schrödinger Variable
                                Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                                if (otherServerPlayer == null)  //WTF!?
                                    throw new Exception("Whatever...");
                                NewRoomOwnerMsg.AsyncPost(otherServerPlayer.connection, serverPlayer.serverRoom.room.OwnerId);
                            }
                        }
                        foreach (PlayerInRoom playerInRoom in serverPlayer.serverRoom.room.Players)
                        {
                            playerInRoom.IsReady = false;
                            ServerPlayer otherServerPlayer; //Schrödinger Variable
                            Globals.players.TryGetValue(playerInRoom.Player.Id, out otherServerPlayer);
                            if (otherServerPlayer == null)  //WTF!?
                                throw new Exception("Whatever...");
                            PlayerLeavedMsg.AsyncPost(otherServerPlayer.connection, serverPlayer.player.Id);
                        }
                    }
                    ClearStatuses();
                    room = null;
                }
            }
            finally
            {
                Globals.dataLock.ReleaseWriterLock();
            }
        }

    }
}
